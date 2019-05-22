import { Component, OnInit } from '@angular/core';
import { DeviceService } from 'src/app/core/devices/device.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Observer } from 'rxjs';
import { Device, DeviceMetadata } from 'src/app/core/devices/device';
import { tap, switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-device',
  templateUrl: './device.component.html',
  styleUrls: ['./device.component.css']
})
export class DeviceComponent implements OnInit {

  private device$: Observable<DeviceMetadata>;
  private isNew: boolean;
  private connection: string;

  constructor(
    private devices: DeviceService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {
    this.device$ = this.route.paramMap.pipe(
      tap(params => {
        this.connection = params.get('id');
        this.isNew = !this.connection;
      }),
      switchMap(_ => {
        if (this.isNew) {
          return new Observable<DeviceMetadata>(
            (observer: Observer<DeviceMetadata>)  => {
            observer.next(new DeviceMetadata());
            observer.complete();
          });
        }
        return this.devices.get(this.connection);
      }),
      tap(device => console.log(device)),
    );
  }

  async save(deviceData: DeviceMetadata, type: string) {
    if (this.isNew) {
      await this.create(deviceData);
    } else {
      await this.setRole(deviceData.connectionString, deviceData.role);
      await this.setProgram(deviceData.connectionString, deviceData.program);
    }
    return this.router.navigate(['../'], { relativeTo: this.route });
  }

  create(deviceData: DeviceMetadata) {
    return this.devices.create(deviceData);
  }

  setRole(connection: string, role: string) {
    return this.devices.setRole(connection, role);
  }

  setProgram(connection: string, program: string) {
    return this.devices.setProgram(connection, program);
  }
}
