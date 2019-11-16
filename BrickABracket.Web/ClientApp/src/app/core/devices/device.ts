export interface Device {
    connected: boolean;
    connection: string;
    batteryLevel: number;
    brickName: string;
    program: string;
    programs: Array<string>;
}
export class DeviceMetadata {
    device: Device;
    role: string;
    connectionString: string;
    program: string;
    deviceType: string;
    programs: Array<string>;
}
export enum DeviceRole {
    None = 0,
    // tslint:disable:no-bitwise
    StatusProvider = 1 << 0,
    StatusFollower = 1 << 1,
    ScoreProvider = 1 << 2,
    StartButton = StatusProvider | StatusFollower,
    Timer = StatusFollower | ScoreProvider,
    All = StatusFollower | StatusProvider | ScoreProvider
    // tslint:enable:no-bitwise
}
export interface DeviceOptions {
    deviceType: string;
    ports: Array<string>;
}
