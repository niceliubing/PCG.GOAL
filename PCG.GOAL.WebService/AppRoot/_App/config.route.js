(function () {

	angular.module("webServices").config(configRoute);

	function configRoute($stateProvider, $urlRouterProvider) {

		$stateProvider
			.state('home', {
				url: "/"
			})
			.state("credentials", {
				url: "/user",
				templateUrl: "/AppRoot/admin/userAdmin.html"
			})
			.state("credentialsEdit", {
			    url: "/credentials/:id",
			    templateUrl: "/AppRoot/admin/userEdit.html"
			})

			.state("clientApp", {
				url: "/app",
				templateUrl: "/AppRoot/admin/appAdmin.html"
			})
			.state("clientAppEdit", {
			    url: "/appedit/:id",
			    templateUrl: "/AppRoot/admin/appEdit.html"
			});

		$urlRouterProvider.otherwise('/user');

	}

})();