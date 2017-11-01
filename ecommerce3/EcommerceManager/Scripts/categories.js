angular.module('prodApp', [])
   .controller('productController', function($scope, $http) {
	   $scope.websites = [];


	   //load website and category lists
	   $http({
		   url: Url.baseUrl() + '/categories/getwebsites',
		   method: "GET"
	   })
		   .then(function successCallback(response) {
			   $scope.websites = response.data;
		   }, function errorCallback(response) {
			   console.log('AJAX ERROR!');
			   console.log(response);
		   });

   });