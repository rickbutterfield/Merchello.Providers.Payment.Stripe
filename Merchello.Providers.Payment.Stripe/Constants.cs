using System;

namespace Merchello.Providers.Payment.Stripe
{
    public class Constants
    {
        /// <summary>
        /// Stripe constants
        /// </summary>
        public static class Stripe
        {
            /// <summary>
            /// The gateway provider key
            /// </summary>
            public const string GatewayProviderKey = "e1956823-3607-4516-934c-2696c4fb754b";

            /// <summary>
            /// Gets the gateway provider settings key
            /// </summary>
            public static Guid GatewayProviderSettingsKey
            {
                get
                {
                    return new Guid(GatewayProviderKey);
                }
            }

            /// <summary>
            /// Gets the transaction channel.
            /// </summary>
            public static string TransactionChannel
            {
                get
                {
                    return "MerchelloStripePaymentProvider";
                }
            }

            /// <summary>
            /// Braintree Provider PaymentCodes.
            /// </summary>
            public static class PaymentCodes
            {
                /// <summary>
                /// Gets the transaction.
                /// </summary>
                public const string Transaction = "StripeTransaction";
            }

            /// <summary>
            /// Constant ExtendedDataCollection keys
            /// </summary>
            public static class ExtendedDataKeys
            {
                /// <summary>
                /// Gets the key for the StripeProviderSettings serialization.
                /// </summary>
                public const string ProviderSettings = "stripeProviderSettings";

                public static string StripeTransaction
                {
                    get
                    {
                        return "stripeTransaction";
                    }
                }
            }

            /// <summary>
            /// Stripe processor arguments
            /// </summary>
            public static class ProcessorArguments
            {
                /// <summary>
                /// Gets the Stripe token ID
                /// </summary>
                public static string TokenId = "stripeTokenId";

                /// <summary>
                /// Gets the Stripe charge ID
                /// </summary>
                public static string ChargeId = "stripeChargeId";

                /// <summary>
                /// Gets the Stripe customer name
                /// </summary>
                public static string CustomerId = "stripeCustomerId";

                /// <summary>
                /// Gets the Stripe customer name
                /// </summary>
                public static string CustomerName = "stripeCustomerName";

                /// <summary>
                /// Gets the Stripe customer name
                /// </summary>
                public static string CustomerEmail = "stripeCustomerEmail";


                public static string AuthorizeDeclinedResult = "stripeAuthorizeDeclined";

                public static string CaptureDeclinedResult = "stripeCaptureDeclined";
                public static string CaputureTransactionCode = "stripeCaptureTransactionCode";
                public static string CaptureTransactionResult = "stripeCaptureTransactionResult";

                public static string RefundDeclinedResult = "stripeRefundDeclined";
                public static string VoidDeclinedResult = "stripeVoidDeclined";
            }
        }
    }
}