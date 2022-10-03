import { Component, OnInit } from '@angular/core';
import * as _ from 'lodash';
import { DateTime } from 'luxon';
import { FileViewModel } from 'src/app/shared/models/file-view-model';
import { UserBindingModel } from './../../shared/models/user-binding-model';
import { FileService } from './../../shared/services/file.service';
import { UserService } from './../../shared/services/user.service';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss'],
})
export class OverviewComponent implements OnInit {
  isLoggedIn = false;
  token: string = '';
  files = new Array<FileViewModel>();
  usersCount = 0;

  constructor(
    private fileService: FileService,
    private userService: UserService
  ) {}

  async ngOnInit() {
    let token = localStorage.getItem('token');

    if (token) {
      this.token = token;
      await this.loadFiles();
      this.isLoggedIn = true;
    } else {
      this.isLoggedIn = false;
    }
  }

  async login(bindingModel: UserBindingModel) {
    if (this.usersCount === 0) {
      this.usersCount = await this.userService.getUsersCount();
    }

    if (bindingModel.username.length < 3) {
      alert('Username must be at least 3 characters long!');
      return;
    }

    if (this.usersCount === 0) {
      this.token =
        (await this.userService
          .register(bindingModel)
          .catch(() => alert('Wrong Credentials'))) ?? '';
    } else {
      this.token =
        (await this.userService
          .login(bindingModel)
          .catch(() => alert('Wrong Credentials'))) ?? '';
    }

    console.log(this.token);

    localStorage.setItem('token', this.token);
    await this.loadFiles();
    this.isLoggedIn = true;
  }

  async loadFiles() {
    this.files = await this.fileService.getAllFiles(this.token);
    this.files.forEach((file) => {
      file.date = DateTime.fromISO(file.date.toString());
    });

    this.files = _.orderBy(this.files, ['date'], ['desc']);
  }

  async downloadFile(file: FileViewModel) {
    let blob = await this.fileService.downloadFile(file.id, this.token);
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
      await this.fileService.deleteFile(file.id, this.token);
      await this.loadFiles();
    } else {
      file.deleteButtonClicked = true;
    }
  }
}
