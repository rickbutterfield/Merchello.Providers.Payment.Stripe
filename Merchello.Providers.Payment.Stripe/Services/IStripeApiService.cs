using Merchello.Plugin.Payments.Stripe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Services;

namespace Merchello.Providers.Payment.Stripe.Services
{
    public interface IStripeApiService : IService
    {
        StripeProviderSettings StripeProviderSettings { get; }
    }
}