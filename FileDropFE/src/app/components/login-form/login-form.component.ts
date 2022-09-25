import { Component, EventEmitter, Output } from '@angular/core';
import { UserBindingModel } from './../../shared/models/user-binding-model';

@Component({
  selector: 'app-login-form',
  templateUrl: './login-form.component.html',
  styleUrls: ['./login-form.component.scss'],
})
export class LoginFormComponent {
  @Output() loginChange = new EventEmitter<UserBindingModel>();
  bindingModel = new UserBindingModel('', '');

  constructor() {}

  login() {
    this.loginChange.emit(this.bindingModel);
  }
}
