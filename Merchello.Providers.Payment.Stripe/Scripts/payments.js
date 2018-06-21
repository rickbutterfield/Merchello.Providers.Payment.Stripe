var stripeInit = function(publishableKey, totalCost, lineItems) {
    'use strict';

    // Create references to the main form and its submit button.

    var form = document.querySelector('#payment-form');
    var submitButton = form.querySelector('button[type="submit"]');

    // Create a Stripe client
    var stripe = Stripe(publishableKey);

    // Create an instance of Elements
    var elements = stripe.elements();

    var elementsOptions = {
        classes: {
            base: 'o-form-group o-input' //TODO: Change me to something generic
        },
        style: {
            base: {
                fontFamily: 'lato,-apple-system,BlinkMacSystemFont,Segoe UI,Roboto,Oxygen,Ubuntu,Cantarell,Fira Sans,Droid Sans,Helvetica Neue,Arial,sans-serif', //TODO: Change me to something generic
                fontWeight: 400
            }
        }
    };

    /**
     * Implement a Stripe Card Element that matches the look-and-feel of the app.
     *
     * This makes it easy to collect debit and credit card payments information.
     */

    // Create a Card Element and pass some custom styles to it.
    var card = elements.create('card', elementsOptions);

    // Mount the Card Element on the page.
    card.mount('#card-element');

    // Remove the js classes
    document.querySelectorAll('.o-ratio').classList = '';

    // Monitor change events on the Card Element to display any errors.
    card.addEventListener('change', function (_ref) {
        var error = _ref.error;

        var cardErrors = document.getElementById('card-errors');
        if (error) {
            cardErrors.textContent = error.message;
            cardErrors.classList.add('visible');
        } else {
            cardErrors.classList.remove('visible');
        }
        // Re-enable the Pay button.
        submitButton.disabled = false;
    });

    /**
     * Implement a Stripe Payment Request Button Element.
     *
     * This automatically supports the Payment Request API (already live on Chrome),
     * as well as Apple Pay on the Web on Safari.
     * When of these two options is available, this element adds a “Pay” button on top
     * of the page to let users pay in just a click (or a tap on mobile).
     */

    // Create the payment request.
    var paymentRequest = stripe.paymentRequest({
        country: 'GB',
        currency: 'gbp',
        total: {
            label: 'Total',
            amount: totalCost
        },
        displayItems: lineItems
    });

    // Create the payment request button
    var paymentRequestButton = elements.create('paymentRequestButton', {
        paymentRequest: paymentRequest
    });

    // Check if the Payment Request is available (or Apple Pay on the Web).
    paymentRequest.canMakePayment()
        .then(function (result) {
            if (result) {
                // Display the Pay button by mounting the Element in the DOM
                paymentRequestButton.mount('#payment-request-button');
            }
            else {
                document.querySelector('.payment-request').style.display = 'none';
            }
        })
        .catch(function (error) {
            console.log(error);
            document.querySelector('.payment-request').style.display = 'none';
        });

    paymentRequest.on('token', function (event) {
        document.querySelector('input[name="Token"]').value = event.token.id;
        form.submit();
    });

    form.addEventListener('submit', function (event) {
        event.preventDefault();

        submitButton.disabled = true;

        stripe.createToken(card).then(function (result) {
            if (result.error) {
                // Inform the customer that there was an error.
                var errorElement = document.querySelector('#card-errors');
                errorElement.textContent = result.error.message;
                submitButton.disabled = false;
            } else {
                // Send the token to your server.
                document.querySelector('input[name="Token"]').value = result.token.id;
                document.querySelector('#payment-form').submit();
            }
        });
    });
};

if (typeof stripeElements !== "undefined") {
    stripeInit(stripeElements.publishableKey, stripeElements.totalCost, stripeElements.lineItems);
}