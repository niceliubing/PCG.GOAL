(function () {
    angular.module("webServices", ['ui.router', 'ui.bootstrap'])
    .run(function ($rootScope, $state) {
        $rootScope.$state = $state;
    });


})();


(function () {
    var app = angular.module("webServices");

    var dataServicePrefix = "/api/";

   
    var config = {
        docTitle: "Goal Web Service",
        dataServicePrefix: dataServicePrefix,
        dataServicePath: buildDataServicePath,
        cacheMaxAge: 5000,
        enableConsoleLog: true,
        enableToastrLog: true,
        version: '1.0.0'
    };


    function buildDataServicePath(endPoint,prefix) {
        if (!endPoint) {
            throw "End Point Is Empty!!";
        }
        endPoint = buildPath(endPoint.trim());

        if (!prefix) {
            prefix = dataServicePrefix;
        } else {
            prefix = buildPath(prefix);
        }
        return prefix + endPoint;
    }

    function buildPath(path) {
        if (path.indexOf('/') === 0) {
            path = path.substr(1);
        }
        if (path.indexOf('/') === path.length - 1) {
            path = path.substr(0, path.length - 1);
        }
        return path;
    }

    app.value("config", config);

    app.factory("logService", [function() {
        return {
            logError: function(data,disableToastr) {
                if (config.enableConsoleLog) {
                    console.log("message:  " + data.message + "\n" + "exceptionType:  "
                        + data.exceptionType + "\n" + "exceptionMessage:  " + data.exceptionMessage);
                }
                if (config.enableToastrLog && !disableToastr) {
                    if (data && data.exceptionMessage) {
                        toastr.error(data.exceptionMessage);
                    } else {
                        toastr.error("An error has occurred.");
                    }
                }
            }
        };
    }]);

    app.config(["$logProvider", function($logProvider) {
        // turn debugging off/on
        if ($logProvider.debugEnabled) {
            $logProvider.debugEnabled(true);
        }
    }]);
    app.config([
        "$httpProvider", function($httpProvider) {
            $httpProvider.defaults.withCredentials = true;
        }
    ]);
})();