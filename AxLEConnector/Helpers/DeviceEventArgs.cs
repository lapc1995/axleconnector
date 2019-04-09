using System;
namespace AxLEConnector.Helpers
{
    public class DeviceEventArgs : EventArgs
    {
        public string Serial { set; get; }

        public DeviceEventArgs() {}

        public DeviceEventArgs(string serial)
        {
            Serial = serial;
        }
    }
}
