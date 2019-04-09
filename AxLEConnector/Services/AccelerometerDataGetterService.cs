using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using AxLEConnector.Abstractions;
using AxLEConnector.Helpers;

namespace AxLEConnector.Services
{
    public class AccelerometerDataGetterService<T> where T : AbstractRecordingParameters, new()
    {
        private readonly ConcurrentQueue<object> _receivedData;
        private List<T> _recordingParameters;
        private readonly List<string> _serials;

        private List<string> _streamingDevices;

        public AccelerometerDataGetterService(ref ConcurrentQueue<object> receivedData, List<string> serials, string activity)
        {
            _receivedData = receivedData;
            _recordingParameters = new List<T>();
            _serials = serials;

            foreach (string serial in _serials)
            {
                T rp = new T();
                rp.SetParameters(ref _receivedData, serial, activity);
                rp.DeviceStreaming += HandlerDeviceStreaming;

                Devices.Instance.SetDeviceAccelerometerEventHandler(serial, rp.HandleAccelerometerStream);
                _recordingParameters.Add(rp);
            }
        }

        void HandlerDeviceStreaming(object sender, DeviceStreamingEventArgs e)
        {
            AddStreamingDevice(e.Serial);
            ((T)sender).DeviceStreaming -= HandlerDeviceStreaming;
        }


        public AccelerometerDataGetterService(ref ConcurrentQueue<object> receivedData, String serial, string activity) : this(ref receivedData, new List<string>{serial}, activity){}

        public void StartAccelerometerStream()
        {
            Devices.Instance.StartAccelerometers();
        }

        public void InitiateStreamStopping()
        {
            Devices.Instance.SendInitiateStreamStopEvent();
            foreach (T parameters in _recordingParameters)
                parameters.Stop = true;

        }

        public void StopAccelerometerStream()
        {
            foreach (string serial in _serials)
            {
                Devices.Instance.GetSensor(serial).StopAccelerometerStream();
                Devices.Instance.RemoveDeviceAccelerometerEventHandler(serial);
            }
        }

        public void StartRecordingStream()
        { 
            foreach (T rp in _recordingParameters)
            {
                rp.Recording = true;
            }
        }

        public void AddStreamingDevice(string serial)
        {
            if (_streamingDevices == null)
                _streamingDevices = new List<string>();

            if (_streamingDevices.Contains(serial))
                throw (new Exception("Something went wrong, device " + serial + " started streaming again!!"));
                            
            _streamingDevices.Add(serial);
            if (_streamingDevices.Count == _serials.Count)
            {
                StartRecordingStream();
                Devices.Instance.SendAllDevicesStreamingEvent();
            }
        }
    }
}
