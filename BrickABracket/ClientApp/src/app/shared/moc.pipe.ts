import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'moc'
})
export class MocPipe implements PipeTransform {

  transform(value: any, args?: any): any {
    return null;
  }

}
