import { HttpClient } from 'aurelia-fetch-client';
import { inject } from 'aurelia-framework';
import Oidc from 'oidc-client';

@inject(Oidc.UserManager, HttpClient)
export class App {
    constructor(userManager, httpClient) {
        this.error = '';
        this.message = '';

        this.userManager = userManager;
        this.httpClient = httpClient;

        this.displayUserInfo();
    }

    callApi() {
        this.httpClient
            .fetch('weatherforecast')
            .then(response => response.json())
            .then(forecasts => {
                this.error = '';
                this.message = 'Data loaded successfully';
                this.result = JSON.stringify(forecasts, null, 2);
            })
            .catch(error => {
                this.error = `Error loading data: ${error}`;
                this.message = '';
            });
    }

    displayUserInfo() {
        this.userManager.getUser()
            .then(user => {
                if (user) {
                    this.error = '';
                    this.message = `User logged in: ${user.profile.name}`;
                    console.log(user.profile);
                } else {
                    this.error = 'User not logged in!';
                    this.message = '';
                }
            })
            .catch(error => {
                this.error = `Error loading user data: ${error}`;
                this.message = '';
            });
    }

    login() {
        this.userManager.signinRedirect();
    }

    logout() {
        this.userManager.signoutRedirect();
    }
}
