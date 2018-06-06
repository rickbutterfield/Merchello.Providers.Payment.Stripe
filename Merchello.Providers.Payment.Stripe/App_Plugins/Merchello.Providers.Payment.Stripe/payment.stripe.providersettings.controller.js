angular.module('merchello.providers.payments.stripe').controller('Merchello.Providers.Payment.Stripe.PaymentController',
    ['$scope',
        function ($scope, stripeProviderSettings) {

            $scope.providerSettings = {};
            function init() {
                var json = JSON.parse($scope.dialogData.provider.extendedData.getValue('stripeProviderSettings'));
                $scope.providerSettings = stripeProviderSettings.transform(json);

                // Watch with object equality to convert back to a string for the submit() call on the Save button
                $scope.$watch(function () {
                    return $scope.providerSettings;
                }, function (newValue, oldValue) {
                    $scope.dialogData.provider.extendedData.setValue('stripeProviderSettings', angular.toJson(newValue));
                }, true);
            }


        }
    ]);