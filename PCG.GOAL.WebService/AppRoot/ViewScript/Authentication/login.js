$(function () {

    $("#btnLogin").on('click', login);

    function login(e) {
        if ($('#txtUsername').val() === '' || $('#txtPassword').val() === '') {
            toastr.warning("Please enter username and password");
            e.preventDefault();
            return;
        }
        $('#formLogin').attr('action', "Login").attr('method', 'post').submit();
        
        //$.post('/Authentication/Login/', $('#formLogin').serialize());
    }

    var loginFailed = $('#loginFailed').val();
    if (loginFailed && loginFailed.length > 0) {
        toastr.warning("Invalid username or password");
    }
});