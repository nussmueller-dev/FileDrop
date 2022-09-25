import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { lastValueFrom, Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { FileViewModel } from './../models/file-view-model';

@Injectable({
  providedIn: 'root',
})
export class FileService {
  constructor(private http: HttpClient) {}

  getAllFiles() {
    return lastValueFrom(
      this.http.get<Array<FileViewModel>>(environment.BACKENDURL + 'files')
    );
  }

  uploadFile(file: File): Observable<any> {
    var formData: any = new FormData();
    formData.append('file', file);

    return this.http.post(environment.BACKENDURL + 'files/upload', formData, {
      reportProgress: true,
      observe: 'events',
    });
  }

  async downloadFile(id: number) {
    return lastValueFrom(
      this.http.get<Blob>(environment.BACKENDURL + `files/${id}/download`, {
        responseType: 'blob' as 'json',
      })
    );
  }

  async deleteFile(id: number) {
    return lastValueFrom(
      this.http.delete(environment.BACKENDURL + `files/${id}`)
    );
  }
}
