using Merchello.Core.Models;
using Merchello.Plugin.Payments.Stripe.Models;
using Newtonsoft.Json;

namespace Merchello.Providers.Payment.Stripe
{
    /// <summary>
    /// Extended data utiltity extensions
    /// </summary>
    public static class MappingExtensions
    {
        /// <summary>
        /// Saves the processor settings to an extended data collection
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <param name="providerSettings">The <see cref="StripeProviderSettings"/> to be serialized and saved</param>
        public static void SaveProcessorSettings(this ExtendedDataCollection extendedData, StripeProviderSettings providerSettings)
        {
            var settingsJson = JsonConvert.SerializeObject(providerSettings);

            extendedData.SetValue(Constants.Stripe.ExtendedDataKeys.ProviderSettings, settingsJson);
        }

        /// <summary>
        /// Get the processor settings from the extended data collection
        /// </summary>
        /// <param name="extendedData">The <see cref="ExtendedDataCollection"/></param>
        /// <returns>The deserialized <see cref="StripeProviderSettings"/></returns>
        public static StripeProviderSettings GetProcessorSettings(this ExtendedDataCollection extendedData)
        {
            if (!extendedData.ContainsKey(Constants.Stripe.ExtendedDataKeys.ProviderSettings)) return new StripeProviderSettings();

            return
                JsonConvert.DeserializeObject<StripeProviderSettings>(
                    extendedData.GetValue(Constants.Stripe.ExtendedDataKeys.ProviderSettings));
        }
    }
}
