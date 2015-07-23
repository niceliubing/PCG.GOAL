(function () {
    angular.module('webServices')
        .controller('appAdminCtrl', ['$rootScope', '$scope', 'dataContext', '$state',
            '$stateParams', 'logService','dialogService', appAdminCtrl]);

    function appAdminCtrl($rootScope, $scope, dataContext, $state, $stateParams, logService, dialogService) {
        $scope.isFromEdit = true;
        $scope.hasApp = false;
        $scope.isEditMode = ($state.current.name !== 'clientApp');
        $scope.backToAdmin = backToAdmin;
        $scope.deleteApp = deleteApp;
        $scope.saveApp = saveApp;


        // -- main function
        (function () {
            if ($scope.isEditMode && $stateParams.id) {
                getAppById($stateParams.id);
                $scope.title = 'Edit Application';
            } else {
                $scope.title = 'Add Applications';
            }

            resetApp();
            getApps();

        })();


        // -- functions

        function backToAdmin() {
            $state.go("clientApp");
        }


        function saveApp(formApp) {
            if (!formApp.$invalid) {

                dataContext.saveApp($scope.app)
                    .success(function (data) {
                        if (data.error) {
                            toastr.warning(data.error.message);
                        } else {
                            getApps();
                            resetApp();
                            if ($scope.isEditMode) {
                                $state.go('clientApp');
                            }
                            toastr.success("Saved Successfully");
                        }
                    })
                    .error(function (data, status, headers, config) {
                        logService.logError(data);
                    });
            } else {
                toastr.warning("Please correct the validation errors");
            }
        };


        function getApps() {
            dataContext.getApps().success(function (data) {
                if (data && data.data) {
                    $scope.appList = data.data;
                    $scope.hasApp = $scope.appList.length > 0;
                }
            });
            resetApp();
        }

        function getAppById(id) {
            dataContext.getAppById(id)
                .success(function (data) {
                    if (data && data.data) {
                        $scope.app = data.data[0];
                        $scope.app.clientSecret = '';
                        $scope.app.isImportant = true;
                    }
                })
                .error(function (data, status, headers, config) {
                    logService.logError(data);
                });
        }

        function deleteApp(id) {
            dialogService.confirmModal("Delete", "Are you sure to delete this client?", funcDeleteClient);

            function funcDeleteClient() {
                dataContext.deleteApp(id).success(function (data) {
                    getApps();
                    if (data && data.done === false) {
                        toastr.error(data.message);
                    }
                }).error(function (data, status, headers, config) {
                    logService.logError(data);
                });
            }
        }

        function resetApp() {
            $scope.app = { appName: '', clientId: '', clientSecret: '', description: '' };
        }
    }


})();