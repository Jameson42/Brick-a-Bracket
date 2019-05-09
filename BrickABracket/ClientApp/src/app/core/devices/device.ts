export interface Device {
    Connected: boolean;
    Connection: string;
    BatteryLevel: number;
    BrickName: string;
    Program: string;
    Programs: Array<string>;
}
export interface DeviceMetadata {
    Device: Device;
    Role: DeviceRole;
    ConnectionString: string;
    Program: string;
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
