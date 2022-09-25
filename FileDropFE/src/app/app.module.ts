import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { DateTimeInterceptor } from './config/date-time.interceptor';

import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { AngularSvgIconModule } from 'angular-svg-icon';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoaderComponent } from './components/loader/loader.component';
import { LoginFormComponent } from './components/login-form/login-form.component';
import { PortalComponent } from './components/portal/portal.component';
import { OverviewComponent } from './pages/overview/overview.component';
import { UploadComponent } from './pages/upload/upload.component';

@NgModule({
  declarations: [
    AppComponent,
    UploadComponent,
    OverviewComponent,
    LoginFormComponent,
    PortalComponent,
    LoaderComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    AngularSvgIconModule.forRoot(),
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: DateTimeInterceptor,
      multi: true,
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule {}
