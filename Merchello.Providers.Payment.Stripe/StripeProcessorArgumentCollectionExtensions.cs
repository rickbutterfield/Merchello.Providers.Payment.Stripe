using Merchello.Core.Gateways.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Logging;

namespace Merchello.Providers.Payment.Stripe
{
    public static class StripeProcessorArgumentCollectionExtensions
    {
        /// <summary>
        /// Set Stripe token ID
        /// </summary>
        /// <param name="args"></param>
        /// <param name="chargeId"></param>
        public static void SetTokenId(this ProcessorArgumentCollection args, string chargeId)
        {
            args.Add(Constants.Stripe.ProcessorArguments.TokenId, chargeId);
        }

        /// <summary>
        /// Get Stripe token ID
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetTokenId(this ProcessorArgumentCollection args)
        {
            if (args.ContainsKey(Constants.Stripe.ProcessorArguments.TokenId)) return args[Constants.Stripe.ProcessorArguments.TokenId];

            LogHelper.Debug(typeof(MappingExtensions), "Payment Method Token Id not found in process argument collection");

            return string.Empty;
        }


        /// <summary>
        /// Set Stripe charge ID
        /// </summary>
        /// <param name="args"></param>
        /// <param name="chargeId"></param>
        public static void SetChargeId(this ProcessorArgumentCollection args, string chargeId)
        {
            args.Add(Constants.Stripe.ProcessorArguments.ChargeId, chargeId);
        }

        /// <summary>
        /// Get Stripe charge ID
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetChargeId(this ProcessorArgumentCollection args)
        {
            if (args.ContainsKey(Constants.Stripe.ProcessorArguments.ChargeId)) return args[Constants.Stripe.ProcessorArguments.ChargeId];

            LogHelper.Debug(typeof(MappingExtensions), "Payment Method Charge Id not found in process argument collection");

            return string.Empty;
        }


        /// <summary>
        /// Set Stripe customer name
        /// </summary>
        /// <param name="args"></param>
        /// <param name="customerName"></param>
        public static void SetCustomerName(this ProcessorArgumentCollection args, string customerName)
        {
            args.Add(Constants.Stripe.ProcessorArguments.CustomerName, customerName);
        }

        /// <summary>
        /// Get Stripe customer name
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetCustomerName(this ProcessorArgumentCollection args)
        {
            if (args.ContainsKey(Constants.Stripe.ProcessorArguments.CustomerName)) return args[Constants.Stripe.ProcessorArguments.CustomerName];

            LogHelper.Debug(typeof(MappingExtensions), "Payment Method Customer Name not found in process argument collection");

            return string.Empty;
        }


        /// <summary>
        /// Set Stripe customer name
        /// </summary>
        /// <param name="args"></param>
        /// <param name="customerEmail"></param>
        public static void SetCustomerEmail(this ProcessorArgumentCollection args, string customerEmail)
        {
            args.Add(Constants.Stripe.ProcessorArguments.CustomerEmail, customerEmail);
        }

        /// <summary>
        /// Get Stripe customer name
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GetCustomerEmail(this ProcessorArgumentCollection args)
        {
            if (args.ContainsKey(Constants.Stripe.ProcessorArguments.CustomerEmail)) return args[Constants.Stripe.ProcessorArguments.CustomerEmail];

            LogHelper.Debug(typeof(MappingExtensions), "Payment Method Customer Email not found in process argument collection");

            return string.Empty;
        }
    }
}