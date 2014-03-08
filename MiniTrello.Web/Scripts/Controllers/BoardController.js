'use strict';

// Google Analytics Collection APIs Reference:
// https://developers.google.com/analytics/devguides/collection/analyticsjs/

angular.module('app.controllers')



    // Path: /login
    .controller('BoardController', ['$scope', '$location', '$window', 'BoardServices', function ($scope, $location, $window, boardServices) {

        $scope.boards = [];

        var board = { Name: 'Myboard1', Description: 'Description1' };
        var board1 = { Name: 'Myboard2', Description: 'Description2' };
        $scope.boards.push(board);
    $scope.boards.push(board1);
        

        // TODO: Authorize a user
        $scope.getBoardsForLoggedUser = function () {

            boardServices
                .getBoardsForLoggedUser()
              .success(function (data, status, headers, config) {
                    $scope.boards = data;
                })
              .error(function (data, status, headers, config) {

              });
            //$location.path('/');
        };

       

        $scope.$on('$viewContentLoaded', function () {
            $window.ga('send', 'pageview', { 'page': $location.path(), 'title': $scope.$root.title });
        });
    }]);