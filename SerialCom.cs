using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace Trigger250
{
    public class DataReceivedEventArgs : EventArgs
    {
        public string Data { get; }

        public DataReceivedEventArgs(string data)
        {
            Data = data;
        }
    }

    internal class SerialCom : IDisposable
    {
        private SerialPort _serialPort;
        private string data = null;

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        public SerialCom(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
        {
            _serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            _serialPort.DataReceived += OnDataReceived;
        }

        public void Open()
        {
            if (_serialPort != null && !_serialPort.IsOpen)
            {
                Debug.WriteLine("Attempting to connect to com port");
                _serialPort.Open();
            }
        }

        public void Close()
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
        }

        public void SendCommand(string command)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                data = null;
                _serialPort.WriteLine(command);
            }
            else
            {
                throw new InvalidOperationException("Serial port is not open.");
            }
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_serialPort != null)
            {
                data = _serialPort.ReadExisting();
                OnDataReceived(new DataReceivedEventArgs(data));
                Debug.WriteLine("here is your data" + data);
            }
        }

        protected virtual void OnDataReceived(DataReceivedEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        public void Dispose()
        {
            Close();
            if (_serialPort != null)
            {
                _serialPort.Dispose();
                _serialPort = null;
            }
        }

        public string Get()
        {
            return data;
        }
    }
}
