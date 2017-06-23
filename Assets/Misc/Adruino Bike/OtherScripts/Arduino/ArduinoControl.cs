using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System;
using UnityEngine.Events;

/// <summary> Looks through all the serial ports, opens connections and then filters. Puts the raw values during Update into lastValues. 
/// Subscribe to the OnNewData UnityEvent to be notified when new data arrives, the event provides a SerialPortDataContainer.</summary>
public class ArduinoControl : MonoBehaviour
{
    private static ArduinoControl _Instance;

    List<SerialPortDataContainer> _Ports = new List<SerialPortDataContainer>();
    List<string> _AvailiblePorts = new List<string>();
    public static List<KeyValuePair<int, UnityAction>> QueueForArduino = new List<KeyValuePair<int, UnityAction>>();
    bool _IsQueuedToConnect = true;

    #region Static wrapping
    /// <summary>
    /// Registers for events at a port.
    /// </summary>
    /// <param name="arduinoName">Name of the arduino/COM port.</param>
    /// <param name="newData">The method to be called when there is new data.</param>
    /// <param name="Disconnect">The method to be called when the port closes.</param>
    public static void RegisterForEvents(string arduinoName, UnityAction<SerialPortDataContainer> newData, UnityAction<string> Disconnect)
    {
        _Instance._RegisterForEvents(arduinoName, newData, Disconnect);
    }
    /// <summary>
    /// Reserves a port for you. Returns name of reserved port. null if none availible.
    /// </summary>
    /// <returns>string name</returns>
    public static string ReserveArduino() {
        return _Instance._ReserveArduino();
    }
    /// <summary>
    /// Releases the port so it can be givven to an other class.
    /// </summary>
    /// <param name="arduinoName">Name of the arduino/COM port.</param>
    public static void ReleaseArduino(string arduinoName) {
        _Instance._ReleaseArduino(arduinoName);
    }
    /// <summary>
    /// Closes all current connections, clears the lists and then looks for ports again just like at start.
    /// </summary>
    public static void Reconnect() {
        _Instance._Reconnect();
    }

    public static List<string> GetConnectedArduinos() { return _Instance.GetConnectedPorts(); }
    #endregion

