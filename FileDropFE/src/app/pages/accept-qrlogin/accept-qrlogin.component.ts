import { Component, OnInit } from '@angular/core';
import {ActivatedRoute, Router} from "@angular/router";
import {UserService} from "../../shared/services/user.service";
import {FileService} from "../../shared/services/file.service";

@Component({
  selector: 'app-accept-qrlogin',
  templateUrl: './accept-qrlogin.component.html',
  styleUrls: ['./accept-qrlogin.component.scss']
})
export class AcceptQrloginComponent implements OnInit {
  token: string = '';
  authToken: string = '';

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private userService: UserService,
    private fileService: FileService
  ) { }

  async ngOnInit() {
    this.token = this.route.snapshot.queryParams['token'];

    if(!this.token){
      this.cancel();
      return;
    }

    let authToken = localStorage.getItem('token');
    if (authToken) {
      this.authToken = authToken;
      try {
        await this.fileService.getAllFiles(authToken);
      } catch {
        localStorage.removeItem('token');
        this.cancel();
      }
    }else{
      this.cancel();
    }
  }

  async accept(sessionVallidForHours: number){
    await this.userService.acceptQrLogin(this.token, sessionVallidForHours, this.authToken);
    this.cancel();
  }

  cancel(){
    this.router.navigate(['']);
  }
}
