using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Timers;
using AxLEConnector.Abstractions;

namespace AxLEConnector.Services
{
    public class RecordAxLEAccelerometerService<T> where T : AbstractRecordingParameters, new()
    {
        private readonly ConcurrentQueue<object> _queue;
        private AccelerometerDataGetterService<T> _accelerometerDataGetterService;
        private FileWriterService _fileWritingService;
        private BatteryGetterService _batteryGetterService;
        private Timer _stopTimer;

        public RecordAxLEAccelerometerService(List<string> serials, string activity)
        {
            _queue = new ConcurrentQueue<object>();
            _accelerometerDataGetterService = new AccelerometerDataGetterService<T>(ref _queue, serials, activity);
            _fileWritingService = new FileWriterService(ref _queue);
            _batteryGetterService = new BatteryGetterService(ref _queue, serials);
        }

        public RecordAxLEAccelerometerService(string serial, string activity) : this(new List<string> {serial}, activity) { }

        public void StartRecording()
        {
            _batteryGetterService.StoreCurrentBattery();
            _fileWritingService.StartFetchingData();
            _accelerometerDataGetterService.StartAccelerometerStream();
        }

        public void StopRecording()
        {
            _accelerometerDataGetterService.InitiateStreamStopping();
            _stopTimer = new Timer(2000);
            _stopTimer.Elapsed += StopTimerHandler;
            _stopTimer.Enabled = true;
        }

        private void StopTimerHandler(object sender, ElapsedEventArgs e)
        {
            _stopTimer.Stop();
            _accelerometerDataGetterService.StopAccelerometerStream();
            _batteryGetterService.StoreCurrentBattery();
            _fileWritingService.StopFetchingData();
        }

    }
}
