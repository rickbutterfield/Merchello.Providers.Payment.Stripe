using Merchello.Core;
using Merchello.Plugin.Payments.Stripe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;

namespace Merchello.Providers.Payment.Stripe.Services
{
    /// <summary>
    /// Represents a <see cref="StripeApiService"/>.
    /// </summary>
    public class StripeApiService : IStripeApiService
    {
        /// <summary>
        /// The <see cref="StripeProviderSettings"/>.
        /// </summary>
        private readonly StripeProviderSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="StripeApiService"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public StripeApiService(StripeProviderSettings settings)
            : this(MerchelloContext.Current, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StripeApiService"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The Merchello context.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <remarks>
        /// Used for testing
        /// </remarks>
        internal StripeApiService(IMerchelloContext merchelloContext, StripeProviderSettings settings)
        {
            Mandate.ParameterNotNull(merchelloContext, "merchelloContext");
            Mandate.ParameterNotNull(settings, "settings");

            this._settings = settings;

            this.Initialize(merchelloContext);
        }

        /// <summary>
        /// Gets the <see cref="StripeProviderSettings"/>.
        /// </summary>
        public StripeProviderSettings StripeProviderSettings
        {
            get
            {
                return this._settings;
            }
        }

        /// <summary>
        /// Initializes the provider
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        private void Initialize(IMerchelloContext merchelloContext)
        {
        }
    }
}