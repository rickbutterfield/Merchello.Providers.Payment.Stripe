using Merchello.Core.Models;
using Merchello.Plugin.Payments.Stripe.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Merchello.Providers.Payment.Stripe.Models
{
    public static class ProviderSettingExtensions
    {
        public static StripeProviderSettings GetStripeProviderSettings(this ExtendedDataCollection extendedData)
        {
            StripeProviderSettings settings;
            if (extendedData.ContainsKey(Constants.Stripe.ExtendedDataKeys.ProviderSettings))
            {
                var json = extendedData.GetValue(Constants.Stripe.ExtendedDataKeys.ProviderSettings);
                settings = JsonConvert.DeserializeObject<StripeProviderSettings>(json);
            }
            else
            {
                settings = new StripeProviderSettings();
            }

            return settings;
        }
    }
}