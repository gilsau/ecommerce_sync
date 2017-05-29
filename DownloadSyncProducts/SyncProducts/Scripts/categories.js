angular.module('prodApp', [])
   .controller('productController', function($scope, $http) {
       $scope.loading = false;
       $scope.categories = [];
       $scope.products = [];

       //load categories on pageload
       $http({
           url: '/home/getCats',
           method: "GET"
       })
           .then(function successCallback(response) {
               $scope.categories = response.data;
               $scope.loading = false;
           }, function errorCallback(response) {
               $scope.error = "An Error has occured while loading categories!";
               $('.error').show();
               console.log(response);
               $scope.loading = false;
           });

       //select all products
       $scope.selectAllProds = function () {
           $scope.clearMsgBoxes();
           $('.prodBox').removeClass('boxHighlight').addClass('boxHighlight');
       };

       //deselect all products
       $scope.deselectAllProds = function () {
           $scope.clearMsgBoxes();
           $('.prodBox').removeClass('boxHighlight');
       };

       //toggle product box
       $scope.toggleProdBox = function (e) {
           $scope.clearMsgBoxes();
           var box = $(e.currentTarget);
           box.toggleClass('boxHighlight');
       };

       //save products for website/supplier category/site category
       $scope.saveProds = function () {
           $scope.clearMsgBoxes();

           if ($scope.selectedWebsite == undefined) {
               $scope.error = 'Select Website!';
               $('.error').show();
               $scope.loading = false;
           }
           else if ($scope.selectedCategory == undefined) {
               $scope.error = 'Select Category!';
               $('.error').show();
               $scope.loading = false;
           }
           else {
               $('.error').hide();
               $scope.loading = true;

               //get all selected products
               var arrProds = [];
               $('.boxHighlight').each(function () {
                   arrProds.push($(this).attr('data-id'));
               });

               //get site category info
               var radSiteCat = $('input[name="radSiteCategory"]:checked');
               var webCatId = radSiteCat.attr('data-id');
               var siteCatPath = radSiteCat.closest('tr').find('.siteCategoryPath').val();
               var siteCatFilter = radSiteCat.closest('tr').find('.siteCategoryFilter').val();

               var prodData = {
                   Products: arrProds,
                   WebCatId: webCatId,
                   WebsiteId: $scope.selectedWebsite,
                   SupplierCategory: $scope.selectedCategory.FullPath,
                   SiteCategoryPath: siteCatPath,
                   SiteCategoryFilter: siteCatFilter
               };

               console.log('prodData');
               console.log(prodData);

               $http({
                   url: '/home/saveProducts',
                   method: "POST",
                   data: prodData
               })
                   .then(function successCallback(response) {
                       $scope.loading = false;
                       $scope.msg = "Products were saved successfully!";
                       $('.msg').show();
                   }, function errorCallback(response) {
                       $scope.error = "An Error has occured while saving the products!";
                       $('.error').show();
                       console.log(response);
                       $scope.loading = false;
                   });
           }
       };

       //load site categories for SELECTED supplier category
       $scope.getSiteCategories = function () {
           $scope.clearMsgBoxes();

           if ($scope.selectedWebsite == undefined) {
               $scope.error = "Select Website!";
               $('.error').show();
           }
           else {
               $scope.loading = true;

               var webCatData = {
                   WebsiteId: $scope.selectedWebsite,
                   SupplierCategoryPath: $scope.selectedCategory.FullPath
               };

               console.log('webCatData');
               console.log(webCatData);

               //clear products list
               $scope.products = [];

               $http({
                   url: '/home/getSiteCategories',
                   method: "POST",
                   data: webCatData
               })
                   .then(function successCallback(response) {
                       $scope.siteCategories = response.data;
                       $scope.loading = false;

                       console.log('Site Categories');
                       console.log(response.data);

                   }, function errorCallback(response) {
                       $scope.error = "An Error has occured while loading Site Categories!";
                       $('.error').show();
                       $scope.loading = false;

                       console.log('ERROR!');
                       console.log(response);
                   });
           }
       };

       //load products for selected category
       $scope.getProductsForCategory = function () {
           $scope.clearMsgBoxes();

           var webId = $scope.selectedWebsite;
           var suppCatPath = encodeURIComponent($scope.selectedCategory.FullPath);
           var radSiteCat = $('input[name="radSiteCategory"]:checked');
           var siteCatPath = radSiteCat.closest('tr').find('.siteCategoryPath').val();
           var siteCatFilter = radSiteCat.closest('tr').find('.siteCategoryFilter').val();

           var catData = {
               WebsiteId: webId,
               SupplierCategoryPath: suppCatPath,
               SiteCategoryPath: siteCatPath,
               SiteCategoryFilter: siteCatFilter
           };

           if ($scope.selectedWebsite == undefined) {
               $scope.error = "Select Website!";
               $('.error').show();
           }
           else if ($scope.selectedCategory == undefined) {
               $scope.error = "Select Supplier Category!";
               $('.error').show();
           }
           else if (siteCatPath == undefined) {
               $scope.error = "Select/Add Site Category!";
               $('.error').show();
           }
           else {
               $scope.loading = true;

               console.log('catData');
               console.log(catData);

               //clear products list
               $scope.products = [];

               $http({
                   url: '/home/getProducts',
                   method: "POST",
                   data: catData
               })
                   .then(function successCallback(response) {
                       $scope.products = response.data;
                       $scope.loading = false;

                       console.log('Products');
                       console.log(response.data);

                   }, function errorCallback(response) {
                       $scope.error = "An Error has occured while loading products!";
                       $('.error').show();
                       $scope.loading = false;
                   });
           }
       };

       //remove category
       $scope.removeCategory = function () {
           $scope.clearMsgBoxes();

           var radSiteCat = $('input[name="radSiteCategory"]:checked');
           var webCatId = radSiteCat.attr('data-id');
           var siteCatPath = radSiteCat.closest('tr').find('.siteCategoryPath').val();
           var siteCatFilter = radSiteCat.closest('tr').find('.siteCategoryFilter').val();
           var catData = {
               WebCatId: webCatId,
               WebsiteId: $scope.selectedWebsite,
               SupplierCategoryPath: encodeURIComponent($scope.selectedCategory.FullPath),
               SiteCategoryPath: '',
               SiteCategoryFilter: ''
           };

           $http({
               url: '/home/removeSiteCategory',
               method: "POST",
               data: catData
           })
               .then(function successCallback(response) {
                   $scope.siteCategories = response.data;
                   $scope.loading = false;

                   console.log('Site Categories');
                   console.log(response.data);

               }, function errorCallback(response) {
                   $scope.error = "An Error has occured while loading site categories!";
                   $('.error').show();
                   $scope.loading = false;

                   console.log('Error!');
                   console.log(response.data);
               });
       };

       //add category
       $scope.addCategory = function (e) {
           $scope.clearMsgBoxes();

           var catData = {
               WebsiteId: $scope.selectedWebsite,
               SupplierCategoryPath: encodeURIComponent($scope.selectedCategory.FullPath),
               SiteCategoryPath: '',
               SiteCategoryFilter: ''
           };

           $http({
               url: '/home/addSiteCategory',
               method: "POST",
               data: catData
           })
               .then(function successCallback(response) {
                   $scope.siteCategories = response.data;
                   $scope.loading = false;

                   console.log('Site Categories');
                   console.log(response.data);

               }, function errorCallback(response) {
                   $scope.error = "An Error has occured while loading site categories!";
                   $('.error').show();
                   $scope.loading = false;

                   console.log('Error!');
                   console.log(response.data);
               });
       };

       $scope.clearMsgBoxes = function() {
           $scope.msg = '';
           $('.msg').hide();
           $scope.error = '';
           $('.error').hide();

       };
   });