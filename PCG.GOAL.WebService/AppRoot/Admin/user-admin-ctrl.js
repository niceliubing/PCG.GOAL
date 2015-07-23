(function () {
    angular.module("webServices")
        .controller("userAdminCtrl", ["$scope", "dataContext", "$state", "$stateParams", "logService", "dialogService",
            userAdminCtrl]);

    function userAdminCtrl($scope, dataContext, $state, $stateParams, logService, dialogService) {
        resetUser();
        $scope.hasUser = false;
        $scope.deleteUser = function (id) {
            deleteUser(id);
        };

        $scope.isEditMode = ($state.current.name !== "credentials");

        if ($scope.isEditMode && $stateParams.id) {
            getUserById($stateParams.id);
            $scope.title = "Edit User";
        } else {
            $scope.title = "Add User";
        }

        getUsers();

        $scope.saveUser = function (formUser) {
            if (!formUser.$invalid) {

                dataContext.saveUser($scope.user)
                    .success(function (data) {
                        if (data.error) {
                            toastr.warning(data.error.message);
                        } else {
                            getUsers();
                            resetUser();
                            if ($scope.isEditMode) {
                                $state.go("credentials");
                            }
                            if (data.done) {
                                toastr.success("Saved Successful");
                            } else {
                                toastr.error("Failed to save");
                            }
                        }
                    })
                    .error(function (data, status, headers, config) {
                        logService.logError(data);
                    });
            } else {
                toastr.warning("Please correct the validation errors");
            }
        };


        function getUsers() {
            dataContext.getUsers().success(function (data) {
                if (data && data.data) {
                    $scope.userList = data.data;
                    $scope.hasUser = $scope.userList.length > 0;
                }
            }).error(function (data, status, headers, config) {
                logService.logError(data);
            });
        }

        function getUserById(id) {
            dataContext.getUserById(id).success(function (data) {
                if (data && data.data) {
                    $scope.user = data.data[0];
                    $scope.user.password = "";
                }
            }).error(function (data, status, headers, config) {
                logService.logError(data);
            });
        }
        function deleteUser(id) {
            dialogService.confirmModal("Delete", "Are you sure to delete this user?", funcDeleteUser);

            function funcDeleteUser() {
                dataContext.deleteUser(id).success(function (data) {
                    getUsers();
                    if (data && data.done === false) {
                        toastr.error(data.message);
                    }
                }).error(function (data, status, headers, config) {
                    logService.logError(data);
                });
            }

        }

        function resetUser() {
            $scope.user = { userName: "", password: "", role: "User" };
        }

    }


})();