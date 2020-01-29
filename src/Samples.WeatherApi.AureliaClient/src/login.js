import Oidc from 'oidc-client';
import { inject } from 'aurelia-framework';
import { Router } from 'aurelia-router';

@inject(Oidc, Router)
export class Login {
    constructor(oidc, router) {
        this.oidc = oidc;
        this.router = router;

        this.doLogin();
    }

    doLogin() {
        new this.oidc.UserManager({ response_mode: 'query' }).signinRedirectCallback().then(() => {
            this.router.navigateToRoute('home');
        }).catch(e => {
            console.error(e);
        });
    }
}
