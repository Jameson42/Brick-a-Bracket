import { Component, OnInit } from '@angular/core';
import { DeviceService } from 'src/app/core/devices/device.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Observable, Observer } from 'rxjs';
import { Device, DeviceMetadata, DeviceOptions, DeviceRole } from 'src/app/core/devices/device';
import { tap, switchMap, filter, map } from 'rxjs/operators';

@Component({
  selector: 'app-device',
  templateUrl: './device.component.html',
  styleUrls: ['./device.component.css']
})
export class DeviceComponent implements OnInit {

  private device$: Observable<DeviceMetadata>;
  private deviceOptions$: Observable<Array<DeviceOptions>>;
  private connectionStrings$: Observable<Array<string>>;
  private isNew: boolean;
  private connection: string;
  private deviceMetadata: DeviceMetadata;

  constructor(
    private devices: DeviceService,
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {
    this.device$ = this.route.paramMap.pipe(
      tap(params => {
        this.connection = params.get('id');
        if (this.connection) {
          this.connection = this.connection.replace(/\./g, '/');
        }
        this.isNew = !this.connection;
      }),
      switchMap(_ => {
        if (this.isNew) {
          return new Observable<DeviceMetadata>(
            (observer: Observer<DeviceMetadata>) => {
              observer.next(new DeviceMetadata());
              observer.complete();
            });
        }
        return this.devices.get(this.connection);
      }),
      tap(d => this.deviceMetadata = d),
      tap(d => this.changeDeviceType(d.deviceType)),
    );
    this.deviceOptions$ = this.devices.getDeviceOptions();
  }

  async save(deviceData: DeviceMetadata) {
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

  setRole(connection: string, role: DeviceRole) {
    return this.devices.setRole(connection, role);
  }

  setProgram(connection: string, program: string) {
    return this.devices.setProgram(connection, program);
  }

  // TODO: On change of each form element, act on that change and update
  // observables for other elements
  changeDeviceType(deviceType: string) {
    // Device Type updated, update available connection strings
    this.deviceMetadata.deviceType = deviceType;
    this.connectionStrings$ = this.deviceOptions$.pipe(
      filter(o => o != null),
      map(o => o.find(d => d.deviceType === deviceType)),
      filter(o => o != null),
      map(o => o.ports)
    );
  }
  async changeConnectionString(connectionString: string) {
    // Connection String selected, get Program list
    this.isNew = false;
    this.deviceMetadata.connectionString = connectionString;
    await this.create(this.deviceMetadata);
    this.router.navigate(['../' + connectionString], { relativeTo: this.route });
  }
  changeDeviceRole(deviceRole: DeviceRole) {
    // Device Role updated
    this.deviceMetadata.role = deviceRole;
  }
  changeProgram(program: string) {
    // Program updated
    this.deviceMetadata.program = program;
  }
}
