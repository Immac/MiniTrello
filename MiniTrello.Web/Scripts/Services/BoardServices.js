﻿'use strict';

angular.module('app.services').factory('BoardServices', ['$http', '$window', function ($http, $window) {

    var board = {};

    var baseRemoteUrl = "http://mcminitrelloapi.apphb.com";
    var baseLocalUrl = "http://localhost:1416";
    var baseUrl = baseRemoteUrl;

    board.getBoardsForLoggedUser = function () {
        console.log(baseUrl + '/boards/' + $window.sessionStorage.token);
        return $http.get(baseUrl + '/boards/' + $window.sessionStorage.token);
    };

    board.createBoard = function (model) {
        return $http.post(baseUrl + '/boards/create/' + $window.sessionStorage.token,model);
    };

    board.deleteBoard = function (model) {
        console.log(model);
        return $http.delete(baseUrl + '/boards/delete/' + $window.sessionStorage.token, model);
    };
    board.getBoard = function (id) {
        return $http.get(baseUrl + '/boards/' + id + '/' + $window.sessionStorage.token);
    };
    board.createLane = function (model) {
        console.log(model);
        return $http.post(baseUrl + '/boards/createlane/' + $window.sessionStorage.token, model);
    };
    board.deleteLane = function (model) {
        console.log(model);
        return $http.delete(baseUrl + '/boards/deletelane/' + $window.sessionStorage.token, model);
    };
    board.createCard = function (model) {
        console.log(model);
        return $http.post(baseUrl + '/boards/createcard/' + $window.sessionStorage.token, model);
    };
    board.deleteCard = function (model) {
        console.log(model);
        return $http.delete(baseUrl + '/boards/deletecard/' + $window.sessionStorage.token, model);
    };
    return board;

}]);