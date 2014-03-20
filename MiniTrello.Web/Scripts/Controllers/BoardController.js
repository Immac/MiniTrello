'use strict';

// Google Analytics Collection APIs Reference:
// https://developers.google.com/analytics/devguides/collection/analyticsjs/

angular.module('app.controllers')



    // Path: /login
    .controller('BoardController', ['$scope', '$location', '$window', 'BoardServices','$stateParams', function ($scope, $location, $window, boardServices, $stateParams) {

    
        $scope.boardDetailId = $stateParams.boardId;
        $scope.BoardCreateModel = {Title: ''};
        $scope.BoardDeleteModel = { Id : '', IsArchived : true };

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
                  $scope.boards = data.Boards;
                })
              .error(function (data, status, headers, config) {
                console.log(data);
            });
            //$location.path('/');
        };

        $scope.CreateBoard = function () {
            boardServices
                .createBoard($scope.BoardCreateModel)
              .success(function (data, status, headers, config) {
                  console.log("data sent:");
                  console.log($scope.BoardCreateModel);
                  console.log("data recieved:");
                  console.log(data);
                  
                  if (data.ErrorCode != 0) {
                      $scope.hasError = true;
                      $scope.errorMessage = data.ErrorMessage;
                      alert(data.ErrorMessage);
                  } else {
                      $scope.hasError = false;
                      $scope.getBoardsForLoggedUser();
                      $scope.BoardCreateModel = '';
                  }

              })
              .error(function (data, status, headers, config) {
                  $scope.hasError = true;
                  $scope.message = 'Error: an unexpected error has occured.';
              });
            //$location.path('/');
            };

        $scope.DeleteBoard = function () {
            console.log($scope.BoardDeleteModel.Id);
            console.log("DELETEBOARD");
            boardServices.deleteBoard($scope.BoardDeleteModel)
                 .success(function (data, status, headers, config) {
                     console.log("data sent:");
                     console.log($scope.BoardDeleteModel);
                     console.log("data recieved:");
                     console.log(data);

                     if (data.ErrorCode != 0) {
                         $scope.hasError = true;
                         $scope.errorMessage = data.ErrorMessage;
                         alert(data.ErrorMessage);
                     } else {
                         $scope.hasError = false;
                         $scope.getBoardsForLoggedUser();
                         $scope.BoardCreateModel = '';
                         alert("Board was succesfully deleted.");
                     }

                 })
                 .error(function (data, status, headers, config) {
                     $scope.hasError = true;
                     $scope.message = 'Error: an unexpected error has occured.';
                 });
            //$location.path('/');
        };

    if ($scope.boardDetailId > 0){
        $scope.selectedBoard = $.grep($scope.boards, function (e) { return e.id == id; });
        console.log($scope.selectedBoard);
    }else{
        $scope.getBoardsForLoggedUser();
    }
    

       

        $scope.$on('$viewContentLoaded', function () {
            $window.ga('send', 'pageview', { 'page': $location.path(), 'title': $scope.$root.title });
        });
    }]);