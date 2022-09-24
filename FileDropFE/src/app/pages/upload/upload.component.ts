import { Component, HostBinding } from '@angular/core';
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
}
