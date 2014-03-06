'use strict';

angular.module('app.services',[]).factory('AccountServices', ['$http', function ($http) {

    var account = {};
    var baseRemoteUrl = "http://mcminitrelloapi.apphb.com";
    var baseLocalUrl = "http://localhost:8080";
    var baseUrl = baseRemoteUrl;

    account.login = function (data) {
        return $http.post(baseUrl + '/login', data);
    };

    account.register = function(data) {
        return $http.post(baseUrl +  '/register', data);
    };

    return account;

}]);