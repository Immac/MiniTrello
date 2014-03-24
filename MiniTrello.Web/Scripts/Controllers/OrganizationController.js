'use strict';

// Google Analytics Collection APIs Reference:
// https://developers.google.com/analytics/devguides/collection/analyticsjs/

angular.module('app.controllers')



    // Path: /login
    .controller('OrganizationController', ['$scope', '$location', '$window', 'OrganizationServices', '$stateParams', function ($scope, $location, $window, organizationServices, $stateParams) {


        $scope.organizationDetailId = $stateParams.organizationId;

        //console.log($location.search().boardId);

        console.log($scope.organizationDetailId);

        $scope.organizations = [];

        $scope.getOrganizationsForLoggedUser = function () {

            organizationServices
                .getOrganizationsForLoggedUser()
              .success(function (data, status, headers, config) {
                  $scope.organizations = data;
                  console.log("Organizations returned data:");
                  console.log(data);
                })
              .error(function (data, status, headers, config) {
                  console.log(data);
              });
        };

    $scope.CreateOrganization = function() {
            
        organizationServices
                .createOrganization($scope.OrganizationCreateModel)
              .success(function (data, status, headers, config) {
                  $scope.organizations = data;
                  console.log("Organizations returned data:");
                  console.log(data);
                $scope.getOrganizationsForLoggedUser();
            })
              .error(function (data, status, headers, config) {
                  console.log(data);
              });
    };
        

        //if ($scope.boardDetailId > 0) {
        //    //get board details
        //}
        //else {
            $scope.getOrganizationsForLoggedUser();
       // }




        $scope.$on('$viewContentLoaded', function () {
            $window.ga('send', 'pageview', { 'page': $location.path(), 'title': $scope.$root.title });
        });
    }]);