import Oidc from 'oidc-client';
import { inject } from 'aurelia-framework';
import { Router } from 'aurelia-router';

@inject(Router)
export class Login {
    constructor(router) {
        this.router = router;
        this.error = '';

        this.doLogin();
    }

    doLogin() {
        new Oidc.UserManager({ response_mode: 'query' })
            .signinRedirectCallback()
            .then(() => {
                this.error = '';
                this.router.navigateToRoute('home');
            }).catch(error => {
                this.error = `Error logging user: ${error}`;
            });
    }
}
