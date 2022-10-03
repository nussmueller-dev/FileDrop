import { Component, HostBinding, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FileState } from 'src/app/shared/util/fileState';
import { FileStatusEnum } from 'src/app/shared/util/fileStatusEnum';

@Component({
  selector: 'app-upload',
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.scss'],
})
export class UploadComponent implements OnInit {
  fileStatusEnum = FileStatusEnum;
  uploadingFiles: Array<FileState> = new Array<FileState>();

  test = '';

  @HostBinding('class.uploading-file') get uploadingFile() {
    return this.uploadingFiles.length !== 0;
  }

  constructor(private router: Router, private route: ActivatedRoute) {}

  ngOnInit() {
    let fileTitle = this.route.snapshot.queryParamMap.get('filetitle');
    let fileType = this.route.snapshot.queryParamMap.get('filetype');

    if (fileTitle && fileType) {
      var intiFileStatus = new FileState(fileTitle + '.' + fileType);
      intiFileStatus.status = FileStatusEnum.Uploaded;
      this.uploadingFiles.push(intiFileStatus);
    }
  }

  async navigateToOverview() {
    if (
      this.uploadingFiles.every((x) => x.status === FileStatusEnum.Uploaded)
    ) {
      this.router.navigate(['overview']);
      return;
    }

    if (
      confirm(
        'You have files that are still uploading. Are you sure you want to leave this page?'
      )
    ) {
      this.router.navigate(['overview']);
    }
  }
}
