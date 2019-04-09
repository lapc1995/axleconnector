using System;
using AxLEConnector.Abstractions;
using AxLEConnector.Models;
using OpenMovement.AxLE.Comms.Values;

namespace AxLEConnector.Helpers
{
    public class RecordingAccParameters : AbstractRecordingParameters
    {
        private uint _firstTimestamp;
        private DateTime _firstDataRecorded;
        private uint _sampleId;
        private bool _firstTimestampSet;
        private uint _lastTimestamp;
        private int _nOverflows;

        public RecordingAccParameters()
        {
            _firstTimestampSet = false;
        }

        public override void HandlerBehaviour(object sender, AccBlock accBlock)
        {
            if (accBlock.Timestamp != 0)
            {
                if (Recording)
                {
                    DateTime receiveTime = DateTime.UtcNow;

                    if (!_firstTimestampSet)
                    {
                        _firstTimestamp = accBlock.Timestamp;
                        _firstDataRecorded = receiveTime.AddSeconds(-(1 / (float)accBlock.Rate * 25));
                        _firstTimestampSet = true;
                    }

                    if (Stop)
                        _activity = "Stop";

                    if (_lastTimestamp > accBlock.Timestamp)
                        _nOverflows++;

                    double lastSeconds = 0;
                    for (int i = 0; i < accBlock.Samples.Length; i++)
                    {
                        lastSeconds = i == 0 ? (accBlock.Timestamp + (16777216 * _nOverflows) - _firstTimestamp) / 32768.0 : lastSeconds + 1 / (float)accBlock.Rate;
                        _lastTimestamp = accBlock.Timestamp;

                        DateTime sampleTime = _firstDataRecorded.AddSeconds(lastSeconds);
                        DateTime endTime = DateTime.UtcNow;
                        queue.Enqueue(new AccelerometerData
                                (
                                    _sampleId,
                                    i,
                                    _activity,
                                    _serial,
                                    ((DateTimeOffset)receiveTime).ToUnixTimeMilliseconds(),
                                    accBlock.Timestamp,
                                    lastSeconds,
                                    ((DateTimeOffset)sampleTime).ToUnixTimeMilliseconds(),
                                    accBlock.Samples[i][0] / (32768.0 / accBlock.Range), // TODO: '16' is actually '24'
                                    accBlock.Samples[i][1] / (32768.0 / accBlock.Range), // TODO: '16' is actually '24'
                                    accBlock.Samples[i][2] / (32768.0 / accBlock.Range)  // TODO: '16' is actually '24'
                                ));
                        _sampleId++;
                    }
                }
            }
        }
    }
}
