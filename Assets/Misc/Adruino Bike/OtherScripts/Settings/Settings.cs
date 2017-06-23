using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Container for Game settings that can be modified and need to be persistant and/or shared between e-installations.
/// </summary>
[Serializable]
public class Settings {

    /* On awake we check if there already is an instance of the class.
     * if not this will be set as the instance. 
     * If there is already an instance then this will remove itself.
     */
    private static Settings _Instance;
    public static Settings Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new Settings();
            }
            return _Instance;
        }
    }

    /*Settings*/
    //Arduino Connection
    public bool DEBUGMSG = false;
    public int BAUT_RATE = 115200;
    public int TRACK_COUNT = 5;
    public int READ_TIMEOUT = 50;
    public int WRITE_TIMEOUT = 50;
    public bool HANDSHAKE = true;
    public string HANDSHAKE_SEND = "@@@@@@@"; //when sending text to and from the arduino the string tends to lose characters at the beginning and end of the string.
    public string HANDSHAKE_RECIEVE = "@@";
    /*End settings*/
}