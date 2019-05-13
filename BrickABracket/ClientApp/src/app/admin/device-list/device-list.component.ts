import { Component, OnInit } from '@angular/core';
import { DeviceService } from 'src/app/core/devices/device.service';
import { Device } from 'src/app/core/devices/device';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.component.html',
  styleUrls: ['./device-list.component.css']
})
export class DeviceListComponent implements OnInit {

  private devices$: Observable<Array<Device>;

  constructor(private devices: DeviceService) { }

  ngOnInit() {
    this.devices$ = this.devices.devices;
  }

  delete(connection:string) {
    this.devices.delete(connection);
  }

}
