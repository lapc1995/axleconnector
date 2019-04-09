using System;
namespace AxLEConnector.Helpers
{
    public class DeviceErrorEventArgs : EventArgs
    {
        public string Serial { set; get; }
        public string Exception { set; get; }

        public DeviceErrorEventArgs() {}

        public DeviceErrorEventArgs(string serial, string exception)
        {
            Serial = serial;
            Exception = exception;
        }
    }
}
