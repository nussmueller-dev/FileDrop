import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {lastValueFrom} from 'rxjs';
import {environment} from 'src/environments/environment';
import {UserBindingModel} from './../models/user-binding-model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private http: HttpClient) {
  }

  getUsersCount() {
    return lastValueFrom(
      this.http.get<number>(environment.BACKENDURL + 'users/count')
    );
  }

  loadQrLoginToken() {
    return lastValueFrom(
      this.http.get<string>(environment.BACKENDURL + 'users/qrlogin-code')
    );
  }

  acceptQrLogin(token: string, sessionVallidForHours: number, authToken: string) {
    return lastValueFrom(
      this.http.post<void>(environment.BACKENDURL + 'users/accept-qrlogin', {token, sessionVallidForHours}, {
        headers: {
          Authorization: `Bearer ${authToken}`,
        }
      })
    );
  }

  loginUsingQr(token: string){
    return lastValueFrom(
      this.http.post<string>(
        environment.BACKENDURL + 'users/login/qr',
        {token}
      )
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
