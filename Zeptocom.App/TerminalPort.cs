using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Collections.Concurrent;
using System.IO;

namespace Zeptocom.App
{
    public class TerminalPort : IDisposable
    {
        public record PortSettings(string Name, int baudRate, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One, 
                                   Handshake handshake = Handshake.None, bool echo = false);

        private SerialPort _serialPort = null;
        private bool _shouldStop = false;
        private Thread _readThread = null;
        private bool disposedValue;

        private ConcurrentQueue<char> _writeQueue;
        private object _writeLock = new object();
        private TerminalScreen _screen = null;
        readonly PortSettings _portSettings;

        public TerminalPort(PortSettings settings, TerminalScreen screen)
        {
            _screen = screen;



            // Create a new SerialPort object with default settings.
            _serialPort = new SerialPort();

            // Allow the user to set the appropriate properties.
            _portSettings = settings;
            setPortSetting(_portSettings);

            _serialPort.RtsEnable = true;
            _serialPort.DtrEnable = true;

            // Set the read/write timeouts
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;

            _writeQueue = new ConcurrentQueue<char>();
        }

        private void setPortSetting(PortSettings settings)
        {
            _serialPort.PortName = settings.Name;
            _serialPort.BaudRate = settings.baudRate;
            _serialPort.Handshake = settings.handshake;
            _serialPort.Parity = settings.parity;
            _serialPort.DataBits = settings.dataBits;
            _serialPort.StopBits = settings.stopBits;
        }

        public void Start()
        {
            _serialPort.Open();
            _shouldStop = false;

            _readThread = new Thread(WorkerThread);
            _readThread.Start();
        }
        public void Stop(bool wait = true)
        {
            _shouldStop = true;
            if (wait)
            {
                _readThread.Join();
                _readThread = null;
            }

            _serialPort.Close();
        }

        public void Write(string str)
        {
            lock (_writeLock)
            {
                var arr = str.ToCharArray();

                foreach (var ch in arr)
                {
                    _writeQueue.Enqueue(ch);
                }
            }

        }
        public void Write2(string str)
        {
            lock (_writeLock)
            {
                int freeBytesCount = _serialPort.WriteBufferSize - _serialPort.BytesToWrite;

                if (freeBytesCount > 0)
                {
                    var arry = ASCIIEncoding.ASCII.GetBytes(str);

                    int len = (freeBytesCount > arry.Length) ? arry.Length : freeBytesCount;

                    _serialPort.Write(arry, 0, len);
                }
            }
        }

        const int READ_HANDLING_BUFFER = 1024;
        private void WorkerThread()
        {
            while (!_shouldStop)
            {
                try
                {
                    if (_serialPort.BytesToRead > 0)
                    {
                        int maxRead = (_serialPort.BytesToRead > READ_HANDLING_BUFFER) ? READ_HANDLING_BUFFER : _serialPort.BytesToRead;

                        byte[] buf = new byte[maxRead];

                        var count = _serialPort.Read(buf, 0, maxRead);

                        var message = ASCIIEncoding.ASCII.GetString(buf);
                        _screen.Write(message);
                        Thread.Yield();
                    }

                    if (_writeQueue.Count() > 0 && _serialPort.BytesToWrite < _serialPort.WriteBufferSize)
                    {
                        int bytesCount = _serialPort.WriteBufferSize - _serialPort.BytesToWrite;
                        char ch;
                        StringBuilder sb = new StringBuilder();
                        while (_writeQueue.TryDequeue(out ch))
                        {
                            if (_shouldStop)
                                break;

                            sb.Append(ch);
                            bytesCount--;

                            if (bytesCount == 0)
                                break;
                            
                            Thread.Yield();
                        }

                        var str = sb.ToString();
                        if (_portSettings.echo)
                        {
                            _screen.WriteLine(str);
                        }

                        _serialPort.Write(str);

                    }

                }
                catch (TimeoutException) { }
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects)
                    _serialPort.Close();
                    _serialPort.Dispose();
                    _serialPort = null;

                    _shouldStop = true;
                    _readThread.Join();
                    _readThread = null;
                }


                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TerminalPort()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
