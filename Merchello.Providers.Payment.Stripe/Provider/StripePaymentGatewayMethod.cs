using System;
using Stripe;
using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Payments.Stripe;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Merchello.Providers.Payment.Stripe.Services;

namespace Merchello.Providers.Payment.Stripe.Provider
{
    [GatewayMethodUi("Stripe.Payment")]
    [GatewayMethodEditor("Stripe Payment Method Editor", "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/payment.paymentmethod.addedit.html")]
    public class StripePaymentGatewayMethod : PaymentGatewayMethodBase, IStripePaymentGatewayMethod
    {
        public StripePaymentGatewayMethod(IGatewayProviderService gatewayProviderService, IPaymentMethod paymentMethod, IStripeApiService stripeApiService)
            : base(gatewayProviderService, paymentMethod)
        {
            Mandate.ParameterNotNull(stripeApiService, "stripeApiService");

            this.StripeApiService = stripeApiService;
            this.Initialize();
        }

        /// <summary>
        /// Gets the Stripe API service.
        /// </summary>
        protected IStripeApiService StripeApiService { get; private set; }

        /// <summary>
        /// Gets or sets the back office payment method name.
        /// </summary>
        /// <remarks>
        /// This defaults to the original payment method name set in the back office
        /// </remarks>
        protected virtual string BackOfficePaymentMethodName { get; set; }

        /// <summary>
        /// Performs the actual work of authorizing and capturing the payment.
        /// </summary>
        /// <param name="invoice">
        /// The invoice.
        /// </param>
        /// <param name="amount">
        /// The amount.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        protected override IPaymentResult PerformAuthorizeCapturePayment(IInvoice invoice, decimal amount, ProcessorArgumentCollection args)
        {
            var paymentMethodToken = args.GetTokenId();

            if (string.IsNullOrEmpty(paymentMethodToken))
            {
                var error = new InvalidOperationException("No payment method token was found in the ProcessorArgumentCollection");
                LogHelper.Debug<StripePaymentGatewayMethod>(error.Message);
                return new PaymentResult(Attempt<IPayment>.Fail(error), invoice, false);
            }

            var attempt = this.ProcessPayment(invoice, TransactionOption.AuthorizeAndCapture, amount, paymentMethodToken, args.GetCustomerName(), args.GetCustomerEmail());

            var payment = attempt.Payment.Result;

            this.GatewayProviderService.Save(payment);

            if (!attempt.Payment.Success)
            {
                this.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Denied, attempt.Payment.Exception.Message, 0);
            }
            else
            {
                this.GatewayProviderService.ApplyPaymentToInvoice(payment.Key, invoice.Key, AppliedPaymentType.Debit, "Stripe Payment - authorized and captured", amount);
            }

            return attempt;
        }

        protected override IPaymentResult PerformAuthorizePayment(IInvoice invoice, ProcessorArgumentCollection args)
        {
            throw new NotImplementedException();
        }

