import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'battery'
})
export class BatteryPipe implements PipeTransform {

  transform(value: number): string {
    if (!value) {
      return '';
    }
    if ( value > 87.5 ) {
      return 'fas fa-battery-full';
    }
    if (value > 62.5 ) {
      return 'fas fa-battery-three-quarters';
    }
    if ( value > 37.5 ) {
      return 'fas fa-battery-half';
    }
    if ( value > 12.5 ) {
      return 'fas fa-battery-quarter';
    }
    return 'fas fa-battery-empty';
  }

}
