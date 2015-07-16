
var _goalServiceTest = _goalServiceTest || {};
_goalServiceTest.token = { accessToken: '', refreshToken: '' };

_goalServiceTest.showResult = function () {

    return {
        showToken: showToken,
        showResponse: showResponse,
        showError: showError
    }
    function showToken(object) {
        var token = object.access_token;
        _goalServiceTest.token.refreshToken = object.refresh_token;
        if (!token) {
            $('#errorMessage').text('Failed to get access token');
            return;
        }
        showJson($('#token'), object);
        _goalServiceTest.token.accessToken = token;
        cleanUp();
    }
    function showResponse(object) {
        showJson($('#responseData'), object);
        cleanUp();
    }
    function showError(object) {
        showJson($('#errorMessage'), object);
        cleanUp();
    }

    function cleanUp() {
        $('#imgLoading').hide();
    }

    function showJson($container, json) {
        $container.html('<pre></pre>');
        $container.find('pre').html(JSON.stringify(json, null, 4));
    }
};

_goalServiceTest.service = function () {
    var tokenRequestType = { request: 'password', refresh: 'refresh_token' };

    var showResult = _goalServiceTest.showResult();

    return {
        getToken: getToken,
        getRefreshToken: getRefreshToken,
        getService: getService
    }

    // ---
    function formEncode(data) {
        var pairs = [];
        for (var name in data) {
            pairs.push(encodeURIComponent(name) + '=' + encodeURIComponent(data[name]));
        }
        return pairs.join('&').replace(/%20/g, '+');
    }

    function getOAuthSettings(grantType, refreshToken) {
        var tokenEndpoin = $("#tokenEndpoint").val();
        if (tokenEndpoin.substr(0, 1) !== '/') {
            tokenEndpoin = '/' + tokenEndpoin;
        }
        return {
            urlToken: $('#baseUrl').val() + tokenEndpoin, //'http://localhost:63765/token';
            data: formEncode({
                username: $('#userName').val(),
                password: $('#password').val(),
                grant_type: grantType,  //"password","refresh_token"
                client_id: $('#clientCode').val(),
                client_secret: $('#clientSecret').val(),
                refresh_token: refreshToken
            }),

            clientCode: $('#clientCode').val(),
            clientSecret: $('#clientSecret').val()
        }
    }

    function getToken(funcAction) {

        $('#responseData').text('');
        $('#errorMessage').text('');
        $('#token').text('');
        $('#imgLoading').show();

        var oauth = getOAuthSettings(tokenRequestType.request);

        var ajaxSettings = {
            type: "POST",
            url: oauth.urlToken,
            dataType: 'json',
            headers: {
                "Authorization": "Basic " + btoa(oauth.clientCode + ":" + oauth.clientSecret),
                "Content-Type": "application/x-www-form-urlencoded"
            },
            data: oauth.data,
            success: jQuery.isFunction(funcAction) ? funcAction : showResult.showToken,
            error: showResult.showError
        };
        $.ajax(ajaxSettings);
    }

    function getService() {
        if (_goalServiceTest.token.accessToken) {
            getServiceWithToken(_goalServiceTest.token.accessToken);
        } else {
            getToken(callService);
        }
    }

    function callService(object) {

        showResult.showToken(object);

        getServiceWithToken(_goalServiceTest.token.accessToken);
    }

    function getServiceWithToken(token) {
        $('#imgLoading').show();
        $('#responseData').text('');
        $('#errorMessage').text('');
        var serviceEndpoint = $("#serviceEndpoint").val();
        if (serviceEndpoint.substr(0, 1) !== '/') {
            serviceEndpoint = '/' + serviceEndpoint;
        }
        var urlService = $('#baseUrl').val() + serviceEndpoint;
        var ajaxSettings = {
            type: "GET",
            url: urlService,
            dataType: 'json',
            headers: {
                "Authorization": "BEARER " + token
            },
            success: showResult.showResponse,
            error: showResult.showError
        };
        $.ajax(ajaxSettings);
    }


    function getRefreshToken() {
        $('#responseData').text('');
        $('#errorMessage').text('');
        $('#token').text('');
        $('#imgLoading').show();

        var oauth = getOAuthSettings(tokenRequestType.refresh, _goalServiceTest.token.refreshToken);

        var ajaxSettings = {
            type: "POST",
            url: oauth.urlToken,
            dataType: 'json',
            headers: {
                "Authorization": "Basic " + btoa(oauth.clientCode + ":" + oauth.clientSecret),
                "Content-Type": "application/x-www-form-urlencoded"
            },
            data: oauth.data,
            success: showResult.showToken,
            error: showResult.showError
        };
        $.ajax(ajaxSettings);
    }
}

_goalServiceTest.activate = function () {
    var service = _goalServiceTest.service();
    $('#imgLoading').hide();
    $("#btnGetToken").on('click', service.getToken);
    $("#btnRefreshToken").on('click', service.getRefreshToken);
    $('#btnCallService').on('click', service.getService);

    var baseUrl = window.location.protocol + "//" + window.location.host;
    $('#baseUrl').val(baseUrl);
}

$(function () {
    _goalServiceTest.activate();
});