        protected override IPaymentResult PerformCapturePayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            throw new NotImplementedException();
        }

        protected override IPaymentResult PerformRefundPayment(IInvoice invoice, IPayment payment, decimal amount, ProcessorArgumentCollection args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Processes a payment against the Stripe API using Stripe.net
        /// </summary>
        /// <param name="invoice"></param>
        /// <param name="option"></param>
        /// <param name="amount"></param>
        /// <param name="token"></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="telephone"></param>
        /// <returns>
        /// The <see cref="IPaymentResult"/>.
        /// </returns>
        public IPaymentResult ProcessPayment(IInvoice invoice, TransactionOption option, decimal amount, string token, string name = "", string email = "")
        {
            var payment = this.GatewayProviderService.CreatePayment(Core.PaymentMethodType.CreditCard, amount, this.PaymentMethod.Key);

            payment.CustomerKey = invoice.CustomerKey;
            payment.Authorized = false;
            payment.Collected = false;
            payment.PaymentMethodName = this.BackOfficePaymentMethodName;
            payment.ExtendedData.SetValue(Constants.Stripe.ProcessorArguments.TokenId, token);

            // Fail payment if the currency code isn't valid
            if (!IsValidCurrencyCode(invoice.CurrencyCode()))
            {
                return new PaymentResult(
                    Attempt<IPayment>.Fail(payment, new Exception("Payment could not be made")),
                                invoice, false);
            }

            // These are currently our only two options so let's just check for them for now
            if (option == TransactionOption.Authorize || option == TransactionOption.AuthorizeAndCapture)
            {
                // Empty variables for usage by the Stripe API
                StripeResponse response;
                StripeCustomer customer;
                StripeCharge charge;

                // Make the payment using Stripe API
                var secretKey = StripeApiService.StripeProviderSettings.SecretKey;
                StripeConfiguration.SetApiKey(secretKey);

                // Create a Stripe customer
                var customerOptions = new StripeCustomerCreateOptions
                {
                    Email = email,
                    Description = name,
                    SourceToken = token
                };

                var customerService = new StripeCustomerService();
                customer = customerService.Create(customerOptions);

                payment.ExtendedData.SetValue(Constants.Stripe.ProcessorArguments.CustomerId, customer.Id);

                response = customer.StripeResponse;

                if (response != null)
                {
                    // Create a Stripe charge
                    var chargeOptions = new StripeChargeCreateOptions
                    {
                        Amount = ConvertAmount(invoice, amount),
                        Currency = invoice.CurrencyCode().ToLower(),
                        Description = "Valley Store Order",
                        CustomerId = customer.Id,
                        Capture = option == TransactionOption.AuthorizeAndCapture,
                    };

                    var chargeService = new StripeChargeService();
                    charge = chargeService.Create(chargeOptions);

                    // Get the status and response of the charge
                    string status = charge.Status;
                    response = charge.StripeResponse;

                    if (response != null && status == "succeeded")
                    {
                        payment.Authorized = true;
                        payment.Collected = option == TransactionOption.AuthorizeAndCapture;
                        payment.ExtendedData.SetValue(Constants.Stripe.ProcessorArguments.ChargeId, charge.Id);

                        return new PaymentResult(Attempt<IPayment>.Succeed(payment), invoice, true);
                    }
                }
            }

            return new PaymentResult(
                Attempt<IPayment>.Fail(payment, new Exception("Payment could not be made")),
                            invoice, false);
        }


        private static int ConvertAmount(IInvoice invoice, decimal amount)
        {
            // need to convert non-zero-decimal currencies
            bool isZeroDecimalCurrency = IsZeroDecimalCurrency(invoice.CurrencyCode());
            decimal stripeAmountDecimal = isZeroDecimalCurrency ? amount : (amount * 100);
            return Convert.ToInt32(stripeAmountDecimal);
        }

        /// <summary>
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        private static bool IsValidCurrencyCode(string currencyCode)
        {
            switch (currencyCode)
            {
                // TODO: add other valid codes
                case "GBP": // British Pound
                case "DOP": // Dominican Peso
                case "USD": // US Dollar
                case "CAD": // Canadian Dollar
                case "EUR": // Euro
                case "NIO": // Nicaraguan Córdoba
                case "DKK": // Danish Krone
                case "PEN": // Peruvian Nuevo Sol
                    return true;
            }
            return IsZeroDecimalCurrency(currencyCode);
        }

        /// <summary>
        ///     See https://support.stripe.com/questions/which-zero-decimal-currencies-does-stripe-support
        /// </summary>
        /// <param name="currencyCode"></param>
        /// <returns></returns>
        private static bool IsZeroDecimalCurrency(string currencyCode)
        {
            switch (currencyCode)
            {
                case "BIF":
                case "DJF":
                case "JPY":
                case "KRW":
                case "PYG":
                case "VND":
                case "XAF":
                case "XPF":
                case "CLP":
                case "GNF":
                case "KMF":
                case "MGA":
                case "RWF":
                case "VUV":
                case "XOF":
                    return true;
            }
            return false;
        }


        private void Initialize()
        {
            this.BackOfficePaymentMethodName = PaymentMethod.Name;
        }
    }
}