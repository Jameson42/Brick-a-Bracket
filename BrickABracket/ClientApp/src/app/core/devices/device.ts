class Device {
    public Connected: boolean;
    public Connection: string;
    public BatteryLevel: number;
    public BrickName: string;
    public Program: string;
    public Programs: string[];
}
class DeviceMetadata {
    public Device: Device;
    public Role: DeviceRole;
    public ConnectionString: string;
    public Program: string;
}
enum DeviceRole {
    None = 0,
    StatusProvider = 1 << 0,
    StatusFollower = 1 << 1,
    ScoreProvider = 1 << 2,
    StartButton = StatusProvider | StatusFollower,
    Timer = StatusFollower | ScoreProvider,
    All = StatusFollower | StatusProvider | ScoreProvider
}