/*
 * usage:
 * dialogService.confirmModal('Confirm','Are you sure?',function_Ok);
 *
 * */

(function () {

    angular.module("webServices").factory('dialogService', ['$modal', rainConfirm]);

    function rainConfirm($modal) {

        return {
            confirmModal: confirmModal,
            messageModal: messageModal
        };

        // confirmModal
        function confirmModal(title, message, funcOk) {

            title = title || 'Confirm';
            message = message || 'Are you sure?';

            var modalInstance = $modal.open({
                //templateUrl: 'deleteUserModal.html',
                //size:'sm',
                template: getConfirmTemplate(title, message),
                controller: function ($scope, $modalInstance) {
                    $scope.ok = function () {
                        if (funcOk && angular.isFunction(funcOk)) {
                            funcOk();
                            //return;
                        }
                        $modalInstance.close(true);
                    };
                    $scope.cancel = function () {
                        $modalInstance.close(false);
                    };
                }
            });
            return modalInstance.result;
        }

        function getConfirmTemplate(title, message) {
            return '<div class="modal-header">'
                + '<h3 class="modal-title">' + title + '</h3>'
                + '</div>'
                + '<div class="modal-body">'
                + '<p style="font-size: 16px;">' + message + '</p>'
                + '</div>'
                + '<div class="modal-footer">'
                + '<button class="btn btn-primary" ng-click="ok()">Yes</button>'
                + '<button class="btn btn-warning" ng-click="cancel()">No</button>'
                + '</div>';
        }

        // messageModal
        function messageModal(title, markup, funcOk) {

            title = title || 'Information';
            markup = markup || '<p></p>';

            var modalInstance = $modal.open({
                //size:'sm',
                template: getMessageTemplate(title, markup),
                controller: function ($scope, $modalInstance) {
                    $scope.ok = function () {
                        if (funcOk && angular.isFunction(funcOk)) {
                            funcOk();
                            //return;
                        }
                        $modalInstance.close(true);
                    };
                }
            });
            return modalInstance.result;
        }

        function getMessageTemplate(title, markup) {
            return '<div class="modal-header">'
                + '<h3 class="modal-title">' + title + '</h3>'
                + '</div>'
                + '<div class="modal-body">'
                + markup
                + '</div>'
                + '<div class="modal-footer">'
                + '<button class="btn btn-primary" ng-click="ok()">Close</button>'
                + '</div>';
        }
    }
})();