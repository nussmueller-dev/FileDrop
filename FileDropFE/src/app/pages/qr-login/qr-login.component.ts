import {Component, OnDestroy, OnInit} from '@angular/core';
import {Router} from "@angular/router";
import {environment} from "../../../environments/environment";
import {UserService} from "../../shared/services/user.service";
import {SignalrConnection} from "../../shared/services/util/SignalrConnection";

@Component({
  selector: 'app-qr-login',
  templateUrl: './qr-login.component.html',
  styleUrls: ['./qr-login.component.scss']
})
export class QrLoginComponent implements OnInit, OnDestroy {
  private signalrConnection: SignalrConnection;
  qrCodeUrl: string = '';
  token: string = '';

  constructor(
    private userService: UserService,
    private router: Router
  ) {
    this.signalrConnection = new SignalrConnection();
  }

  async ngOnInit() {
    this.token = await this.userService.loadQrLoginToken();
    let backendUrl = environment.BACKENDURL + 'users/qr-code?url=';
    let acceptUrl = window.location.origin + '/accept-qrlogin?token=' + this.token;
    this.qrCodeUrl = backendUrl + acceptUrl;

    await this.startSignalrRConnection();
  }

  ngOnDestroy() {
    this.signalrConnection.stop();
  }

  async startSignalrRConnection() {
    await this.signalrConnection.start(environment.BACKENDURL + 'hubs/login');
    this.signalrConnection.addEvent('QrLoginAccepted', () => this.tryLogin());
  }

  async tryLogin(){
    let authToken = await this.userService.loginUsingQr(this.token);
    if(!authToken){
      return;
    }

    console.log(authToken);

    localStorage.setItem('token', authToken);
    this.router.navigate(['overview'])
  }

  cancel(){
    this.router.navigate(['overview']);
  }
}
