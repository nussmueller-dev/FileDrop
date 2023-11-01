import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OverviewComponent } from './pages/overview/overview.component';
import { UploadComponent } from './pages/upload/upload.component';
import {QrLoginComponent} from "./pages/qr-login/qr-login.component";
import {AcceptQrloginComponent} from "./pages/accept-qrlogin/accept-qrlogin.component";

const routes: Routes = [
  { path: 'upload', component: UploadComponent },
  { path: 'overview', component: OverviewComponent },
  { path: 'qr-login', component: QrLoginComponent },
  { path: 'accept-qrlogin', component: AcceptQrloginComponent },
  { path: '**', redirectTo: 'upload' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
