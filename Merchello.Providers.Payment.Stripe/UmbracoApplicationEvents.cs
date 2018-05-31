using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Events;
using Merchello.Core.Services;
using Merchello.Core.Observation;
using Merchello.Core.Gateways.Payment;
using Merchello.Plugin.Payments.Stripe.Models;
using Merchello.Providers.Payment.Stripe.Controllers;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;

namespace Merchello.Providers.Payment.Stripe
{
    public class StripeApplicationEventListener : ApplicationEventHandler
    {
        /// <summary>
        /// Handles the Umbraco Application Started event.
        /// </summary>
        /// <param name="umbracoApplication">
        /// The umbraco application.
        /// </param>
        /// <param name="applicationContext">
        /// The application context.
        /// </param>
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            base.ApplicationStarted(umbracoApplication, applicationContext);

            //LogHelper.Info<StripeApplicationEventListener>("Initializing Stripe Payment provider registration binding events");

            //GatewayProviderService.Saving += GatewayProviderServiceOnSaving;

            //StripePaymentController.Processed += StripeControllerProcessed;
        }

        /// <summary>
        /// The gateway provider service on saving.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The save event args.
        /// </param>
        private static void GatewayProviderServiceOnSaving(IGatewayProviderService sender, SaveEventArgs<IGatewayProviderSettings> e)
        {
            //var key = Constants.Stripe.GatewayProviderSettingsKey;
            //var provider = e.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);
            //if (provider == null) return;

            //MappingExtensions.SaveProcessorSettings(provider.ExtendedData, new StripeProviderSettings());
        }

        /// <summary>
        /// Handles the StripePaymentController processed event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void StripeControllerProcessed(StripePaymentController sender, PaymentAttemptEventArgs<IPaymentResult> e)
        {
        //    var attempt = e.Entity;
        //    if (attempt.Payment.Success)
        //    {
        //        var email = attempt.Invoice.BillToEmail;

        //        LogHelper.Info<UmbracoEventHandler>(string.Format("Raising notification trigger for order no. {0}", attempt.Invoice.BillToEmail));

        //        Notification.Trigger("OrderConfirmation", attempt, new[] { attempt.Invoice.BillToEmail }, Topic.Notifications);
        //    }
        }
    }
}
