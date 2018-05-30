using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Merchello.Providers.Models;

namespace Merchello.Plugin.Payments.Stripe.Models
{
    public class StripeProviderSettings : IPaymentProviderSettings
    {
        public StripeProviderSettings()
        {
            this.DeleteInvoiceOnCancel = true;
            this.DefaultTransationOption = TransactionOption.AuthorizeAndCapture;
        }

        // From provider settings dialog data
        [DisplayName("Publishable Key")]
        public string PublishableKey { get; set; }

        [DisplayName("Secret Key")]
        public string SecretKey { get; set; }

        [DisplayName("Success Redirect URL")]
        public string SuccessRedirectUrl { get; set; }

        public bool DeleteInvoiceOnCancel { get; set; }

        public TransactionOption DefaultTransationOption { get; set; }
    }
}
