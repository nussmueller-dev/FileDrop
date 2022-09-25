import { Component, HostBinding } from '@angular/core';
import { Router } from '@angular/router';
import { FileState } from 'src/app/shared/util/fileState';
import { FileStatusEnum } from 'src/app/shared/util/fileStatusEnum';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss']
})
export class UploadComponent{
  fileStatusEnum = FileStatusEnum;
  uploadingFiles: Array<FileState> = new Array<FileState>();

  @HostBinding('class.uploading-file') get uploadingFile() { return this.uploadingFiles.length !== 0; }

  constructor(
    private router: Router
  ) { }

  async navigateToOverview() {
    if(this.uploadingFiles.every(x => x.status === FileStatusEnum.Uploaded)) {
      this.router.navigate(['overview']);
      return;
    }

    if(confirm('You have files that are still uploading. Are you sure you want to leave this page?')) {
      this.router.navigate(['overview']);
    }
  }
}
