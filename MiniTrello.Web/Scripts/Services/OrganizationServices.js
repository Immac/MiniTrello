'use strict';

angular.module('app.services').factory('OrganizationServices', ['$http', '$window', function ($http, $window) {

    var organization = {};

    var baseRemoteUrl = "http://mcminitrelloapi.apphb.com";
    var baseLocalUrl = "http://localhost:1416";
    var baseUrl = baseRemoteUrl;

    organization.getOrganizationsForLoggedUser = function () {
        console.log(baseUrl + '/organizations/' + $window.sessionStorage.token);
        return $http.get(baseUrl + '/organizations/' + $window.sessionStorage.token);
    };

    organization.createOrganization = function(model) {
        console.log(baseUrl + '/organizations/create/' + $window.sessionStorage.token);
        console.log("model:" + model);
        return $http.post(baseUrl + '/organizations/create/' + $window.sessionStorage.token, model);
    };

    return organization;

}]);

