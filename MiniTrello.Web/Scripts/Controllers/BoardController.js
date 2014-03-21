'use strict';

// Google Analytics Collection APIs Reference:
// https://developers.google.com/analytics/devguides/collection/analyticsjs/

angular.module('app.controllers')



    // Path: /login
    .controller('BoardController', ['$scope', '$location', '$window', 'BoardServices','$stateParams', function ($scope, $location, $window, boardServices, $stateParams) {

    
        $scope.boardDetailId = $stateParams.boardId;
        $scope.BoardCreateModel = {Title: ''};
        $scope.BoardDeleteModel = { Id : 0, IsArchived : true };
        $scope.LaneCreateModel = { BoardId: 0, Name: '' };
        //$scope.LaneDeleteModel = {LaneID,IsArchived: true};
        console.log($scope.boardDetailId);
    
        $scope.boards = [];
        $scope.BoardDetailModel = [];

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

        $scope.GetBoard = function() {
            boardServices.getBoard($stateParams.boardId)
            .success(function (data, status, headers, config) {
                    console.log("GetBoard: ");
                    console.log(data);
                    $scope.BoardDetailModel = data;
                    console.log("Board Detail Model:");
                    console.log($scope.BoardDetailModel);
                if (data.errorCode != 0) {
                    $scope.hasError = true;
                    $scope.errorMessage = data.ErrorMessage;
                } else {
                    $scope.BoardDetailModel = data;
                    console.log("Board Detail Model:");
                    console.log($scope.BoardDetailModel);
                };
            })
              .error(function (data, status, headers, config) {
                  console.log(data);
              });
            //$location.path('/');
        };

        $scope.CreateLane = function () {
            $scope.LaneCreateModel.BoardId = $stateParams.boardId;
            boardServices.createLane($scope.LaneCreateModel)
              .success(function (data, status, headers, config) {
                  console.log("data sent:");
                  console.log($scope.LaneCreateModel);
                  console.log("data recieved:");
                  console.log(data);
                  
                  if (data.ErrorCode != 0) {
                      $scope.hasError = true;
                      $scope.errorMessage = data.ErrorMessage;
                      alert(data.ErrorMessage);
                  } else {
                      $scope.hasError = false;
                      $scope.getBoardsForLoggedUser();
                      $scope.LaneCreateModel = '';
                      $scope.GetBoard();
                  }

              })
              .error(function (data, status, headers, config) {
                  $scope.hasError = true;
                  $scope.message = 'Error: an unexpected error has occured.';
              });
            //$location.path('/');
            };
        $scope.CreateCard = function () {
           
            boardServices.createCard($scope.CardCreateModel)
              .success(function (data, status, headers, config) {
                  console.log("data sent:");
                  console.log($scope.CardCreateModel);
                  console.log("data recieved:");
                  console.log(data);

                  if (data.ErrorCode != 0) {
                      $scope.hasError = true;
                      $scope.errorMessage = data.ErrorMessage;
                      alert(data.ErrorMessage);
                  } else {
                      $scope.hasError = false;
                      $scope.getBoardsForLoggedUser();
                      $scope.CardCreateModel = '';
                      $scope.GetBoard();
                  }

              })
              .error(function (data, status, headers, config) {
                  $scope.hasError = true;
                  $scope.message = 'Error: an unexpected error has occured.';
              });
            //$location.path('/');
        };

        $scope.DeleteLane = function () {
            $scope.LaneDeleteModel.IsArchived = true;
            boardServices.deleteLane($scope.LaneDeleteModel)
              .success(function (data, status, headers, config) {
                  console.log("data sent:");
                  console.log($scope.LaneDeleteModel);
                  console.log("data recieved:");
                  console.log(data);

                  if (data.ErrorCode != 0) {
                      $scope.hasError = true;
                      $scope.errorMessage = data.ErrorMessage;
                      alert(data.ErrorMessage);
                  } else {
                      $scope.hasError = false;
                      $scope.getBoardsForLoggedUser();
                      $scope.LaneDeleteModel = '';
                      $scope.GetBoard();
                  }

              })
              .error(function (data, status, headers, config) {
                  $scope.hasError = true;
                  $scope.message = 'Error: an unexpected error has occured.';
              });
            //$location.path('/');
        };

        $scope.AddMember = function () {
            $scope.MemberAddModel.BoardID = $stateParams.boardId;
            boardServices.addMember($scope.MemberAddModel)
              .success(function (data, status, headers, config) {
                  console.log("data sent:");
                  console.log($scope.MemberAddModel);
                  console.log("data recieved:");
                  console.log(data);

                  if (data.ErrorCode != 0) {
                      $scope.hasError = true;
                      $scope.errorMessage = data.ErrorMessage;
                      alert(data.ErrorMessage);
                  } else {
                      $scope.hasError = false;
                      $scope.getBoardsForLoggedUser();
                      $scope.MemberAddModel = '';
                      $scope.GetBoard();
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
        console.log("selected board:");
        console.log($stateParams.boardId);
        $scope.GetBoard();
    }else{
        $scope.getBoardsForLoggedUser();
    }
    

       

        $scope.$on('$viewContentLoaded', function () {
            $window.ga('send', 'pageview', { 'page': $location.path(), 'title': $scope.$root.title });
        });
    }]);