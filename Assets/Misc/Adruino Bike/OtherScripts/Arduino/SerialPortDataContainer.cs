using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using System.Threading;

public class SerialPortDataContainer
{
    private Exception _ThrownException = null;
    public SerialPortState State;
    int attempts = 0;
    private string _LastMessage;
    private Thread ReadingThread;
    private Timer TimeoutTimer;
    public int TimeoutTimeInSeconds = 1;
    private SerialPort _Port;
    private string _LastValue;
    public List<string> Allvalues;
    [Serializable]
    public class ArduinoDataEvent : UnityEvent<SerialPortDataContainer> { }
    public ArduinoDataEvent OnNewData = new ArduinoDataEvent();
    [Serializable]
    public class ArduinoDisconnectEvent : UnityEvent<string> { }
    public ArduinoDisconnectEvent OnDisconnect = new ArduinoDisconnectEvent();

    public SerialPort Port
    {
        get { return _Port; }
    }
    public string LastValue
    {
        get { return _LastValue; }
    }

    public SerialPortDataContainer(SerialPort port)
    {
        _Port = port;
        _LastValue = "0";
        Allvalues = new List<string>();
        ReadingThread = new Thread(new ThreadStart(() => ReadData(_Port)));
        ReadingThread.Start();
        State = SerialPortState.UNCONNECTED;
    }

    public bool Connect() {
        State = SerialPortState.CONNECTING;
        bool valueToReturn = true;
            try
            {
                _Port.Write(Settings.Instance.HANDSHAKE_SEND);
            }
            catch (TimeoutException) { 
            }
            attempts++;
            if (TimeoutTimer == null)
            {
                TimeoutTimer = new Timer(CheckConnectionTimeout, new AutoResetEvent(false), TimeoutTimeInSeconds * 1000, 1000);
            }
        return valueToReturn;
    }

    public void CheckConnectionTimeout(object stateInfo) {
        if (attempts > 3)
        {
            TimeoutTimer.Dispose();

            if (State == SerialPortState.CONNECTING)
            {
                State = SerialPortState.FAILED;
            }
        }
        else if(State != SerialPortState.CONNECTED) { 
            Connect(); 
        }
    }

    /*Thread*/
    public void ReadData(SerialPort port) {
        string message;
        while (true) {
            message = null;
            try
            {
                message = port.ReadLine();
                //port.BaseStream.Flush();
            }
            catch (TimeoutException) { }
            catch (Exception ex) {
                _ThrownException = ex;
                break;
            }
            if (message != null) {
                _LastMessage = message;
            }
        }
    }
    /*End thread*/

    /// <summary>
    /// Reads the data from the port.
    /// </summary>
    public void ReadLastMessage()
    {
        if (_LastMessage != null && _LastMessage != string.Empty) {
            if (Settings.Instance.DEBUGMSG)
            {
                GameConsole.Log(_LastMessage);
            }
            if (Settings.Instance.HANDSHAKE_RECIEVE.Contains(_LastMessage) || _LastMessage.Contains(Settings.Instance.HANDSHAKE_RECIEVE))
            {
                GameConsole.Log(Port.PortName + " connected.");
                Debug.Log(Port.PortName + " connected.");
                State = SerialPortState.CONNECTED;
                _LastMessage = null;
            }
        }
        if (_ThrownException != null)
        {
            lock (_ThrownException)
            {
                Debug.Log("exception");
                Exception temp = _ThrownException;
                _ThrownException = null;
                throw temp;
            }
        }
        switch(State){
            case SerialPortState.UNCONNECTED :
                if (Settings.Instance.HANDSHAKE)
                {
                    Connect();
                    GameConsole.Log(Port.PortName + " connecting...");
                    Debug.Log(Port.PortName + " connecting...");
                }
                else {
                    State = SerialPortState.CONNECTED;
                }
                break;
            case SerialPortState.CONNECTED :

            if (_LastMessage == null)
            {
                return;
            }
            lock (_LastMessage)
            {
                _LastValue = _LastMessage;
                _LastMessage = null;
            }
            Allvalues.Insert(0, LastValue);
            if (Allvalues.Count > Settings.Instance.TRACK_COUNT)
            {
                Allvalues.RemoveAt(Allvalues.Count - 1);
            }
            OnNewData.Invoke(this);
        break;
            case SerialPortState.FAILED :
        throw new Exception("Arduino failed to connect");
        }
    }

    public void ClosePort()
    {
        OnDisconnect.Invoke(Port.PortName);
        OnNewData.RemoveAllListeners();
        OnDisconnect.RemoveAllListeners();
        ReadingThread.Abort();
        Port.Close();
    }
}