$(function () {

    $("#btnLogin").on('click', login);

    function login(e) {
        $('#formLogin').attr('action', "Login").attr('method','post').submit();
    }
});