$(document).ready(function () {

});

var Loading = {
    show: function () { $('#divLoading').show(); },
    hide: function () { $('#divLoading').hide(); }
};

var Url = {
    baseUrl: function () {
        //var base = window.location.port == '' ? window.location.origin + '/ecommerce' : window.location.origin;
        var base = window.location.origin;
        return base;
    }
};