define('app',["exports"], function (_exports) {
  "use strict";

  _exports.__esModule = true;
  _exports.App = void 0;

  var App =
  /*#__PURE__*/
  function () {
    function App() {}

    var _proto = App.prototype;

    _proto.configureRouter = function configureRouter(config, router) {
      config.title = 'Aurelia Client';
      config.options.pushState = true;
      config.map([{
        route: '',
        name: 'home',
        moduleId: 'home',
        nav: true,
        title: 'Home'
      }, {
        route: 'login',
        name: 'login',
        moduleId: 'login',
        nav: false,
        title: 'Login'
      }]);
      this.router = router;
    };

    return App;
  }();

  _exports.App = App;
});;
define('text!app.html',[],function(){return "<template>\r\n    <require from=\"bootstrap/css/bootstrap.min.css\"></require>\r\n    <main class=\"container\">\r\n        <div>\r\n            <router-view></router-view>\r\n        </div>\r\n    </main>\r\n</template>\r\n";});;
define('environment',["exports"], function (_exports) {
  "use strict";

  _exports.__esModule = true;
  _exports.default = void 0;
  var _default = {
    debug: true,
    testing: true
  };
  _exports.default = _default;
});;
define('home',["exports", "aurelia-fetch-client", "aurelia-framework", "oidc-client"], function (_exports, _aureliaFetchClient, _aureliaFramework, _oidcClient) {
  "use strict";

  _exports.__esModule = true;
  _exports.App = void 0;
  _oidcClient = _interopRequireDefault(_oidcClient);

  var _dec, _class;

  function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

  var App = (_dec = (0, _aureliaFramework.inject)(_oidcClient.default.UserManager, _aureliaFetchClient.HttpClient), _dec(_class =
  /*#__PURE__*/
  function () {
    function App(userManager, httpClient) {
      this.error = '';
      this.message = '';
      this.userManager = userManager;
      this.httpClient = httpClient;
      this.displayUserInfo();
    }

    var _proto = App.prototype;

    _proto.callApi = function callApi() {
      var _this = this;

      this.httpClient.fetch('weatherforecast').then(function (response) {
        return response.json();
      }).then(function (forecasts) {
        _this.error = '';
        _this.message = 'Data loaded successfully';
        _this.result = JSON.stringify(forecasts, null, 2);
      }).catch(function (error) {
        _this.error = "Error loading data: " + error;
        _this.message = '';
      });
    };

    _proto.displayUserInfo = function displayUserInfo() {
      var _this2 = this;

      this.userManager.getUser().then(function (user) {
        if (user) {
          _this2.error = '';
          _this2.message = "User logged in: " + user.profile.name;
          console.log(user.profile);
        } else {
          _this2.error = 'User not logged in!';
          _this2.message = '';
        }
      }).catch(function (error) {
        _this2.error = "Error loading user data: " + error;
        _this2.message = '';
      });
    };

    _proto.login = function login() {
      this.userManager.signinRedirect();
    };

    _proto.logout = function logout() {
      this.userManager.signoutRedirect();
    };

    return App;
  }()) || _class);
  _exports.App = App;
});;
define('text!home.html',[],function(){return "<template>\r\n    <h1>Aurelia Client</h1>\r\n\r\n    <hr />\r\n\r\n    <div if.bind=\"error\" class=\"alert alert-danger\">${error}</div>\r\n    <div if.bind=\"message\" class=\"alert alert-info\">${message}</div>\r\n\r\n    <pre if.bind=\"result\"><code>${result}</code></pre>\r\n\r\n    <hr />\r\n\r\n    <button class=\"btn btn-secondary\" click.delegate=\"login()\">Login</button>\r\n    <button class=\"btn btn-secondary\" click.delegate=\"callApi()\">Call API</button>\r\n    <button class=\"btn btn-secondary\" click.delegate=\"logout()\">Logout</button>\r\n\r\n</template>\r\n";});;
define('login',["exports", "oidc-client", "aurelia-framework", "aurelia-router"], function (_exports, _oidcClient, _aureliaFramework, _aureliaRouter) {
  "use strict";

  _exports.__esModule = true;
  _exports.Login = void 0;
  _oidcClient = _interopRequireDefault(_oidcClient);

  var _dec, _class;

  function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

  var Login = (_dec = (0, _aureliaFramework.inject)(_aureliaRouter.Router), _dec(_class =
  /*#__PURE__*/
  function () {
    function Login(router) {
      this.router = router;
      this.error = '';
      this.doLogin();
    }

    var _proto = Login.prototype;

    _proto.doLogin = function doLogin() {
      var _this = this;

      new _oidcClient.default.UserManager({
        response_mode: 'query'
      }).signinRedirectCallback().then(function () {
        _this.error = '';

        _this.router.navigateToRoute('home');
      }).catch(function (error) {
        _this.error = "Error logging user: " + error;
      });
    };

    return Login;
  }()) || _class);
  _exports.Login = Login;
});;
define('text!login.html',[],function(){return "<template>\r\n    <div class=\"alert alert-info\" if.bind=\"!error\">\r\n        Logging in...\r\n    </div>\r\n    <div class=\"alert alert-danger\" if.bind=\"error\">\r\n        ${error}\r\n    </div>\r\n</template>\r\n";});;
define('main',["exports", "regenerator-runtime/runtime", "./environment", "oidc-client", "aurelia-fetch-client"], function (_exports, _runtime, _environment, _oidcClient, _aureliaFetchClient) {
  "use strict";

  _exports.__esModule = true;
  _exports.configure = configure;
  _environment = _interopRequireDefault(_environment);
  _oidcClient = _interopRequireDefault(_oidcClient);

  function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

  // regenerator-runtime is to support async/await syntax in ESNext.
  // If you don't use async/await, you can remove regenerator-runtime.
  function configure(aurelia) {
    aurelia.use.standardConfiguration().feature('resources');
    aurelia.use.developmentLogging(_environment.default.debug ? 'debug' : 'warn');

    if (_environment.default.testing) {
      aurelia.use.plugin('aurelia-testing');
    }

    var container = aurelia.container;
    var urlPrefix = aurelia.host.dataset.urlPrefix;
    console.log('Element dataset:', aurelia.host.dataset); // Register UserManager instance with Aurelia container

    var userManager = new _oidcClient.default.UserManager({
      authority: urlPrefix + ":44396",
      client_id: 'aurelia-client',
      redirect_uri: urlPrefix + ":44336/login",
      response_type: 'code',
      scope: 'openid profile weather-api',
      post_logout_redirect_uri: urlPrefix + ":44336/"
    });
    container.registerInstance(_oidcClient.default.UserManager, userManager); // Register HttpClient instance with Aurelia container

    var httpClient = new _aureliaFetchClient.HttpClient();
    httpClient.configure(function (config) {
      config.useStandardConfiguration().withBaseUrl(urlPrefix + ":44373/").withDefaults({
        credentials: 'same-origin',
        headers: {
          'X-Requested-With': 'Fetch'
        }
      }).withInterceptor({
        request: function request(_request) {
          return userManager.getUser().then(function (user) {
            _request.headers.append('Authorization', "Bearer " + user.access_token);

            return _request;
          });
        }
      });
    });
    container.registerInstance(_aureliaFetchClient.HttpClient, httpClient);
    aurelia.start().then(function () {
      return aurelia.setRoot();
    });
  }
});;
define('resources/index',["exports"], function (_exports) {
  "use strict";

  _exports.__esModule = true;
  _exports.configure = configure;

  function configure(config) {//config.globalResources([]);
  }
});;
define('resources',['resources/index'],function(m){return m;});
//# sourceMappingURL=app-bundle.js.map