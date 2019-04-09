namespace AxLEConnector.Helpers
{
    public enum MessageType
    {
        START_SCANNING,
        DEVICE_CONNECTED, 
        FINISHED_SCANNING, 
        EXCEPTION_DEVICE_NOT_IN_RANGE, 
        EXCEPTION,
        STOP_RECORDING, 
        FINISHED_WRITING,
        PREPARING_DEVICES,
        DEVICE_STREAMING,
        ALL_DEVICES_STREAMING,
        DEVICE_DISCONNECTED
    }
}
