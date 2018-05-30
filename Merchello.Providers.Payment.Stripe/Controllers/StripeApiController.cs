using Merchello.Core;
using Merchello.Providers.Payment.Stripe.Models;
using Merchello.Providers.Payment.Stripe.Provider;
using Merchello.Providers.Payment.Stripe.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Logging;
using Umbraco.Web.WebApi;

namespace Merchello.Providers.Payment.Stripe.Controllers
{
    /// <summary>
    /// The Stripe API controller.
    /// </summary>
    public class StripeApiController : UmbracoApiController
    {
        /// <summary>
        /// The <see cref="IStripeApiService"/>.
        /// </summary>
        private readonly IStripeApiService _stripeApiService;

        /// <summary>
        /// Initializes a new instance of the <see cref="StripeApiController"/> class.
        /// </summary>
        public StripeApiController()
            : this(MerchelloContext.Current)
        {
        }

        public StripeApiController(IMerchelloContext merchelloContext)
        {
            if (merchelloContext == null) throw new ArgumentNullException("merchelloContext");
            var provider = (StripePaymentGatewayProvider)merchelloContext.Gateways.Payment.GetProviderByKey(Constants.Stripe.GatewayProviderSettingsKey);

            if (provider == null)
            {
                var ex = new NullReferenceException("The StripePaymentGatewayProvider could not be resolved.  The provider must be activiated");
                LogHelper.Error<StripeApiController>("StripePaymentGatewayProvider not activated.", ex);
                throw ex;
            }

            var settings = provider.ExtendedData.GetStripeProviderSettings();

            this._stripeApiService = new StripeApiService(merchelloContext, settings);
        }
    }
}