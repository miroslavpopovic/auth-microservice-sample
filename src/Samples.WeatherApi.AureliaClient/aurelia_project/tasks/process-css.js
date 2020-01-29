import {build} from 'aurelia-cli';
import gulp from 'gulp';
import project from '../aurelia.json';
import sass from 'gulp-sass';

export default function processCSS() {
  return gulp.src(project.cssProcessor.source, {sourcemaps: true})
    .pipe(sass().on('error', sass.logError))
    .pipe(build.bundle());
}

