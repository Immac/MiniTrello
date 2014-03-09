'use strict';

// Google Analytics Collection APIs Reference:
// https://developers.google.com/analytics/devguides/collection/analyticsjs/

angular.module('app.controllers')

  

    // Path: /login
    .controller('AccountController', ['$scope', '$location', '$window', 'AccountServices', function ($scope, $location, $window, AccountServices) {

        $scope.hasError = false;
    $scope.errorMessage = '';

        $scope.isLogged = function() {
            return $window.sessionStorage.token != null;
        };
        
        $scope.loginModel = { Email: '', Password: '' };

    $scope.registerModel = { Email: '', Password: '', FirstName: '', LastName: '', ConfirmPassword: '' };
        
        // TODO: Authorize a user
        $scope.login = function () {

            AccountServices
                .login($scope.loginModel)
              .success(function (data, status, headers, config) {
                  
                  $window.sessionStorage.token = data.Token;
                  $location.path('/boards');
              })
              .error(function (data, status, headers, config) {
                // Erase the token if the user fails to log in
                delete $window.sessionStorage.token;

                $scope.errorMessage = 'Error o clave incorrect';
                $scope.hasError = true;
                // Handle login errors here
                $scope.message = 'Error: Invalid user or password';
            });
            //$location.path('/');
        };

    $scope.goToRegister = function() {
        $location.path('/register');
    };
    $scope.goToLogin = function() {
        $location.path('/login');
    };

    $scope.register = function() {
        AccountServices
            .register($scope.registerModel)
            .success(function (data, status, headers, config) {
                console.log(data);
                $scope.goToLogin();
            })
            .error(function (data, status, headers, config) {
                console.log(data);
            });
    };

        $scope.$on('$viewContentLoaded', function () {
            $window.ga('send', 'pageview', { 'page': $location.path(), 'title': $scope.$root.title });
        });
    }]);