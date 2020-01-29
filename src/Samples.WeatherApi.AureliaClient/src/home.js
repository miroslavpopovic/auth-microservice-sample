import { inject } from 'aurelia-framework';
import Oidc from 'oidc-client';

@inject(Oidc)
export class App {
    constructor(oidc) {
        this.oidc = oidc;
        this.message = '';

        this.initializeUserManager();
    }

    initializeUserManager() {
        const config = {
            authority: 'https://localhost:44396',
            client_id: 'aurelia',
            redirect_uri: 'https://localhost:44336/login',
            response_type: 'code',
            scope: 'openid profile weather-api',
            post_logout_redirect_uri: 'https://localhost:44336/',
        };

        this.userManager = new Oidc.UserManager(config);
        this.userManager.getUser().then(user => {
            if (user) {
                console.log('User logged in', user.profile);
            } else {
                console.log('User not logged in');
            }
        });
    }

    login() {
        this.userManager.signinRedirect();
    }
}