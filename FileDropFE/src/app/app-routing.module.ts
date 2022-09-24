import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginFormComponent } from './components/login-form/login-form.component';
import { UploadComponent } from './pages/upload/upload.component';

const routes: Routes = [
  { path: 'upload', component: UploadComponent },
  { path: 'overview', component: LoginFormComponent },
  { path: '**', redirectTo: 'upload' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
