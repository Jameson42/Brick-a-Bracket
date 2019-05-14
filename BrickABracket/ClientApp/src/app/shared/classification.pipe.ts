import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'classification'
})
export class ClassificationPipe implements PipeTransform {

  transform(value: any, args?: any): any {
    return null;
  }

}
