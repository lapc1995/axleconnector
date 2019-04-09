namespace AxLEConnector.Models
{
    public class AccelerometerData
    {

        public uint Id { private set; get; }
        public int Sample { private set; get; }
        public string Activity { private set; get; }
        public string Device { private set; get; }
        public long SampleReceivedTimeUnixMilliseconds { private set; get; }
        public uint TimestampRaw { private set; get; }
        public double RecordingTimeSeconds { private set; get; }
        public long TimestampUnixMilliseconds { private set; get; }
        public double X { private set; get; }
        public double Y { private set; get; }
        public double Z { private set; get; }

        public AccelerometerData(uint id,
                                 int sample,
                                 string activity,
                                 string device,
                                 long sampleReceivedTimeUnixMilliseconds,
                                 uint timestampRaw,
                                 double recordingTimeSeconds,
                                 long timestampUnixMilliseconds,
                                 double x,
                                 double y,
                                 double z)
        {
            Id = id;
            Sample = sample;
            Activity = activity;
            Device = device;
            SampleReceivedTimeUnixMilliseconds = sampleReceivedTimeUnixMilliseconds;
            TimestampRaw = timestampRaw;
            RecordingTimeSeconds = recordingTimeSeconds;
            TimestampUnixMilliseconds = timestampUnixMilliseconds;
            X = x;
            Y = y;
            Z = z;
        }

    }
}
