//-----------------------------------------------bootstrap
$(document).ready(function () {
    $('ul.nav.navbar-nav, ul.list-group, ul.nav.nav-tabs').find('a[href="' + location.pathname + '"]')
                          .closest('li')
                          .addClass('active');

});
//-----------------------------------------------bootstrap