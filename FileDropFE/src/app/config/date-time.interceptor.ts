// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-nocheck
import {
  HttpEventType,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import * as _ from 'lodash';
import { DateTime } from 'luxon';
import { tap } from 'rxjs/operators';

@Injectable()
export class DateTimeInterceptor implements HttpInterceptor {
  private readonly ISO_REGEX: RegExp =
    /^(19|20)\d\d-(0[1-9]|1[012])-([012]\d|3[01])T([01]\d|2[0-3]):([0-5]\d):([0-5]\d)$/;

  intercept(request: HttpRequest<any>, next: HttpHandler) {
    return next
      .handle(
        request.clone({
          body:
            request.body instanceof FormData
              ? request.body
              : this.convertDatesToStrings(_.cloneDeep(request.body)),
        })
      )
      .pipe(
        tap((event) => {
          if (event.type === HttpEventType.Response) {
            this.convertStringsToDates(event.body);
          }
        })
      );
  }

  convertStringsToDates(data: any) {
    if (data && _.isObject(data)) {
      Object.keys(data).forEach((key) => {
        if (_.isString(data[key]) && this.ISO_REGEX.test(data[key])) {
          data[key] = DateTime.fromISO(data[key]);
        } else if (typeof data[key] === 'object') {
          data[key] = this.convertStringsToDates(data[key]);
        } else if (_.isArray(data[data[key]])) {
          data[key] = this.convertStringsToDates(data[key]);
        }
      });
    }

    return data;
  }

  convertDatesToStrings(data: any) {
    if (data && _.isObject(data)) {
      Object.keys(data).forEach((key) => {
        if (data[key] instanceof DateTime && (data[key] as DateTime).isValid) {
          data[key] = (data[key] as DateTime).toISODate();
        } else if (typeof data[key] === 'object') {
          data[key] = this.convertDatesToStrings(data[key]);
        } else if (_.isArray(data[key])) {
          data[key] = this.convertDatesToStrings(data[key]);
        }
      });
    }

    return data;
  }
}
