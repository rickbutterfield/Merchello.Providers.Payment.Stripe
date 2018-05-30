using Merchello.Core;
using Merchello.Core.Events;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Logging;
using Merchello.Plugin.Payments.Stripe.Models;
using Merchello.Providers.Models;
using Merchello.Providers.Payment.Stripe.Models;
using Merchello.Providers.Payment.Stripe.Services;
using Merchello.Web.Controllers;
using Merchello.Web.Store.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Web.Mvc;

namespace Merchello.Providers.Payment.Stripe.Controllers
{
    [PluginController("Merchello")]
    [GatewayMethodUi("Stripe.Payment")]
    public class StripePaymentController : CheckoutPaymentControllerBase<StripePaymentModel>
    {
        /// <summary>
        /// Occurs after the final redirection and before redirecting to the success URL
        /// </summary>
        /// <remarks>
        /// Can be used to send OrderConfirmation notification
        /// </remarks>
        public static event TypedEventHandler<StripePaymentController, PaymentAttemptEventArgs<IPaymentResult>> Processed;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Process(StripePaymentModel model)
        {
            try
            {
                var paymentMethod = CheckoutManager.Payment.GetPaymentMethod();
                if (paymentMethod == null)
                {
                    var ex = new NullReferenceException("PaymentMethod was null");
                    return HandlePaymentException(model, ex);
                }

                var args = new ProcessorArgumentCollection();
                args.SetTokenId(model.Token);
                args.SetCustomerName(model.Name);
                //args.SetCustomerEmail(model.Email);

                // We can now capture the payment
                // This will actually make a few more API calls back to Stripe to get required transaction
                // data so that we can refund the payment later through the back office if needed.
                var attempt = CheckoutManager.Payment.AuthorizeCapturePayment(paymentMethod.Key, args);

                // Raise the event to process the email
                Processed.RaiseEvent(new PaymentAttemptEventArgs<IPaymentResult>(attempt), this);

                var resultModel = CheckoutPaymentModelFactory.Create(CurrentCustomer, paymentMethod, attempt);
                resultModel.SuccessRedirectUrl = model.SuccessRedirectUrl;

                if (attempt.Payment.Success)
                {
                    CustomerContext.SetValue("invoiceKey", attempt.Invoice.Key.ToString());
                }

                if (!resultModel.ViewData.Success)
                {
                    var invoiceKey = attempt.Invoice.Key;
                    var paymentKey = attempt.Payment.Result != null ? attempt.Payment.Result.Key : Guid.Empty;
                    EnsureDeleteInvoiceOnCancel(invoiceKey, paymentKey);
                }

                return HandlePaymentSuccess(resultModel);
            }
            catch (Exception ex)
            {
                return HandlePaymentException(model, ex);
            }
        }

        protected override ActionResult HandlePaymentSuccess(StripePaymentModel model)
        {
            return model.ViewData.Success && !model.SuccessRedirectUrl.IsNullOrWhiteSpace() ?
                Redirect(model.SuccessRedirectUrl) :
                base.HandlePaymentSuccess(model);
        }

        /// <summary>
        /// Render the Stripe payment form.
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [ChildActionOnly]
        [GatewayMethodUi("Stripe.Payment")]
        public override ActionResult PaymentForm(string view = "")
        {
            var provider = GatewayContext.Payment.GetProviderByKey(Constants.Stripe.GatewayProviderSettingsKey);
            var settings = provider.ExtendedData.GetStripeProviderSettings();

            var paymentMethod = this.CheckoutManager.Payment.GetPaymentMethod();
            var billing = this.CheckoutManager.Customer.GetBillToAddress();
            if (paymentMethod == null) return this.InvalidCheckoutStagePartial();

            var model = this.CheckoutPaymentModelFactory.Create(CurrentCustomer, paymentMethod);
            model.PublishableKey = settings.PublishableKey;
            model.SuccessRedirectUrl = settings.SuccessRedirectUrl;
            model.Name = billing.Name;
            //model.Email = billing.Email;

            return view.IsNullOrWhiteSpace() ? this.PartialView(model) : this.PartialView(view, model);
        }

        /// <summary>
        /// Deletes the invoice on cancel.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="paymentKey">
        /// The payment key.
        /// </param>
        /// <returns>
        /// The <see cref="PayPalProviderSettings"/>.
        /// </returns>
        private StripeProviderSettings EnsureDeleteInvoiceOnCancel(Guid invoiceKey, Guid paymentKey)
        {
            var provider = GatewayContext.Payment.GetProviderByKey(Constants.Stripe.GatewayProviderSettingsKey);
            var settings = provider.ExtendedData.GetProcessorSettings();

            if (settings.DeleteInvoiceOnCancel)
            {
                // validate that this invoice should be deleted
                var invoice = MerchelloServices.InvoiceService.GetByKey(invoiceKey);

                var payments = invoice.Payments().ToArray();

                // we don't want to delete if there is more than one payment
                if (payments.Count() <= 1)
                {
                    // Assert the payment key matches - this is to ensure that the 
                    // payment matches the invoice
                    var ensure = payments.All(x => x.Key == paymentKey) || !payments.Any();
                    if (ensure && invoice.InvoiceStatus.Key == Core.Constants.InvoiceStatus.Unpaid)
                    {
                        MultiLogHelper.Info<StripePaymentController>(string.Format("Deleted invoice number {0} to prevent duplicate. Stripe response not success", invoice.PrefixedInvoiceNumber()));
                        MerchelloServices.InvoiceService.Delete(invoice);
                    }
                }
            }

            return settings;
        }
    }
}