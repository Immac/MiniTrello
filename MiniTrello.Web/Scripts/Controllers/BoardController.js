'use strict';

// Google Analytics Collection APIs Reference:
// https://developers.google.com/analytics/devguides/collection/analyticsjs/

angular.module('app.controllers')



    // Path: /login
    .controller('BoardController', ['$scope', '$location', '$window', 'BoardServices','$stateParams', function ($scope, $location, $window, boardServices, $stateParams) {


       $scope.boardDetailId = $stateParams.boardId;

        //console.log($location.search().boardId);

        console.log($scope.boardDetailId);

        $scope.boards = [];
        $scope.getBoardsForLoggedUser = function () {

            boardServices
                .getBoardsForLoggedUser()
              .success(function (data, status, headers, config) {
                  console.log(data);
                  if (data.errorCode != 0) {
                      $scope.hasError = true;
                      $scope.errorMessage = data.ErrorMessage;
                  }
                  $scope.boards = data;
                })
              .error(function (data, status, headers, config) {
                console.log(data);
            });
            //$location.path('/');
        };

    if ($scope.boardDetailId > 0)
    {
        //get board details
    }
    else
    {
        $scope.getBoardsForLoggedUser();
    }
    

       

        $scope.$on('$viewContentLoaded', function () {
            $window.ga('send', 'pageview', { 'page': $location.path(), 'title': $scope.$root.title });
        });
    }]);