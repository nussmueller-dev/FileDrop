import {Component, EventEmitter, Input, Output} from '@angular/core';
import { UserBindingModel } from './../../shared/models/user-binding-model';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.scss'],
})
export class LoginFormComponent {
  @Input() isRegister: boolean = false;
  @Output() loginChange = new EventEmitter<UserBindingModel>();
  bindingModel = new UserBindingModel('', '', false);

  constructor() {}

  login() {
    this.loginChange.emit(this.bindingModel);
  }
}
