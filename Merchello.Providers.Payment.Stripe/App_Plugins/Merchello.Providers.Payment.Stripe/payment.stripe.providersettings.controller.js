angular.module('merchello.providers.payments.stripe').controller('Merchello.Providers.Payment.Stripe.PaymentController',
    ['$scope',
    function ($scope) {

        var extendedDataKey = 'stripeProviderSettings';
        var settingsString = $scope.dialogData.provider.extendedData.getValue(extendedDataKey);
        $scope.providerSettings = angular.fromJson(settingsString);

        // Watch with object equality to convert back to a string for the submit() call on the Save button
        $scope.$watch(function () {
            return $scope.providerSettings;
        }, function (newValue, oldValue) {
            $scope.dialogData.provider.extendedData.setValue(extendedDataKey, angular.toJson(newValue));
        }, true);

    }
]);