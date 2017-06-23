using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ShutdownButton : MonoBehaviour {

	public void BT_Shutdown_Clicked()
    {
        Process.Start("shutdown", "/s /t 0");
    }
}
