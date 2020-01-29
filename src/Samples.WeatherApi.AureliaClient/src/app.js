export class App {

    configureRouter(config, router) {
        config.title = 'Aurelia Client';
        config.options.pushState = true;

        config.map([
            { route: '', name: 'home', moduleId: 'home', nav: true, title: 'Home' },
            { route: 'login', name: 'login', moduleId: 'login', nav: false, title: 'Login' }
        ]);

        this.router = router;
    }
}