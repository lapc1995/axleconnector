using System;
using OpenMovement.AxLE.Comms;
using OpenMovement.AxLE.Comms.Interfaces;
using Plugin.BLE;

namespace AxLEConnector.Helpers
{
    public class GetAxLEManager
    {
        public IAxLEManager Get()
        {
#if __ANDROID__
            //return new AxLEManager(new OpenMovement.AxLE.Comms.Bluetooth.Mobile.Android.BluetoothManager(CrossBluetoothLE.Current));
            return new AxLEManager(new OpenMovement.AxLE.Comms.Bluetooth.Mobile.Android.BluetoothManager());
#elif __IOS__
                return new AxLEManager(new OpenMovement.AxLE.Comms.Bluetooth.Mobile.BluetoothManager(CrossBluetoothLE.Current));
#endif
            throw new Exception("Platform not supported");
        }
    }
}
