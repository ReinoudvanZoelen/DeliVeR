using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionLog : MonoBehaviour {

    static ConnectionLog _Instance;
    public Text T_Connections;
    private void Awake()
    {
        if (ConnectionLog._Instance == null)
        {
            _Instance = this;
        }
    }

    HashSet<string> _Connections = new HashSet<string>();

    #region Static wrapping
    public static void RegisterConnection(string name)
    {
        _Instance._RegisterConnection(name);
    }
    public static void UnregisterConnection(string name)
    {
        _Instance._UnregisterConnection(name);
    }
    public static void ClearConnections()
    {
        _Instance._ClearConnections();
    }
    #endregion

    void _RegisterConnection(string name)
    {
        _Connections.Add(name);
        UpdateUI();
    }
    void _UnregisterConnection(string name)
    {
        _Connections.Remove(name);
        UpdateUI();
    }
    void _ClearConnections()
    {
        _Connections.Clear();
        UpdateUI();
    }

    void UpdateUI()
    {
        string tempText = string.Empty;
        foreach (string s in _Connections)
        {
            tempText += s + "\n";
        }
        T_Connections.text = tempText;
        T_Connections.rectTransform.sizeDelta = new Vector2(T_Connections.rectTransform.sizeDelta.x, _Connections.Count * (T_Connections.fontSize + T_Connections.lineSpacing * 2));
    }
}