'use strict';

angular.module('app.services').factory('BoardServices', ['$http','$window', function ($http, $window) {

    var board = {};

    var baseRemoteUrl = "http://minitrelloapi.apphb.com";
    var baseLocalUrl = "http://localhost:1416";
    var baseUrl = baseRemoteUrl;

    board.getBoardsForLoggedUser = function() {
        return $http.get(baseUrl + '/boards/' + $window.sessionStorage.token);
    };

    return board;

}]);