angular.module('prodApp', [])
   .controller('productController', function($scope, $http) {
       $scope.loading = true;
       $scope.addMode = false;

       //Used to get data for categories tab
       $http
        .get('/home/getcats')
        .then(function successCallback(response) {
            $scope.categories = response.data;
            $scope.loading = false;
       }, function errorCallback(response) {
           $scope.error = "An Error has occured while loading categories!";
           $scope.loading = false;
       });

       $scope.showProducts = function (cat) {
           $('a[href="#tab3"]').click();
           $('#selCategories option:contains(' + cat.Category1 + ' - ' + cat.Category2 + ' - ' + cat.Category3 + ')').prop('selected', true);
           $scope.getProducts(cat);
       };

       $scope.getProducts = function (cat) { 
           $scope.loading = true;

           console.log('cat for get products');
           console.log(cat);

           $http({
               url: '/home/getproducts',
               method: "GET",
               params: cat
           })
            .then(function successCallback(response) {
                $scope.products = response.data;
                $scope.loading = false;
            }, function errorCallback(response) {
                $scope.error = "An Error has occured while loading categories!";
                $scope.loading = false;
            });
       };

       $scope.updateWebMemsAndProdCounts = function () {
            $scope.loading = true;
            
            //collect web memberships from each cat
            var webMems = [];
            
            $('.bigCheck:checked').each(function () {
                var chk = $(this);
                var arrCats = chk.attr('data-categories').split('|');

                webMems.push({
                WebId: chk.attr('data-web-id'),
                IsMember: true,
                Category1: arrCats[0],
                Category2: arrCats[1],
                Category3: arrCats[2]
                });
            });


            $http({
                url: '/home/updateWebMemsAndProdCounts',
                method: "POST",
                data: { webMems: webMems }
            })
            .then(function successCallback(response) {
                $scope.categories = response.data;
                $scope.loading = false;
            }, function errorCallback(response) {
                $scope.error = "An Error has occured while loading categories!";
                console.log(response);
                $scope.loading = false;
            });
       };
   });

$(document).ready(function () {

    //set categories panel height
    $('#pnlCats .panel-body').css('height', $(window).height() - 400);

    //mega checkboxes
    $('.megaChkAll').click(function () {
        var chk = $(this);
        var webId = chk.attr('data-web-id');
        
        $('.chk-' + webId).prop('checked', true);
    });

    //mega checkboxes
    $('.megaChkNone').click(function () {
        var chk = $(this);
        var webId = chk.attr('data-web-id');

        $('.chk-' + webId).prop('checked', false);
    });
});