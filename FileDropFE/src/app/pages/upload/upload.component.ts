import { Component, HostBinding, OnInit } from '@angular/core';
import { FileState } from 'src/app/shared/util/fileState';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss']
})
export class UploadComponent implements OnInit {
  uploadingFiles: Array<FileState> = new Array<FileState>();

  @HostBinding('class.uploading-file') get uploadingFile() { return this.uploadingFiles.length !== 0; }

  constructor() { }

  ngOnInit(): void {
  }

}
