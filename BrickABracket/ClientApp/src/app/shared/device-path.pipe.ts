import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'devicePath'
})
export class DevicePathPipe implements PipeTransform {

  transform(value: string): string {
    return value.replace(/\//g, '.');
  }

}
