import { Pipe, PipeTransform } from '@angular/core';
import { MocService, Moc } from '@bab/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Pipe({
  name: 'moc$'
})
export class MocPipe implements PipeTransform {

  constructor(
    private mocs: MocService
  ) {}

  transform(value: number): Observable<Moc> {
    return this.mocs.get(value).pipe(
      map(m => m || new Moc()),
    );
  }

}
