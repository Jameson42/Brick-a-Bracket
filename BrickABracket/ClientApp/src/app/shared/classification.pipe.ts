import { Pipe, PipeTransform } from '@angular/core';
import { ClassificationService, Classification } from '@bab/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Pipe({
  name: 'classification'
})
export class ClassificationPipe implements PipeTransform {

  constructor(private classifications: ClassificationService) {}

  transform(value: number): Observable<Classification> {
    return this.classifications.get(value).pipe(
      map(c => c || new Classification())
    );
  }

}
