import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './pages/login/login.component';
import { LoginFormComponent } from './components/molecules/login-form/login-form.component';
import { LoaderComponent } from './components/molecules/loader/loader.component';
import { HomeComponent } from './pages/home/home.component';
import { UploadComponent } from './pages/upload/upload.component';
import { FileUploadFormComponent } from './components/molecules/file-upload-form/file-upload-form.component';
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    LoginFormComponent,
    LoaderComponent,
    HomeComponent,
    UploadComponent,
    FileUploadFormComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
