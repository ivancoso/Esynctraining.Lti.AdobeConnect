var app = angular.module('jobs', ["ngResource"]);

app.factory('commonSqlService', function ($http) {
    var results = [ "" ];

    function find(term) {
        angular.copy(["", "", ""], results);

        $http.get('/sql/build/' + term).success(function (data) {
            angular.copy(data, results);
        });
    }

    //public API
    return {
        results: results,
        find: find
    };
});

app.factory('companySqlService', function ($http) {
    var results = [""];

    function find(term) {
        angular.copy([""], results);

        $http.get('/sql/company/' + term).success(function (data) {
            angular.copy(data, results);
        });
    }

    //public API
    return {
        results: results,
        find: find
    };
});

app.controller("SearchCtrl", ['$scope', '$http', 'commonSqlService', function ($scope, $http, searchService) {
    $scope.search_term = '';
    $scope.commonSqlService = searchService;
    $scope.commonSqlService.results = searchService.results;

    $scope.doSearch = function () {
        $scope.commonSqlService.find($scope.search_term);
    };

    $scope.lmsProviders = [];

    $http({
        method: 'GET',
        url: '/lms/providers'
    }).success(function (result) {
        $scope.lmsProviders = result;
    });

    //$scope.commonSqlService.find();
}]);

app.controller("CompanyCtrl", ['$scope', '$http', 'companySqlService', function ($scope, $http, searchService) {
    $scope.search_term = '';
    $scope.companySqlService = searchService;
    $scope.companySqlService.results = searchService.results;

    $scope.doSearch = function () {
        $scope.companySqlService.find($scope.search_term);
    };

    $scope.companies = [];

    $http({
        method: 'GET',
        url: '/lms/companies'
    }).success(function (result) {
        $scope.companies = result;
    });

    //$scope.commonSqlService.find();
}]);

app.config(function ($httpProvider) {
    $httpProvider.responseInterceptors.push('myHttpInterceptor');

    var spinnerFunction = function spinnerFunction(data, headersGetter) {
        $(".spinner").show();
        return data;
    };

    $httpProvider.defaults.transformRequest.push(spinnerFunction);
});

app.factory('myHttpInterceptor', function ($q, $window) {
    return function (promise) {
        return promise.then(function (response) {
            $(".spinner").hide();
            return response;
        }, function (response) {
            $(".spinner").hide();
            return $q.reject(response);
        });
    };
});
