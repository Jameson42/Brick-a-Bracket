import { Pipe, PipeTransform } from '@angular/core';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

import { CompetitorService } from '@bab/core';

@Pipe({
  name: 'competitor'
})
export class CompetitorPipe implements PipeTransform {

  constructor(
    private competitors: CompetitorService
  ) {}

  transform(value: number, args?: any): Observable<string> {
    return this.competitors.get(value).pipe(
      map(c => {
        if (c) {
          return c.name;
        }
        return '';
      })
    );
  }

}
