import { Component, OnInit } from '@angular/core';
import { DateTime } from 'luxon';
import { FileViewModel } from 'src/app/shared/models/file-view-model';
import { FileService } from './../../shared/services/file.service';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss'],
})
export class OverviewComponent implements OnInit {
  isLoggedIn = true;
  files = new Array<FileViewModel>();

  constructor(private fileService: FileService) {}

  async ngOnInit() {
    await this.loadFiles();
  }

  async loadFiles() {
    this.files = await this.fileService.getAllFiles();
    this.files.forEach((file) => {
      file.date = DateTime.fromISO(file.date.toString());
    });
  }

  async downloadFile(file: FileViewModel) {
    let blob = await this.fileService.downloadFile(file.id);
    let filename = file.name + file.fileType;
    let url = window.URL.createObjectURL(blob);
    let a = window.document.createElement('a');

    a.href = url;
    a.download = filename;

    a.click();
    window.URL.revokeObjectURL(url);
  }

  async deleteFile(file: FileViewModel) {
    if (file.deleteButtonClicked) {
      await this.fileService.deleteFile(file.id);
      await this.loadFiles();
    } else {
      file.deleteButtonClicked = true;
    }
  }
}
