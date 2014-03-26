'use strict';

angular.module('app.services').factory('BoardServices', ['$http', '$window', function ($http, $window) {

    var board = {};

    var baseRemoteUrl = "http://mcminitrelloapi.apphb.com";
    var baseLocalUrl = "http://localhost:1416";
    var baseUrl = baseLocalUrl;

    board.getBoardsForLoggedUser = function () {
        console.log(baseUrl + '/boards/' + $window.sessionStorage.token);
        return $http.get(baseUrl + '/boards/' + $window.sessionStorage.token);
    };

    board.createBoard = function (model) {
        return $http.post(baseUrl + '/boards/create/' + $window.sessionStorage.token,model);
    };

    board.deleteBoard = function (model) {
        console.log(model);
        return $http.delete(baseUrl + '/boards/'+model.Id+'/' + $window.sessionStorage.token);
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
        console.log(baseUrl + '/boards/deletelane/' + model.LaneId + '/' + $window.sessionStorage.token);
        return $http.delete(baseUrl + '/boards/deletelane/' + model.LaneId + '/' + $window.sessionStorage.token);

    };
    board.createCard = function (model) {
        console.log(model);
        return $http.post(baseUrl + '/boards/createcard/' + $window.sessionStorage.token, model);
    };
    board.deleteCard = function (model) {
        console.log(model);
        return $http.delete(baseUrl + '/boards/deletecard/'+model.Id+'/' + $window.sessionStorage.token);
    };
    board.addMember = function (model) {
        console.log(model);
        return $http.put(baseUrl + '/boards/addmember/' + $window.sessionStorage.token, model);
    };
    return board;

}]);