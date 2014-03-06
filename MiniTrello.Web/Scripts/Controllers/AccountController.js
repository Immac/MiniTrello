'use strict';

// Google Analytics Collection APIs Reference:
// https://developers.google.com/analytics/devguides/collection/analyticsjs/

angular.module('app.controllers')

    // Path: /login
    .controller('AccountController',
    ['$scope', '$location', '$window', 'AccountServices', function ($scope, $location, $window, AccountServices)
    {
        $scope.$root.title = 'AngularJS SPA | Sign In';
        $scope.loginModel = { Email: '', Password: '' };
        $scope.registerModel = {
            Email: '', Password: '',
            FirstName: '', LastName: '',
            ConfirmPassword: ''
        };
        // TODO: Authorize a user
        $scope.login = function () {
            AccountServices
                .login($scope.loginModel)
                .success(function(data, status, headers, config) {
                    $window.sessionStorage.tokens = data.tokens;
                })
                .error(function(data, status, headersconfig) {
                    delete $window.sessionStorage.token;

                    $scope.message = 'Error: Invalide username or password';
                });
            $location.path('/');
        };

    $scope.register = function() {

        AccountServices.register($scope.registerModel)
            .success(function(data, status, headers, config) {
                console.log(data);
            })
            .error(function(data, status, headers, config) {
                console.log(data);
            });
        
        return false;
    };

        $scope.goToRegister = function() {
            $location.path('/register');
            return false;
        };
        $scope.goToLogin = function() {
            $location.path('/login');
            return false;
        };

        $scope.$on('$viewContentLoaded', function () {
            $window.ga('send', 'pageview', { 'page': $location.path(), 'title': $scope.$root.title });
        });
    }




    ]);