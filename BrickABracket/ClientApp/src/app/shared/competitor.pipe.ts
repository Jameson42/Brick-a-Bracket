import { Pipe, PipeTransform } from '@angular/core';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

import { CompetitorService, Competitor } from '@bab/core';

@Pipe({
  name: 'competitor'
})
export class CompetitorPipe implements PipeTransform {

  constructor(
    private competitors: CompetitorService
  ) {}

  transform(value: number, args?: any): Observable<Competitor> {
    return this.competitors.get(value).pipe(
      map(c => c || new Competitor())
    );
  }

}
