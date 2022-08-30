jQuery(document).ready(function () {
    //jQuery('.collapse.in').prev('.panel-heading').addClass('active');
    jQuery('#accordion, #bs-collapse')
        .on('show.bs.collapse', function (a) {
            jQuery(a.target).prev('.panel-heading').addClass('active');
        })
        .on('hide.bs.collapse', function (a) {
            jQuery(a.target).prev('.panel-heading').removeClass('active');
        });
});