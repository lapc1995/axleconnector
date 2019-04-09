using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AxLEConnector.Services
{
    public class BatteryGetterService
    {
        private ConcurrentQueue<object> _queue;
        private readonly List<string> _serials;

        public BatteryGetterService(ref ConcurrentQueue<object> queue, List<string> serials)
        {
            _serials = serials;
            _queue = queue;
        }

        public void StoreCurrentBattery()
        {
            foreach(string serial in _serials)
            {
                int battery = Devices.Instance.GetBattery(serial);
                string activity = "Battery";
                _queue.Enqueue(new { activity, serial, battery });
            }
        }
    }
}
