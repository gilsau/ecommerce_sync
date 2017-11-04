angular.module('prodApp', [])
   .controller('productController', function($scope, $http) {
	   $scope.websites = [];


	   //load websites
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

	   //load parent categories
	   $scope.GetParentCats = function (isForTgt) {
		   var webId = $scope.selWeb.Id;

		   console.log('$scope.selWebId: ' + $scope.selWeb.Id);
		   console.log('webId: ' + webId);

		   if (isForTgt) {
			   webId = $scope.selWebTgt.Id;
			}

		   $http({
			   url: Url.baseUrl() + '/categories/getparentcats?webId='+webId,
			   method: "GET"
		   })
			   .then(function successCallback(response) {
				   if (isForTgt) {
					   $scope.parentCatsTgt = response.data;
				   }
				   else {
					   $scope.parentCats = response.data;
				   }
			   }, function errorCallback(response) {
				   console.log('AJAX ERROR!');
				   console.log(response);
			   });
	   }

	   //load child categories
	   $scope.GetChildCats = function (isForTgt) {
		   var webId = $scope.selWeb.Id;
		   var parentCatId = $scope.selParentCat.Id;

		   if (isForTgt) {
			   webId = $scope.selWebTgt.Id;
			   parentCatId = $scope.selParentCatTgt.Id;
		   }

		   //copy parent cat selected name to text box, to edit
		   $scope.txtParentCat = $scope.selParentCat.Name;

		   $http({
			   url: Url.baseUrl() + '/categories/getchildcats?webId=' + webId + '&parentCatId=' + parentCatId,
			   method: "GET"
		   })
			   .then(function successCallback(response) {
				   if (isForTgt) {
					   $scope.childCatsTgt = response.data;
				   }
				   else {
					   $scope.childCats = response.data;
				   }
			   }, function errorCallback(response) {
				   console.log('AJAX ERROR!');
				   console.log(response);
			   });
	   }

	   //load child categories
	   $scope.GetProducts = function () {
		   var webId = $scope.selWeb.Id;
		   var childCatId = $scope.selChildCat.Id;

		   //copy child cat selected name to text box, to edit
		   $scope.txtChildCat = $scope.selChildCat.Name;

		   $http({
			   url: Url.baseUrl() + '/categories/getProducts?webId='+webId+'&childCatId='+childCatId,
			   method: "GET"
		   })
			   .then(function successCallback(response) {
				   $scope.products = response.data;
			   }, function errorCallback(response) {
				   console.log('AJAX ERROR!');
				   console.log(response);
			   });
	   }

   });