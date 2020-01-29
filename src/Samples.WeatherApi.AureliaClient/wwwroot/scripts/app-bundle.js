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
define('text!app.html',[],function(){return "<template>\n    <div>\n        <router-view></router-view>\n    </div>\n</template>\n";});;
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
define('home',["exports", "aurelia-framework", "oidc-client"], function (_exports, _aureliaFramework, _oidcClient) {
  "use strict";

  _exports.__esModule = true;
  _exports.App = void 0;
  _oidcClient = _interopRequireDefault(_oidcClient);

  var _dec, _class;

  function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

  var App = (_dec = (0, _aureliaFramework.inject)(_oidcClient.default), _dec(_class =
  /*#__PURE__*/
  function () {
    function App(oidc) {
      this.oidc = oidc;
      this.message = '';
      this.initializeUserManager();
    }

    var _proto = App.prototype;

    _proto.initializeUserManager = function initializeUserManager() {
      var config = {
        authority: 'https://localhost:44396',
        client_id: 'aurelia',
        redirect_uri: 'https://localhost:44336/login',
        response_type: 'code',
        scope: 'openid profile weather-api',
        post_logout_redirect_uri: 'https://localhost:44336/'
      };
      this.userManager = new _oidcClient.default.UserManager(config);
      this.userManager.getUser().then(function (user) {
        if (user) {
          console.log('User logged in', user.profile);
        } else {
          console.log('User not logged in');
        }
      });
    };

    _proto.login = function login() {
      this.userManager.signinRedirect();
    };

    return App;
  }()) || _class);
  _exports.App = App;
});;
define('text!home.html',[],function(){return "<template>\n    <h1>Aurelia Client</h1>\n\n    <button click.delegate=\"login()\">Login</button>\n    <button click.delegate=\"callApi()\">Call API</button>\n    <button click.delegate=\"logout()\">Logout</button>\n\n    <div>${message}</div>\n\n    <pre id=\"results\"></pre>\n</template>\n";});;
define('login',["exports", "oidc-client", "aurelia-framework", "aurelia-router"], function (_exports, _oidcClient, _aureliaFramework, _aureliaRouter) {
  "use strict";

  _exports.__esModule = true;
  _exports.Login = void 0;
  _oidcClient = _interopRequireDefault(_oidcClient);

  var _dec, _class;

  function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

  var Login = (_dec = (0, _aureliaFramework.inject)(_oidcClient.default, _aureliaRouter.Router), _dec(_class =
  /*#__PURE__*/
  function () {
    function Login(oidc, router) {
      this.oidc = oidc;
      this.router = router;
      this.doLogin();
    }

    var _proto = Login.prototype;

    _proto.doLogin = function doLogin() {
      var _this = this;

      new this.oidc.UserManager({
        response_mode: 'query'
      }).signinRedirectCallback().then(function () {
        _this.router.navigateToRoute('home');
      }).catch(function (e) {
        console.error(e);
      });
    };

    return Login;
  }()) || _class);
  _exports.Login = Login;
});;
define('text!login.html',[],function(){return "<template>\r\n    Logging in...\r\n</template>";});;
define('main',["exports", "regenerator-runtime/runtime", "./environment"], function (_exports, _runtime, _environment) {
  "use strict";

  _exports.__esModule = true;
  _exports.configure = configure;
  _environment = _interopRequireDefault(_environment);

  function _interopRequireDefault(obj) { return obj && obj.__esModule ? obj : { default: obj }; }

  // regenerator-runtime is to support async/await syntax in ESNext.
  // If you don't use async/await, you can remove regenerator-runtime.
  function configure(aurelia) {
    aurelia.use.standardConfiguration().feature('resources');
    aurelia.use.developmentLogging(_environment.default.debug ? 'debug' : 'warn');

    if (_environment.default.testing) {
      aurelia.use.plugin('aurelia-testing');
    }

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