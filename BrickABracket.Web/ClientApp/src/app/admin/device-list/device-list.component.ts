import { Component, OnInit } from '@angular/core';
import { DeviceService } from 'src/app/core/devices/device.service';
import { Device, DeviceMetadata } from 'src/app/core/devices/device';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-device-list',
  templateUrl: './device-list.component.html',
  styleUrls: ['./device-list.component.css']
})
export class DeviceListComponent implements OnInit {

  private devices$: Observable<Array<DeviceMetadata>>;

  constructor(private devices: DeviceService) { }

  ngOnInit() {
    this.devices$ = this.devices.devices;
  }

  delete(connection: string) {
    this.devices.delete(connection);
  }

}
// TODO: Add button to find all devices
// TODO: Add role and program name to list
// TODO: If no role/program, indicate incomplete device setup
// TODO: Implement a popup warning on low battery?
// TODO: Background service to poll device status regularly (between matches)
