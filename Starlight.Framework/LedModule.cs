namespace Starlight.Framework;

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enumeration;

public class LedModule : IDisposable
{
    private SerialPort _serialPort;
    private Queue<byte[]> _responseQueue = new();

    public string PortName => _serialPort.PortName;

    public const int Width = 9;
    public const int Height = 34;

    internal LedModule(string portName, int baudRate = 115200)
    {
        _serialPort = new SerialPort(portName, baudRate);
        _serialPort.Encoding = Encoding.UTF8;
    }
    
    public static LedModule[] Enumerate(ushort vid = 0x32AC, ushort pid = 0x0020)
    {
        if (OperatingSystem.IsWindows())
        {
            return new WmiSerialModuleEnumerator()
                .Enumerate(vid, pid)
                .Select(portName => new LedModule(portName))
                .ToArray();
        }
        else if (OperatingSystem.IsLinux())
        {
            throw new NotImplementedException();
        }
        else throw new PlatformNotSupportedException();
    }
    
    public bool Connect()
    {
        if (_serialPort.IsOpen) return false;
        
        try
        {
            _serialPort.DataReceived += SerialPort_DataReceived;
            _serialPort.Open();
            
            return true;
        }
        catch (Exception e)
        {
            throw new CommunicationException("Failed to connect to the LED module.", e);
        }
    }

    public bool Disconnect()
    {
        if (!_serialPort.IsOpen) return false;

        try
        {
            _serialPort.DataReceived -= SerialPort_DataReceived;
            _serialPort.Close();

            return true;
        }
        catch (Exception e)
        {
            throw new CommunicationException("Failed to disconnect from the LED module.", e);
        }
    }

    public async Task SendCommand(params byte[] data)
    {
        if (!_serialPort.IsOpen)
        {
            throw new CommunicationException("This operation requires an active connection to the LED module.");
        }
        
        await Task.Run(() =>
        {
            var buffer = MakeCommandBuffer(data);
            _serialPort.Write(buffer, 0, buffer.Length);
            _serialPort.BaseStream.Flush();
        });
    }
    
    public async Task<byte[]> ReadData(byte registerId)
    {
        await SendCommand(registerId);
        
        while (_responseQueue.Count <= 0)
        {
            await Task.Delay(1);
        }

        return _responseQueue.Dequeue();
    }

    public async Task<byte> GetBrightness()
        => (await ReadData(0x00))[0];

    public async Task SetBrightness(byte brightness)
    {
        if (brightness > 239) brightness = 239;
        await SendCommand(0x00, brightness);
    }

    public async Task SetFirmwarePattern(DefaultFirmwarePattern pattern, byte argument)
        => await SendCommand(0x01, (byte)pattern, argument);
    
    public async Task SetFirmwarePattern(DefaultFirmwarePattern pattern)
        => await SendCommand(0x01, (byte)pattern, 0);

    public async Task SetSleepStatus(bool sleep)
        => await SendCommand(0x03, (byte)(sleep ? 1 : 0));

    public async Task<bool> GetSleepStatus()
        => (await ReadData(0x03))[0] != 0;

    public async Task SetAnimationState(bool animate)
        => await SendCommand(0x04, (byte)(animate ? 1 : 0));

    public async Task<bool> GetAnimationState()
        => (await ReadData(0x04))[0] != 0;

    public async Task SetAnimationPeriod(ushort speed)
        => await SendCommand(0x1C, (byte)((speed & 0xFF00) >> 8), (byte)(speed & 0x00FF));

    public async Task SetColumn(byte x, byte[] pixels)
    {
        if (x > 8)
        {
            throw new CommunicationException("X was too large, maximum value is 8.");
        }
        
        if (pixels.Length != 34)
        {
            throw new CommunicationException($"Expected 39 pixel values, found {pixels.Length}.");
        }

        await SendCommand(new byte[] { 0x07, x }.Concat(pixels).ToArray());
    }

    public async Task FlushColumns()
        => await SendCommand(0x08);

    public async Task<ushort> GetAnimationPeriod()
    {
        var data = await ReadData(0x1C);
        return (ushort)((data[0] << 8) | data[1]);
    }
    
    private byte[] MakeCommandBuffer(params byte[] data)
        => new byte[] { 0x32, 0xAC }.Concat(data).ToArray();
    
    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        _responseQueue.Enqueue(
            Encoding.UTF8.GetBytes(
                _serialPort.ReadExisting()
            )
        );
    }

    public void Dispose()
    {
        Disconnect();
        _serialPort.Dispose();
    }
}