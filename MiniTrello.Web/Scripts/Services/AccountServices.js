'use strict';

angular.module('app.services',[]).factory('AccountServices', ['$http', function ($http) {

    var account = {};

    var baseRemoteUrl = "http://mcminitrelloapi.apphb.com/";
    var baseLocalUrl = "http://localhost:1416";
    var baseUrl = baseLocalUrl;

    account.login = function (model) {
        return $http.post(baseUrl + '/login', model);
    };

    account.register = function (model) {
        return $http.post(baseUrl + '/register', model);
    };


    return account;

}]);