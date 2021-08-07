import { Component, OnInit } from '@angular/core';
import { HttpEvent, HttpEventType } from '@angular/common/http';
import { FileUploadService } from 'src/app/shared/services/file-upload.service';

@Component({
  selector: 'app-file-upload-form',
  templateUrl: './file-upload-form.component.html',
  styleUrls: ['./file-upload-form.component.scss']
})
export class FileUploadFormComponent implements OnInit {

  progress: number = 0;

  constructor(private uploadService: FileUploadService) { }

  ngOnInit(): void {
  }

  uploadFile(inputEvent: any) {
    let file: File = inputEvent.target.files[0];

    this.uploadService.uploadFile(file).subscribe((event: HttpEvent<any>) => {
      switch (event.type) {
        case HttpEventType.Sent:
          console.log('Request has been made!');
          break;
        case HttpEventType.ResponseHeader:
          console.log('Response header has been received!');
          break;
        case HttpEventType.UploadProgress:
          this.progress = Math.round(event.loaded / (event.total ?? 1) * 100);
          console.log(`Uploaded! ${this.progress}%`);
          break;
        case HttpEventType.Response:
          console.log('User successfully created!', event.body);
          setTimeout(() => {
            this.progress = 0;
          }, 1500);

      }
    })
  }
}
