angular.module('prodApp', [])
   .controller('productController', function($scope, $http) {
       $scope.searchKeyword = '';
       $scope.categories = [];
       $scope.websites = [];
       $scope.products = [];
       $scope.productsSel = [];
       $scope.pages = [1];
       $scope.pageSize = 1000;
       $scope.page = 1;
       $scope.found = 0;

       //set height of log window
       $('#logWindow').css('height', $(window).height() - 100);

       //set focus to search box
       $('#searchBox').focus();
       
       //select all products
       $scope.selectAllProds = function () {
           $scope.clearMsgBoxes();
           $('.prodRow').removeClass('boxHighlight').addClass('boxHighlight');

           $scope.updateProdsSel();
       };

       //deselect all products
       $scope.deselectAllProds = function () {
           $scope.clearMsgBoxes();
           $('.prodRow').removeClass('boxHighlight');

           $scope.updateProdsSel();
       };

       //toggle product box
       $scope.toggleProdRow = function (e) {
           $scope.clearMsgBoxes();
           var row = $(e.currentTarget);
           row.toggleClass('boxHighlight');

           $scope.updateProdsSel();
       };

       //add selected products to global array
       $scope.updateProdsSel = function () {
           $scope.productsSel = [];
           $('.boxHighlight').each(function () {
               $scope.productsSel.push($(this).attr('data-id'));
           });
       };

       //display message to Log Window
       $scope.addToLogBox = function (str) {
           $('#divLog').prepend(str + '<hr/>');
       };

       //save selected products to selected website with provided category
       $scope.pushToSite = function () {
            $scope.clearMsgBoxes();

            if ($scope.productsSel.length === 0) {
                $scope.updateProdsSel();
            }

            if ($scope.siteCategory === '') {
                $scope.error = "Provide categories for website!";
                $('.error').show();
            }
            else {
                //remove invalid chars
                $scope.siteCategory = $scope.siteCategory.replace('&', 'and').replace('"', '').replace(',', ' ');

                var data = {
                    webId: $scope.selectedWebsite,
                    supplierCat: $scope.selectedCategory,
                    siteCat: $scope.siteCategory
                };

                if ($scope.productsSel.length > 0) {
                    $scope.addToLogBox('Category Persisting...');
                    $http({
                        url: Url.baseUrl() + '/products/CategoryPersist',
                        method: "POST",
                        data: data
                    })
                        .then(function successCallback(response) {
                            $scope.addToLogBox(response.data.ErrForUser);
                            if (response.data.Success) { $scope.productsPersist(); }
                        }, function errorCallback(response) {
                            $scope.addToLogBox(response.data.ErrForUser);
                        });
                }
                else {
                    $scope.error = "Select product(s)!";
                    $('.error').show();
                }
            }
       };

       //persist products in website's db
       $scope.productsPersist = function () {
           var data = {
               productNums: $scope.productsSel,
               webId: $scope.selectedWebsite,
               supplierCat: $scope.selectedCategory,
               siteCat: $scope.siteCategory
           };

           $scope.addToLogBox('Products Persisting...');

           $http({
               url: Url.baseUrl() + '/products/ProductsPersist',
               method: "POST",
               data: data
           })
               .then(function successCallback(response) {
                   $scope.addToLogBox(response.data.ErrForUser);
                   if (response.data.Success) { $scope.clearImgDir(); }
               }, function errorCallback(response) {
                   $scope.addToLogBox(response.data.ErrForUser);
               });

       };

       //erase files in image directory
       $scope.clearImgDir = function () {
           $scope.addToLogBox('Image Directory Clearing...');

           $http({
               url: Url.baseUrl() + '/products/clearImgDir',
               method: "GET"
           })
               .then(function successCallback(response) {
                   $scope.addToLogBox(response.data.ErrForUser);
                   if (response.data.Success) { $scope.downloadImages(); }
               }, function errorCallback(response) {
                   $scope.addToLogBox(response.data.ErrForUser);
               });
       };

       //download images for products
       $scope.downloadImages = function () {
           var data = {
               productNums: $scope.productsSel,
               webId: $scope.selectedWebsite,
               supplierCat: $scope.selectedCategory,
               siteCat: $scope.siteCategory
           };

           $scope.addToLogBox('Images Downloading...');

           $http({
               url: Url.baseUrl() + '/products/downloadImages',
               method: "POST",
               data: data
           })
               .then(function successCallback(response) {
                   $scope.addToLogBox(response.data.ErrForUser);
                   //if (response.data.Success) { $scope.addImagesToDB(); }
               }, function errorCallback(response) {
                   $scope.addToLogBox(response.data.ErrForUser);
               });
       };

       //download images for products
       $scope.addImagesToDB = function () {
           var data = {
               productNums: $scope.productsSel,
               webId: $scope.selectedWebsite,
               supplierCat: $scope.selectedCategory,
               siteCat: $scope.siteCategory
           };

           $scope.addToLogBox('Images being added to database...');

           $http({
               url: Url.baseUrl() + '/products/addImagesToDB',
               method: "POST",
               data: data
           })
               .then(function successCallback(response) {
                   $scope.addToLogBox(response.data.ErrForUser);
               }, function errorCallback(response) {
                   $scope.addToLogBox(response.data.ErrForUser);
               });
       };

       //page selected
       $scope.goToPage = function () {
           if ($scope.selectedCategory === undefined) {
               $scope.getProductsByKeyword();
           }
           else if ($scope.searchKeyword === '') {
               $scope.getProductsForCategory();
           }
       };

       //get products from website for this category
       $scope.getProductsForWebsite = function () {
           $scope.clearMsgBoxes();
           $scope.deselectAllProds();

           var webId = $scope.selectedWebsite;
           var suppCat = encodeURIComponent($scope.selectedCategory);

           if ($scope.selectedCategory !== undefined) {
               $scope.searchKeyword = '';
               Loading.show();

               //clear products list
               $scope.products = [];
               $scope.selected = 0;

               $http({
                   url: Url.baseUrl() + '/products/getProductsForSite?webId=' + webId + '&category=' + suppCat + '&pageSize=' + $scope.pageSize + '&page=' + $scope.page,
                   method: 'GET'
               })
                   .then(function successCallback(response) {
                       $scope.products = response.data.Products;
                       $scope.siteCategory = response.data.WebsiteCategory;
                       $scope.FillPagesArray(response.data.Found);
                       $scope.found = response.data.Found;
                       Loading.hide();
                   }, function errorCallback(response) {

                       console.log('ERROR!');
                       console.log(response);
                       $scope.error = "An Error has occured while loading products!";
                       $('.error').show();
                       Loading.hide();
                   });
           }
           else {
               $scope.error = "Select Category OR Search for products!";
               $('.error').show();
           }
       };

       //load products for selected category
       $scope.getProductsForCategory = function () {
           $scope.clearMsgBoxes();
           $scope.deselectAllProds();

           if ($scope.selectedWebsite !== undefined) {
               $scope.getProductsForWebsite();
           }
           else {
               var suppCat = encodeURIComponent($scope.selectedCategory);
               $scope.searchKeyword = '';
               Loading.show();

               //clear products list
               $scope.products = [];
               $scope.page = 1;
               $scope.pages = [1];
               $scope.found = 0;

               $http({
                   url: Url.baseUrl() + '/products/getProducts?category=' + suppCat + '&pageSize=' + $scope.pageSize + '&page=' + $scope.page,
                   method: "GET"
               })
                   .then(function successCallback(response) {
                       $scope.products = response.data.Products;
                       $scope.FillPagesArray(response.data.Found);
                       $scope.found = response.data.Found;
                       Loading.hide();
                   }, function errorCallback(response) {
                       $scope.error = "An Error has occured while loading products!";
                       $('.error').show();
                       Loading.hide();
                   });
           }
       };

       //load products for keyword search
       $scope.getProductsByKeyword = function () {
           $scope.clearMsgBoxes();
           $scope.deselectAllProds();
           
           if ($scope.searchKeyword === '') {
               $scope.error = "Provide keyword to search!";
               $('.error').show();
           }
           else {
               $scope.selectedCategory = undefined;

               var keyword = encodeURIComponent($scope.searchKeyword);

               Loading.show();

               //clear products list
               $scope.products = [];
               $scope.selected = 0;

               $http({
                   url: Url.baseUrl() + '/products/productSearch?keyword=' + keyword + '&pageSize=' + $scope.pageSize + '&page=' + $scope.page,
                   method: "GET"
               })
                   .then(function successCallback(response) {
                       $scope.products = response.data.Products;
                       $scope.FillPagesArray(response.data.Found);
                       $scope.found = response.data.Found;
                       Loading.hide();
                   }, function errorCallback(response) {
                       $scope.error = "An Error has occured while loading products!";
                       $('.error').show();
                       Loading.hide();
                   });
           }
       };

       //fill pages array
       $scope.FillPagesArray = function (recs) {
           $scope.pages = [];
           for (var p = 0; p < recs/$scope.pageSize; p++) {
               $scope.pages.push($scope.pages.length + 1);
           }

       };

       //remove all warning and error messages
       $scope.clearMsgBoxes = function() {
           $scope.msg = '';
           $('.msg').hide();
           $scope.error = '';
           $('.error').hide();

       };
       
   });