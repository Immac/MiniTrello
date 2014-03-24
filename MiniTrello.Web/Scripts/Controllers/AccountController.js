'use strict';

// Google Analytics Collection APIs Reference:
// https://developers.google.com/analytics/devguides/collection/analyticsjs/

angular.module('app.controllers')

  

    // Path: /login
    .controller('AccountController', ['$scope', '$location', '$window', 'AccountServices', function ($scope, $location, $window, AccountServices) {

        $scope.hasError = false;
        $scope.errorMessage = '';
        

        $scope.isLogged = function () {
            return $window.sessionStorage.token != null;
        };
        
        $scope.loginModel = { Email: '', Password: '' };

    $scope.registerModel = { Email: '', Password: '', FirstName: '', LastName: '', ConfirmPassword: '' };
    $scope.errorMessage = '';
        $scope.login = function () {

            AccountServices
                .login($scope.loginModel)
              .success(function (data, status, headers, config) {
                    console.log("Login data:");
                    console.log(data);
                    console.log("Login data");
                  if (data.ErrorCode != 0) {
                      $scope.hasError = true;
                      $scope.errorMessage = data.ErrorMessage;
                      alert(data.ErrorMessage);
                      delete $window.sessionStorage.token;
                  } else {
                      $scope.hasError = false;
                      $window.sessionStorage.token = data.Token;
                      
                      console.log($scope.accountName);
                      $location.path('/boards');
                  }
                  
              })
              .error(function (data, status, headers, config) {
                $scope.hasError = true;
                $scope.message = 'Error: An unexpected error has occured, pleas contact the system administrator';
            });
            //$location.path('/');
        };

        

    $scope.goToRegister = function() {
        $location.path('/register');
    };
    $scope.goToLogin = function() {
        $location.path('/login');
    };
    $scope.goToBoards = function () {
        $location.path('/boards');
    };

    $scope.register = function() {
        AccountServices
            .register($scope.registerModel)
            .success(function (data, status, headers, config) {
                console.log(data);
                if (data.ErrorCode != 0) {
                    $scope.hasError = true;
                    $scope.errorMessage = data.ErrorMessage;
                } else {
                    $scope.goToLogin();
                }
            })
            .error(function (data, status, headers, config) {
                console.log(data);
                $scope.hasError = true;
                $scope.errorMessage = 'An unexpected ERROR has occured.';
            });
    };

    $scope.editProfile = function () {
        AccountServices
            .EditProfile($scope.profileEditModel)
            .success(function (data, status, headers, config) {
                console.log("editeProfile: ");
                console.log(data);
                if (data.ErrorCode != 0) {
                    $scope.hasError = true;
                    $scope.errorMessage = data.ErrorMessage;
                } else {
                    $scope.goToBoards();
                }
            })
            .error(function (data, status, headers, config) {
                console.log(data);
                $scope.hasError = true;
                $scope.errorMessage = 'An unexpected ERROR has occured.';
            });
        

    };


    console.log($scope.accountName);
        $scope.$on('$viewContentLoaded', function () {
            $window.ga('send', 'pageview', { 'page': $location.path(), 'title': $scope.$root.title });
        });
    }]);