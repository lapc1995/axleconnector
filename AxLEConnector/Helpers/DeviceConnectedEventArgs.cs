using System;
namespace AxLEConnector.Helpers
{
    public class DeviceConnectedEventArgs : EventArgs
    {
        public string Serial { get; set; }

        public DeviceConnectedEventArgs() { }

        public DeviceConnectedEventArgs(string serial)
        {
            Serial = serial;
        }
    }
}
