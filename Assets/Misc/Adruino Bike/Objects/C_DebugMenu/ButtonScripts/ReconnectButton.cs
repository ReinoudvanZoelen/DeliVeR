using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReconnectButton : MonoBehaviour {

	public void BT_Reconnect_Clicked()
    {
        ArduinoControl.Reconnect();
    }
}
