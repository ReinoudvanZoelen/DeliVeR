using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu : MonoBehaviour {

    public GameObject[] Panels;
    public GameObject P_Pin;
    static DebugMenu _Instance;
    
	void Awake () {
        if (_Instance == null)
        {
            _Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (GameObject gb in Panels)
        {
            gb.SetActive(false);
        }
        P_Pin.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            ToggleDebugMenu();
        }
    }

    public void ToggleDebugMenu() {
        foreach(GameObject gb in Panels)
        {
            gb.SetActive(!gb.activeSelf);
        }
    }

    public void BT_DebugMenu_Clicked()
    {
        if (Panels[0].activeSelf)
        {
            ToggleDebugMenu();
        }
        else
        {
            P_Pin.SetActive(!P_Pin.activeSelf);
        }
    }
}
