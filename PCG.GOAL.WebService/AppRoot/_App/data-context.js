
(function () {
    angular.module("webServices").factory('dataContext', ['$http', 'config', dataService]);

    function dataService($http, config) {
        return {
            saveUser: saveUser,
            deleteUser: deleteUser,
            getUsers: getUsers,
            getUserById: getUserById,
            getApps: getApps,
            getAppById: getAppById,
            saveApp: saveApp,
            deleteApp: deleteApp
        };

        function httpGetConfig(endpoint) {
            return {
                method: 'GET',
                url: dataWith(endpoint),
                headers: {
                    'Authorization': 'Bearer ' + getToken()
                }
            }
        }
        function httpPostConfig(endpoint,data) {
            return {
                method: 'POST',
                url: dataWith(endpoint),
                data:data,
                headers: {
                    'Authorization': 'Bearer ' + getToken()
                }
            }
        }

        function getToken() {
            return angular.element("#token").text();
        }

        function dataWith(endPoint) {
            return config.dataServicePath(endPoint);
        }
        function getUsers() {
            return $http(httpGetConfig('admin/credentials/'));
        }

        function getUserById(id) {
            return $http(httpGetConfig('admin/credentials/' + id));
        }

        function saveUser(user) {
            return $http(httpPostConfig('admin/AddCredentials/', user));
        }

        function deleteUser(id) {
            return $http(httpGetConfig('admin/DeleteCredentials/' + id));
        }

        function getApps() {
            return $http(httpGetConfig('admin/apps'));
        }

        function getAppById(id) {
            return $http(httpGetConfig('admin/app/' + id));
        }

        function saveApp(app) {
            return $http(httpPostConfig('admin/addapp', app));
        }

        function deleteApp(id) {
            return $http(httpGetConfig('admin/deleteapp/' + id));
        }



    }
})();