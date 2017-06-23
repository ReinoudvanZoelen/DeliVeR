using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Class responsable for maintaining a updated list of all the arduinos that have successfully connected.
/// </summary>
public class ConnectionFeedback : MonoBehaviour {

    public GameObject[] FeedbackObjects;
    bool _WasOn = true;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        List<string> arduinos = ArduinoControl.GetConnectedArduinos();
        if ((arduinos.Count != 0 || ArduinoControl.QueueForArduino.Count == 0)&& _WasOn)
        {
            foreach (GameObject gb in FeedbackObjects)
            {
                gb.SetActive(false);
            }
            _WasOn = false;
        }
        else if (arduinos.Count == 0 && ArduinoControl.QueueForArduino.Count != 0 && !_WasOn)
        {
            foreach (GameObject gb in FeedbackObjects)
            {
                gb.SetActive(true);
            }
            _WasOn = true;
        }
        ConnectionLog.ClearConnections();
        foreach(string s in arduinos)
        {
            ConnectionLog.RegisterConnection(s);
        }
    }
}