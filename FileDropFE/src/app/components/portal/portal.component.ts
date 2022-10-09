import { HttpEvent, HttpEventType } from '@angular/common/http';
import {
  Component,
  EventEmitter,
  HostBinding,
  HostListener,
  Output,
} from '@angular/core';
import * as _ from 'lodash';
import { FileState } from 'src/app/shared/util/fileState';
import { FileStatusEnum } from 'src/app/shared/util/fileStatusEnum';
import { FileService } from './../../shared/services/file.service';

@Component({
  selector: 'app-portal',
  templateUrl: './portal.component.html',
  styleUrls: ['./portal.component.scss'],
})
export class PortalComponent {
  fileStates: Array<FileState> = new Array<FileState>();

  @Output() uploadingFilesChange = new EventEmitter<Array<FileState>>();

  private dragOver: boolean = false;
  private dragOverPortal: boolean = false;

  @HostBinding('class.drag-over') get dragOverClass() {
    return this.dragOver;
  }
  @HostBinding('class.drag-over-portal') get dragOverPortalClass() {
    return this.dragOverPortal;
  }

  @HostListener('document:dragover', ['$event']) onDragOver(event: DragEvent) {
    this.dragOver = true;
  }

  @HostListener('document:drop')
  @HostListener('document:dragleave')
  onDragLeave() {
    this.dragOver = false;
  }

  @HostListener('dragover', ['$event']) onDragOverPortal(event: DragEvent) {
    this.dragOverPortal = true;
  }

  @HostListener('drop')
  @HostListener('dragleave')
  onDragPortalLeave() {
    this.dragOverPortal = false;
  }

  constructor(private fileService: FileService) {}

  async uploadFiles(event: any) {
    let fileList: FileList = event.target.files;

    for (let i = 0; i < fileList.length; i++) {
      let file = fileList.item(i) as File;

      let observable = this.fileService.uploadFile(file);
      let fileState = new FileState(file.name);

      observable.subscribe({
        next: (event: HttpEvent<any>) => {
          if (fileState.status === FileStatusEnum.Error) {
            return;
          }

          switch (event.type) {
            case HttpEventType.Sent:
              console.log('Request has been made!');
              fileState.status = FileStatusEnum.Uploading;
              break;
            case HttpEventType.ResponseHeader:
              console.log('Response header has been received!');
              fileState.status = FileStatusEnum.Uploaded;
              break;
            case HttpEventType.UploadProgress:
              let progress = Math.round(
                (event.loaded / (event.total ?? 1)) * 100
              );
              fileState.progress = progress;
              console.log(`Uploaded! ${progress}%`);
              break;
            case HttpEventType.Response:
              fileState.status = FileStatusEnum.Uploaded;
          }
        },
        error: (error: any) => {
          fileState.status = FileStatusEnum.Error;
          console.log('Error while uploading!');
        },
      });

      this.fileStates.push(fileState);
    }

    this.fileStates = _.orderBy(this.fileStates, ['status'], ['asc']);

    this.uploadingFilesChange.emit(this.fileStates);
  }
}
