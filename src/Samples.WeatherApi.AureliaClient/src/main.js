// regenerator-runtime is to support async/await syntax in ESNext.
// If you don't use async/await, you can remove regenerator-runtime.
import 'regenerator-runtime/runtime';
import environment from './environment';
import Oidc from 'oidc-client';
import { HttpClient } from 'aurelia-fetch-client';

export function configure(aurelia) {
    aurelia.use
        .standardConfiguration()
        .feature('resources');

    aurelia.use.developmentLogging(environment.debug ? 'debug' : 'warn');

    if (environment.testing) {
        aurelia.use.plugin('aurelia-testing');
    }

    const container = aurelia.container;
    let authUrl = aurelia.host.dataset.authUrl;
    const appUrl = aurelia.host.dataset.appUrl;
    const apiUrl = aurelia.host.dataset.apiUrl;
    console.log('Element dataset:', aurelia.host.dataset);

    if (authUrl[authUrl.length - 1] === '/') {
        authUrl = authUrl.substr(0, authUrl.length - 1);
    }

    // Register UserManager instance with Aurelia container
    let userManager = new Oidc.UserManager({
        authority: authUrl,
        client_id: 'aurelia-client',
        redirect_uri: `${appUrl}login`,
        response_type: 'code',
        scope: 'openid profile weather-api',
        post_logout_redirect_uri: appUrl
    });
    container.registerInstance(Oidc.UserManager, userManager);

    // Register HttpClient instance with Aurelia container
    let httpClient = new HttpClient();
    httpClient.configure(config => {
        config
            .useStandardConfiguration()
            .withBaseUrl(apiUrl)
            .withDefaults({
                credentials: 'same-origin',
                headers: {
                    'X-Requested-With': 'Fetch'
                }
            })
            .withInterceptor({
                request(request) {
                    return userManager.getUser()
                        .then(user => {
                            request.headers.append('Authorization', `Bearer ${user.access_token}`);
                            return request;
                        });
                }
            });
    });
    container.registerInstance(HttpClient, httpClient);

    aurelia.start().then(() => aurelia.setRoot());
}
