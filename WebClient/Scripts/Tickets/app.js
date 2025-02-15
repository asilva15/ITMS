//para hacer uso de $resource debemos colocarlo al crear el modulo
var app = angular.module("app", ["ngResource", "ngSanitize"]);

//con dataResource inyectamos la factoría
app.controller("appController", function ($scope, $http) {
    $scope.init = function (id) {
        //$http.get('../../Ticket/Unique/' + id)
        $http.get('../Ticket/Unique?idt=' + id)
        .success(function (data) {
            $scope.datos = data.Data;
            $scope.detalle = data.Details;
        });
    };
})