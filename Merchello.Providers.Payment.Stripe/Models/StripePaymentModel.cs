using Merchello.FastTrack.Models;
using Merchello.Web.Store.Models;
using System.ComponentModel.DataAnnotations;

namespace Merchello.Providers.Payment.Stripe.Models
{
    /// <summary>
    /// A model for rendering and processing basic Stripe Payments.
    /// </summary>
    public class StripePaymentModel : StorePaymentModel, ISuccessRedirectUrl
    {
        public StripePaymentModel()
        {
            this.RequireJs = true;
        }

        /// <summary>
        /// Gets or sets the server token.
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Gets or sets the Stripe API publishable key.
        /// </summary>
        public string PublishableKey { get; set; }

        /// <summary>
        /// Gets or sets the customer's name.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the customer's email address.
        /// </summary>
        //[Required]
        //[EmailAddress]
        //public string Email { get; set; }

        /// <summary>
        /// Gets or sets the success redirect url.
        /// </summary>
        public string SuccessRedirectUrl { get; set; }
    }
}