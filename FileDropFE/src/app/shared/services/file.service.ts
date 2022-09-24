import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class FileService {
  constructor(private http: HttpClient) { }

  uploadFile(file: File): Observable<any> {
    var formData: any = new FormData();
    formData.append("file", file);

    return this.http.post(environment.BACKENDURL + 'files/upload', formData, {
      reportProgress: true,
      observe: 'events'
    })
  }
}
