# Merchello.Providers.Payment.Stripe
Stripe Payment Provider for Merchello

## About this package
This Merchello plugin is a payment provider for Stripe (https://stripe.com/). You need a Stripe account and API Key.

## Installation
You should be able to download this, add it to your solution, and reference it from within your existing project. Following that, take the following steps:
- Copy `/Views/StripePayment/PaymentForm.cshtml` into your main project (or use it as a reference file)
- Within your payment template page, call `@Html.Action("PaymentForm", "StripePayment", new { area = "Merchello" })`
