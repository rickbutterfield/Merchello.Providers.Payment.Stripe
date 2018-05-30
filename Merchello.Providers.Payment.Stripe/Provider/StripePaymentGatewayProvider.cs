using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Payments.Stripe.Models;
using Merchello.Providers.Payment.Stripe.Models;
using Merchello.Providers.Payment.Stripe.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Logging;

namespace Merchello.Providers.Payment.Stripe.Provider
{
    [GatewayProviderActivation(Constants.Stripe.GatewayProviderKey, "Stripe Payment Provider", "Stripe Payment Provider")]
    [GatewayProviderEditor("Stripe Configuration", "~/App_Plugins/Merchello.Plugins.Stripe/payment.stripe.providersettings.controller.html")]
    [ProviderSettingsMapper(Constants.Stripe.ExtendedDataKeys.ProviderSettings, typeof(StripeProviderSettings))]
    public class StripePaymentGatewayProvider : PaymentGatewayProviderBase, IStripePaymentGatewayProvider
    {
        internal static readonly IEnumerable<IGatewayResource> AvailableResources = new List<IGatewayResource>
        {
            new GatewayResource(Constants.Stripe.PaymentCodes.Transaction, "Stripe Transaction")
        };

        public StripePaymentGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProviderSettings gatewayProviderSettings, Umbraco.Core.Cache.IRuntimeCacheProvider runtimeCacheProvider)
            : base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        {
        }

        public override IPaymentGatewayMethod CreatePaymentMethod(IGatewayResource gatewayResource, string name, string description)
        {
            var available = this.ListResourcesOffered().FirstOrDefault(x => x.ServiceCode == gatewayResource.ServiceCode);

            if (available == null)
            {
                var error = new InvalidOperationException("The GatewayResource has already been assigned.");

                LogHelper.Error<StripePaymentGatewayProvider>("GatewayResource has alread been assigned", error);

                throw error;
            }

            var attempt = this.GatewayProviderService.CreatePaymentMethodWithKey(this.GatewayProviderSettings.Key, name, description, available.ServiceCode);

            if (attempt.Success)
            {
                this.PaymentMethods = null;

                return GetPaymentGatewayMethodByPaymentCode(available.ServiceCode);
            }

            LogHelper.Error<StripePaymentGatewayProvider>(string.Format("Failed to create a payment method name: {0}, description {1}, paymentCode {2}", name, description, available.ServiceCode), attempt.Exception);

            throw attempt.Exception;
        }

        public override IPaymentGatewayMethod GetPaymentGatewayMethodByKey(Guid paymentMethodKey)
        {
            var paymentMethod = this.PaymentMethods.FirstOrDefault(x => x.Key == paymentMethodKey);

            if (paymentMethod != null)
            {
                return GetPaymentGatewayMethodByPaymentCode(paymentMethod.PaymentCode);
            }

            var error = new NullReferenceException("Failed to find BraintreePaymentGatewayMethod with key specified");
            LogHelper.Error<StripePaymentGatewayProvider>("Failed to find BraintreePaymentGatewayMethod with key specified", error);
            throw error;
        }

        public override IPaymentGatewayMethod GetPaymentGatewayMethodByPaymentCode(string paymentCode)
        {
            var paymentMethod = this.PaymentMethods.FirstOrDefault(x => x.PaymentCode == paymentCode);

            if (paymentMethod != null)
            {
                switch (paymentMethod.PaymentCode)
                {
                    default:
                        return new StripePaymentGatewayMethod(this.GatewayProviderService, paymentMethod, this.GetStripeApiService());
                }
            }

            var error = new NullReferenceException("Failed to find StripePaymentGatewayMethod with key specified");
            LogHelper.Error<StripePaymentGatewayProvider>("Failed to find StripePaymentGatewayMethod with key specified", error);
            throw error;
        }

        /// <summary>
        /// The get braintree api service.
        /// </summary>
        /// <returns>
        /// The <see cref="IStripeApiService"/>.
        /// </returns>
        private IStripeApiService GetStripeApiService()
        {
            return new StripeApiService(this.GatewayProviderSettings.ExtendedData.GetStripeProviderSettings());
        }

        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            return AvailableResources.Where(x => this.PaymentMethods.All(y => y.PaymentCode != x.ServiceCode));
        }
    }
}