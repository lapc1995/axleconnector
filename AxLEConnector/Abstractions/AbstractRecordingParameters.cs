using System;
using System.Collections.Concurrent;
using AxLEConnector.Helpers;
using OpenMovement.AxLE.Comms.Exceptions;
using OpenMovement.AxLE.Comms.Values;

namespace AxLEConnector.Abstractions
{
    public abstract class AbstractRecordingParameters
    {
        protected ConcurrentQueue<object> queue;
        protected string _serial;
        protected string _activity;

        public bool Recording { set; get; }
        public bool Stop { set; get; }

        private bool _sentStreamCheck;

        public event EventHandler<DeviceStreamingEventArgs> DeviceStreaming;

        protected AbstractRecordingParameters(){}

        public void SetParameters(ref ConcurrentQueue<object> queue, string serial, string activity)
        {
            this.queue = queue;
            _serial = serial;
            _activity = activity;
            Recording = false;
            Stop = false;
        }

        public void HandleAccelerometerStream(object sender, AccBlock accBlock)
        {
            try
            {
                HandlerBehaviour(sender, accBlock);
                if (!_sentStreamCheck)
                {
                    DeviceStreaming?.Invoke(this, new DeviceStreamingEventArgs(_serial));
                    _sentStreamCheck = true;
                }
            }
            catch (DeviceNotInRangeException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public abstract void HandlerBehaviour(object sender, AccBlock accBlock);
    }
}
