import {Component, OnDestroy, OnInit} from '@angular/core';
import * as _ from 'lodash';
import { DateTime } from 'luxon';
import { FileViewModel } from 'src/app/shared/models/file-view-model';
import { SignalrConnection } from 'src/app/shared/services/util/SignalrConnection';
import { environment } from 'src/environments/environment';
import { UserBindingModel } from './../../shared/models/user-binding-model';
import { FileService } from './../../shared/services/file.service';
import { UserService } from './../../shared/services/user.service';

@Component({
  selector: 'app-overview',
  templateUrl: './overview.component.html',
  styleUrls: ['./overview.component.scss'],
})
export class OverviewComponent implements OnInit, OnDestroy {
  private signalrConnection: SignalrConnection;
  isLoggedIn = false;
  token: string = '';
  files = new Array<FileViewModel>();
  usersCount = 1;

  load: Function = () => {
    this.loadFiles();
  };

  loadFiles = async () => {
    this.files = await this.fileService.getAllFiles(this.token);
    this.files.forEach((file) => {
      file.date = DateTime.fromISO(file.date.toString());
    });

    this.files = _.orderBy(this.files, ['date'], ['desc']);
  };

  constructor(
    private fileService: FileService,
    private userService: UserService
  ) {
    this.signalrConnection = new SignalrConnection(this.loadFiles);
  }

  async ngOnInit() {
    let token = localStorage.getItem('token');

    this.usersCount = await this.userService.getUsersCount();

    if (token) {
      this.token = token;
      try {
        await this.fileService.getAllFiles(token);
        await this.startSignalrRConnection();
        this.isLoggedIn = true;
      } catch {
        localStorage.removeItem('token');
        this.isLoggedIn = false;
      }
    } else {
      this.isLoggedIn = false;
    }
  }

  ngOnDestroy() {
    this.signalrConnection.stop();
  }

  async startSignalrRConnection() {
    await this.signalrConnection.start(environment.BACKENDURL + 'hubs/upload');
    this.signalrConnection.addEvent('NewUpload', this.loadFiles);
    this.signalrConnection.addEvent('Deleted', this.loadFiles);
  }

  async login(bindingModel: UserBindingModel) {
    if (this.usersCount === 1) {
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

    if(!this.token){
      return;
    }

    localStorage.setItem('token', this.token);
    await this.startSignalrRConnection();
    this.isLoggedIn = true;
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
