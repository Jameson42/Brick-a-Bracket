import { Component, OnInit } from '@angular/core';
import { DeviceService } from 'src/app/core/devices/device.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { Device, DeviceMetadata } from 'src/app/core/devices/device';
import { create } from 'domain';

@Component({
  selector: 'app-device',
  templateUrl: './device.component.html',
  styleUrls: ['./device.component.css']
})
export class DeviceComponent implements OnInit {

  private device$: Observable<DeviceMetadata>;
  private isNew: boolean;

  constructor(
    private devices: DeviceService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {
    this.route.url.subscribe(e => {
      const path = e[e.length - 1].path;
      this.isNew = path === 'new';
      if (this.isNew) {
        this.device$ = Observable.create(observer => {
          observer.next(new DeviceMetadata());
          observer.complete();
        });
      } else {
        this.device$ = this.devices.get(path);
      }
    });
  }

  save(deviceData:DeviceMetadata, type:string) {
    if (this.isNew) {
      this.create(deviceData.connectionString, deviceData.program, type, deviceData.role);
    } else {
      this.setRole(deviceData.connectionString, deviceData.role);
      this.setProgram(deviceData.connectionString, deviceData.program);
    }
  }

  create(connection:string, program:string, type:string, role:string) {
    return this.devices.create(connection, program, type, role);
  }

  setRole(connection:string, role:string) {
    return this.devices.setRole(connection, role);
  }

  setProgram(connection:string, program:string) {
    return this.devices.setProgram(connection, program);
  }
}
