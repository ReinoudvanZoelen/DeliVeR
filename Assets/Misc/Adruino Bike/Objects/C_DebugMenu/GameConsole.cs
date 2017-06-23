using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/// <summary>
/// Controles the console. Use .Instance to obtain the current ConsoleScript.
/// Invoke or register commands and log text useing the Instance.
/// The instance persists between scenes.
/// </summary>
public class GameConsole : MonoBehaviour {

    static GameConsole _Instance;
    void Awake() {
        if (GameConsole._Instance == null)
        {
            _Instance = this;
        }
    }

    public Text T_Log;
    public InputField IF_CommandInput;
    public int LogLength = 500; //how many lines are remembered

    Dictionary<string, Delegate> _RegisteredCommands = new Dictionary<string, Delegate>();
    List<string> _LogEntries = new List<string>();

    #region Static wrapping    
    /// <summary>
    /// Writes text to the console log. Does not procces it as a command! simply prints the text.
    /// If you want to submit a command use ProccesConsoleCommand()
    /// </summary>
    /// <param name="entry">The entry.</param>
    public static void Log(string entry) {
        if (_Instance != null)
        {
            _Instance._Log(entry);
        }
        else { Debug.Log("Missing instance of GameConsole"); }
    }
    /// <summary>
    /// Registers a new method for calling via the console via typing the command text.
    /// </summary>
    /// <param name="method">The method.</param>
    /// <param name="command">The text to type in console.</param>
    public static void RegisterCommand(Delegate method, string command) {
        if (_Instance != null)
        {
            _Instance._RegisterCommand(method, command);
        }
        else { Debug.Log("Missing instance of GameConsole"); }
    }
    /// <summary>
    /// Call the method registered under the given command
    /// and logs it in the console log.
    /// </summary>
    /// <param name="command">The command.</param>
    public static void ProccesConsoleCommand(string command) {
        if (_Instance != null)
        {
            _Instance._ProccesConsoleCommand(command);
        }
        else { Debug.Log("Missing instance of GameConsole"); }
    }
    #endregion

    void Start() {
        RegisterInitialCommands();
    }

    void RegisterInitialCommands()
    {
        Action method = ClearConsole;
        _RegisterCommand(method, "Clear");
        method = ListCommands;
        _RegisterCommand(method, "Help");
    }
    
    public void _RegisterCommand(Delegate method, string command)
    {
        _RegisteredCommands.Add(command, method);
    }
    
    public void _Log(string entry) {
        _LogEntries.Add(entry);
        if (_LogEntries.Count > LogLength)
        {
            _LogEntries.RemoveAt(0);
        }
        string logText = string.Empty;
        foreach (string s in _LogEntries) {
            logText += s + "\n";
        }
        T_Log.text = logText;
        T_Log.rectTransform.sizeDelta = new Vector2(T_Log.rectTransform.sizeDelta.x ,_LogEntries.Count * (T_Log.fontSize + T_Log.lineSpacing * 2));
    }
    
    public void _ProccesConsoleCommand(string command) {
        _Log(command);
        string[] commandBreakdown = command.Split(' ');
        Delegate method = null;
        try
        {
            method = _RegisteredCommands[commandBreakdown[0]];
            List<object> args = new List<object>();
            int index = 1;
            while (index < commandBreakdown.Length)
            {
                args.Add(commandBreakdown[index]);
                index++;
            }
            method.DynamicInvoke(args.ToArray());
        }
        catch (KeyNotFoundException)
        {
            _Log("Unknown Command");
        }
        finally {
            IF_CommandInput.text = string.Empty;
        }
    }

    //----------Basic commands of the console----------//
    public void ClearConsole() {
        _LogEntries.Clear();
    }
    public void ListCommands() {
        foreach (KeyValuePair<string,Delegate> kvp in _RegisteredCommands) {
            Log(kvp.Key);
        }
    }
}
