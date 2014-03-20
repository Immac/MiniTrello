'use strict';

angular.module('app.services').factory('BoardServices', ['$http', '$window', function ($http, $window) {

    var board = {};

    var baseRemoteUrl = "http://mcminitrelloapi.apphb.com/";
    var baseLocalUrl = "http://localhost:1416";
    var baseUrl = baseRemoteUrl;

    board.getBoardsForLoggedUser = function() {
        return $http.get(baseUrl + '/boards/' + $window.sessionStorage.token);
    };

    board.createBoard = function (model) {
        return $http.post(baseUrl + '/boards/create/' + $window.sessionStorage.token,model);
    };

    board.deleteBoard = function(model) {
        return $http.delete(baseUrl + '/boards/delete/' + $window.sessionStorage.token, model);
    };
    board.getBoard = function (id) {
        return $http.get(baseUrl + '/board/' + id + '/' + $window.sessionStorage.token);
    };
    return board;

}]);