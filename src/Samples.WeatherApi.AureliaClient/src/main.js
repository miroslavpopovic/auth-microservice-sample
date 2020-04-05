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
    const urlPrefix = aurelia.host.dataset.urlPrefix;
    console.log('Element dataset:', aurelia.host.dataset);

    // Register UserManager instance with Aurelia container
    let userManager = new Oidc.UserManager({
        authority: `${urlPrefix}:44396`,
        client_id: 'aurelia-client',
        redirect_uri: `${urlPrefix}:44336/login`,
        response_type: 'code',
        scope: 'openid profile weather-api',
        post_logout_redirect_uri: `${urlPrefix}:44336/`
    });
    container.registerInstance(Oidc.UserManager, userManager);

    // Register HttpClient instance with Aurelia container
    let httpClient = new HttpClient();
    httpClient.configure(config => {
        config
            .useStandardConfiguration()
            .withBaseUrl(`${urlPrefix}:44373/`)
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
