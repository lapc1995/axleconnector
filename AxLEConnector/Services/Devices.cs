using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AxLEConnector.Helpers;
using OpenMovement.AxLE.Comms.Exceptions;
using OpenMovement.AxLE.Comms.Interfaces;
using OpenMovement.AxLE.Comms.Values;

namespace AxLEConnector.Services
{
    public sealed class Devices
    {

        private static readonly Lazy<Devices> lazy =
            new Lazy<Devices>(() => new Devices());

        public static Devices Instance { get { return lazy.Value; } }

        private List<string> _serials;
        private Dictionary<string, IAxLE> _devices;
        private Dictionary<string, EventHandler<AccBlock>> _deviceEventHandlers;
        private List<string> _devicesToBeConnected;

        public string Activity { set; get; }

        public IAxLEManager manager;

        public int Rate { set; get; }
        public int Range { set; get; }

        public string SelectedDevice { set; get; }

        public StreamFrequency StreamFrequency { set; get; }

        public event EventHandler StartScanning;
        public event EventHandler<DeviceConnectedEventArgs> DeviceConnected;
        public event EventHandler FinishedScanning;
        public event EventHandler FinishedWritting;
        public event EventHandler AllDevicesStreaming;
        public event EventHandler PreparingDevices;
        public event EventHandler InitiateStreamStop;
        public event EventHandler DeviceDisconnected;
        public event EventHandler<DeviceEventArgs> DeviceNotInRange;
        public event EventHandler<DeviceErrorEventArgs> DeviceError;

        private Devices()
        {
            _serials = new List<string>();
            _devices = new Dictionary<string, IAxLE>();
            _deviceEventHandlers = new Dictionary<string, EventHandler<AccBlock>>();
            StreamFrequency = StreamFrequency.HIGH;
        }

        public void AddSensorSerial(string serial)
        {
            _serials.Add(serial);
        }

        public List<string> GetSerials()
        {
            return new List<string>(_serials);
        }

        public IAxLE GetSensor(string serial) 
        {
            return _devices[serial];
        }

        public void AddSensor(IAxLE sensor)
        {
            _devices.Add(sensor.SerialNumber, sensor);
        }

        public List<IAxLE> GetAllSensors()
        {
            return _devices.Values.ToList();
        }

        public void SetDeviceAccelerometerEventHandler(string serial, EventHandler<AccBlock> handler)
        {
            _devices[serial].AccelerometerStream += handler;
            _deviceEventHandlers.Add(serial, handler);
        }

        public void RemoveDeviceAccelerometerEventHandler(string serial)
        {
            _devices[serial].AccelerometerStream -= _deviceEventHandlers[serial];
            _deviceEventHandlers.Remove(serial);
        }

        public void StartAccelerometers()
        {
            PreparingDevices?.Invoke(this, EventArgs.Empty);
            _devices.Keys.ToList().ForEach(k =>_devices[k].StartAccelerometerStream((int)StreamFrequency));
        }

        public void StopAccelerometers()
        {
            _devices.Keys.ToList().ForEach(k => _devices[k].StopAccelerometerStream());
        }

        public void Connect(List<string> serials)
        {
            Task.Run( async () =>
              {
                  if (manager == null)
                      manager = new GetAxLEManager().Get();

                  _devicesToBeConnected = serials;
                  manager.DeviceFound += DeviceFoundHandler;
                  StartScanning?.Invoke(this, EventArgs.Empty);

                  manager.SwitchToHighPowerScan();
                  await manager.StartScan();
              });
        }

        public void Connect()
        {
            Connect(GetSerials());
        }

        public void Connect(string serial)
        {
            Connect(new List<string>{ serial });
        }

        private void DeviceFoundHandler(object sender, string serial)
        {
            Task.Run(async () =>
            {
                try
                {
                    if (_devicesToBeConnected.Contains(serial))
                    {
                        _devicesToBeConnected.Remove(serial);
                        await manager.StopScan();
                        IAxLE device = await manager.ConnectDevice(serial);

                        await device.ResetPassword();
                        var pass = device.SerialNumber.Substring(device.SerialNumber.Length - 6);
                        if (await device.Authenticate(pass))
                        {
                            await device.UpdateDeviceState();
                            AddSensor(device);
                            DeviceConnected?.Invoke(this, new DeviceConnectedEventArgs(serial));
                        }

                        if (_devicesToBeConnected.Count != 0)
                            await manager.StartScan();
                        else
                        {
                            await manager.SwitchToNormalMode();
                            FinishedScanning?.Invoke(this, EventArgs.Empty);
                        }
                    }
                }
                catch (DeviceNotInRangeException)
                {
                    await manager.StopScan();
                    manager.DeviceFound -= DeviceFoundHandler;
                    await manager.SwitchToNormalMode();
                    DeviceNotInRange?.Invoke(this, new DeviceEventArgs(serial));
                }
                catch (Exception ex)
                {
                    await manager.StopScan();
                    manager.DeviceFound -= DeviceFoundHandler;
                    await manager.SwitchToNormalMode();
                    DeviceError?.Invoke(this, new DeviceErrorEventArgs(serial, ex.ToString()));
                }
            });
        }

        public void RetryConnecting(string serial)
        {
            _devices.Remove(serial);
            Task.Run(() => Connect(serial));
        }

        public void RetryConnectingAll()
        {
            DisconnectAll();
            _devices.Clear();
            Connect();
        }

        public void Disconnect(string serial)
        {
            Task.Run(async () =>
            {
                await manager.DisconnectDevice(_devices[serial]);
                _devices.Remove(serial);
                DeviceDisconnected?.Invoke(this, new DeviceEventArgs(serial));
            });
        }

        public void DisconnectAll()
        {
            foreach (string serial in _devices.Keys)
                Disconnect(serial);
            _serials.Clear();
            _devices.Clear();
        }

        public int GetBattery(string serial)
        {
            var asyncTask = Task.Run(async () => await _devices[serial].UpdateDeviceState());
            asyncTask.Wait();
            return _devices[serial].Battery;
        }

        public void SendFinishedWrittingEvent()
        {
            FinishedWritting?.Invoke(this, EventArgs.Empty);
        }

        public void SendAllDevicesStreamingEvent()
        {
            AllDevicesStreaming?.Invoke(this, EventArgs.Empty);
        }

        public void SendInitiateStreamStopEvent()
        {
            InitiateStreamStop?.Invoke(this, EventArgs.Empty);
        }

        public void SendDeviceNotInRangeException(string serial)
        {
            DeviceNotInRange?.Invoke(this, new DeviceEventArgs(serial));
        }

        public void SendDeviceError(string serial, string exception)
        {
            DeviceError?.Invoke(this, new DeviceErrorEventArgs(serial, exception));
        }

        public void VibrateDevice(string serial)
        {
            Task.Run(async () =>
            {
                await _devices[serial].VibrateDevice();
            });
        }

        public void VibrateAllDevices()
        {
            foreach(string serial in _devices.Keys)
                VibrateDevice(serial);
        }

        public void FlashDeviceLED(string serial)
        {
            Task.Run(async () =>
            {
                await _devices[serial].LEDFlash();
            });
        }

        public void FlashAllDevicesLEDs()
        {
            foreach (string serial in _devices.Keys)
                FlashDeviceLED(serial);
        }
    }
}