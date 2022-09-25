import { DateTime } from 'luxon';

export class FileViewModel {
  public id: number = 0;
  public name: string = '';
  public fileType: string = '';
  public mimeType: string = '';
  public date: DateTime = DateTime.local();
  public deleteButtonClicked = false;
}
