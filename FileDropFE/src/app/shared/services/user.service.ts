import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { lastValueFrom } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UserBindingModel } from './../models/user-binding-model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private http: HttpClient) {}

  getUsersCount() {
    return lastValueFrom(
      this.http.get<number>(environment.BACKENDURL + 'users/count')
    );
  }

  login(bindingModel: UserBindingModel) {
    return lastValueFrom(
      this.http.post<string>(
        environment.BACKENDURL + 'users/login',
        bindingModel
      )
    );
  }

  register(bindingModel: UserBindingModel) {
    return lastValueFrom(
      this.http.post<string>(
        environment.BACKENDURL + 'users/register',
        bindingModel
      )
    );
  }
}
