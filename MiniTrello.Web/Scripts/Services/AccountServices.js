'use strict';

angular.module('app.services',[]).factory('AccountServices', ['$http','$window', function ($http,$window) {

    var account = {};

    var baseRemoteUrl = "http://mcminitrelloapi.apphb.com/";
    var baseLocalUrl = "http://localhost:1416";
    var baseUrl = baseLocalUrl;

    account.login = function (model) {
        console.log(model);
        return $http.post(baseUrl + '/login', model);
    };

    account.register = function (model) {
        console.log(model);
        return $http.post(baseUrl + '/register', model);
    };

    account.EditProfile = function(model) {
        console.log("editProfile:" + model);
        console.log(baseUrl + '/profile/edit/' + $window.sessionStorage.token);
        return $http.put(baseUrl + '/profile/edit/' + $window.sessionStorage.token, model);
    };
    account.PasswordRestoreSend = function (model) {
        console.log("PasswordRestoreSend:" + model);
        console.log(baseUrl + '/restorepassword');
        return $http.post(baseUrl + '/restorepassword', model);
    };

    return account;

}]);