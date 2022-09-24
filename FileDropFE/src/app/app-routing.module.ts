import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { OverviewComponent } from './pages/overview/overview.component';
import { UploadComponent } from './pages/upload/upload.component';

const routes: Routes = [
  { path: 'upload', component: UploadComponent },
  { path: 'overview', component: OverviewComponent },
  { path: '**', redirectTo: 'upload' },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
