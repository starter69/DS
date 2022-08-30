jQuery(document).ready(function () {
    //When click on login button send data to web api
    jQuery(document).on('click', '#btnLogin', function () {
        if (jQuery('#inputEmail').val() == '') {
            alertify.error('Username is mandatory.');
            jQuery('#inputEmail').focus();
            return;
        }
        if (jQuery('#inputPassword').val() == '') {
            alertify.error('Password is mandatory.');
            jQuery('#inputPassword').focus();
            return;
        }
        var UserInfo = {
            'userName': jQuery('#inputEmail').val(),
            'password': jQuery('#inputPassword').val(),
        };
        var dataJson = JSON.stringify(UserInfo);
        $.ajax({
            url: '/api/WebAPI_Authentication',
            type: "POST",
            contentType: 'application/json',
            data: dataJson,
            success: function (response) {
                if (response.userName != null) {
                    createCookie('userName', response.userName, 20);
                    document.location.href = '/Home';
                }
                else
                    alertify.error('Username or Password is not correct.');
            },
            error: function (response) {

            },
            failure: function (response) {

            }
        })
    });
});