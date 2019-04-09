using System;
namespace AxLEConnector.Helpers
{
    public class DeviceStreamingEventArgs : EventArgs
    {
        public string Serial { get; set; }

        public DeviceStreamingEventArgs() { }

        public DeviceStreamingEventArgs(string serial)
        {
            Serial = serial;
        }
    }
}