    /* On awake we check if there already is an instance of the class.
     * if not this will be set as the instance. 
     * If there is already an instance then this will remove itself.
     */
    void Awake()
    {
        if (_Instance != null)
        {
            GameObject.Destroy(gameObject);
        }
        else
        {
            _Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start() {
        UnityAction method = _Reconnect;
        GameConsole.RegisterCommand(method, "Reconnect");
        UnityAction < string > method2 = ConnectToCOMport;
        GameConsole.RegisterCommand(method2, "ConnectCom");
        method = ToggleArduinoDebugMessages;
        GameConsole.RegisterCommand(method, "msg");
        FetchPorts();
    }

    void ToggleArduinoDebugMessages()
    {
        Settings.Instance.DEBUGMSG = !Settings.Instance.DEBUGMSG;
    }

    private List<string> GetConnectedPorts()
    {
        List<string> valueToReturn = new List<string>();
        foreach (SerialPortDataContainer spdc in _Ports)
        {
            if (spdc.State == SerialPortState.CONNECTED)
            {
                valueToReturn.Add(spdc.Port.PortName);
            }
        }
        return valueToReturn;
    }

    /// <summary>
    /// Runs past all the COM ports on the pc that are in use and tries to open a connection with them,
    /// then attempts to read a line of data. If all of this succesfull the port is added to the list of availible ports.
    /// </summary>
    void FetchPorts() {
        foreach (string portName in SerialPort.GetPortNames())
        {
            if (FindPortByName("\\" + "\\" + "." + "\\" + portName) == null)
            {
                ConnectToCOMport(portName);
            }
        }
        _IsQueuedToConnect = false;
    }
    void ConnectToCOMport(string portName) {
        Debug.Log("Opening port: " + portName + " with bautRate " + Settings.Instance.BAUT_RATE);
        GameConsole.Log("Opening port: " + portName + " with bautRate " + Settings.Instance.BAUT_RATE);
        SerialPort port = new SerialPort("\\" + "\\" + "." + "\\" + portName, Settings.Instance.BAUT_RATE);
        try
        {
            if (!_Ports.Contains(new SerialPortDataContainer(port)))
            {
                port.Open();
                port.ReadTimeout = Settings.Instance.READ_TIMEOUT;
                port.WriteTimeout = Settings.Instance.WRITE_TIMEOUT;
                SerialPortDataContainer portdc = new SerialPortDataContainer(port);
                AddPort(portdc);
                if (QueueForArduino.Count > 0)
                {
                    QueueForArduino[0].Value.Invoke();
                    QueueForArduino.RemoveAt(0);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to open " + port.PortName + " Error: " + ex.ToString());
            GameConsole.Log("Failed to open " + port.PortName + " Error: " + ex.ToString());
        }
    }

    void AddPort(SerialPortDataContainer port)
    {
        _Ports.Add(port);
        _AvailiblePorts.Add(port.Port.PortName);
    }
    void RemovePort(SerialPortDataContainer port)
    {
        _Ports.Remove(port);
        port.ClosePort();
        _AvailiblePorts.Remove(port.Port.PortName);
    }

    /// <summary>
    /// Finds a port in the ports list by its name. null if its not found.
    /// </summary>
    /// <param name="name">The name of the port</param>
    /// <returns>SerialPortDataContainer</returns>
    SerialPortDataContainer FindPortByName(string name)
    {
        SerialPortDataContainer ValueToReturn = null;
        foreach (SerialPortDataContainer port in _Ports)
        {
            if(port.Port.PortName.Equals(name)){
                ValueToReturn = port;
            }
        }
        return ValueToReturn;
    }

    public void _RegisterForEvents(string arduinoName, UnityAction<SerialPortDataContainer> newData, UnityAction<string> disconnect)
    {
        SerialPortDataContainer port = FindPortByName(arduinoName);
        port.OnNewData.AddListener(newData);
        port.OnDisconnect.AddListener(disconnect);
    }

    public string _ReserveArduino() {
        string ValueToReturn = null;
        if (_AvailiblePorts.Count > 0) {
            ValueToReturn = FindPortByName(_AvailiblePorts[0]).Port.PortName;
        }
        _AvailiblePorts.Remove(ValueToReturn);
        return ValueToReturn;
    }

    public void _ReleaseArduino(string name) {
        SerialPortDataContainer temp = FindPortByName(name);
        if (temp != null)
        {
            temp.OnNewData.RemoveAllListeners();
            temp.OnDisconnect.RemoveAllListeners();
            _AvailiblePorts.Add(name);
            if (QueueForArduino.Count > 0)
            {
                QueueForArduino[0].Value.Invoke();
                QueueForArduino.RemoveAt(0);
            }
        }
    }

    public void _Reconnect() {
        foreach (SerialPortDataContainer port in _Ports)
        {
            port.ClosePort();
        }
        _Ports.Clear();
        _AvailiblePorts.Clear();
        FetchPorts();
    }
    /// <summary>
    /// Queue a class to recieve a arduino controller once one becomes availible.
    /// you can set the priority to assure that players are assigned arduinos in order.
    /// </summary>
    /// <param name="priority">Lowest first</param>
    /// <param name="method">method to call once an arduino controller becomes availible (loop back to calling </param>
    public static void RegisterForNewArduino(int priority, UnityAction method)
    {
        QueueForArduino.Add(new KeyValuePair<int, UnityAction>(priority, method));
        QueueForArduino.Sort(SortQueue);
    }
    static int SortQueue(KeyValuePair<int, UnityAction> a, KeyValuePair<int, UnityAction> b)
    {
        return a.Key.CompareTo(b.Key);
    }

    // On update, update 'last value'
    void Update() {
        List<SerialPortDataContainer> portsToRemove = new List<SerialPortDataContainer>(); //for removing ports that are not arduinos or have stopped working.
        foreach (SerialPortDataContainer port in _Ports)
        {
            try
            {
                port.ReadLastMessage();
            }
            catch (ArgumentException aex) {
                Debug.LogError(aex.ToString());
            }
            catch (Exception ex)
            {
                if (port.Port.IsOpen)
                {
                    Debug.LogError("Error: Something went wrong, port " + port.Port.PortName + " still open: " + ex.Message.ToString());
                }
                else
                {
                    Debug.LogError("Error: Port " + port.Port.PortName + " Closed Exception!");
                }
                portsToRemove.Add(port);
            }
        }
        foreach (SerialPortDataContainer port in portsToRemove)
        {
            RemovePort(port);
            Debug.Log(port.Port.PortName + " was removed for the active ports list");
            GameConsole.Log(port.Port.PortName + " was removed for the active ports list");
        }
        if (!_IsQueuedToConnect && QueueForArduino.Count != 0)
        {
            _IsQueuedToConnect = true;
            Invoke("FetchPorts", 4f);
        }
    }
    void OnApplicationQuit() {
        OnDestroy();
    }
    void OnDestroy()
    {
        foreach (SerialPortDataContainer port in _Ports)
        {
            port.ClosePort();
        }
    }
}