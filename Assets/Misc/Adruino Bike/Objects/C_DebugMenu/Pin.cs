using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Controller voor de PIN toegang tot het debug menu.
/// Code = 987
/// </summary>
public class Pin : MonoBehaviour {

    public DebugMenu menu;

    public Text T_Didget1;
    public Text T_Didget2;
    public Text T_Didget3;

    int _Didget1 = 0;
    int _Didget2 = 0;
    int _Didget3 = 0;

    public void BT_nr1Up_Clicked()
    {
        _Didget1++;
        if(_Didget1 > 9)
        {
            _Didget1 = 0;
        }
        CheckPin();
        UpdateUI();
    }
    public void BT_nr1Down_Clicked()
    {
        _Didget1--;
        if (_Didget1 < 0)
        {
            _Didget1 = 9;
        }
        CheckPin();
        UpdateUI();
    }
    public void BT_nr2Up_Clicked()
    {
        _Didget2++;
        if (_Didget2 > 9)
        {
            _Didget2 = 0;
        }
        CheckPin();
        UpdateUI();
    }
    public void BT_nr2Down_Clicked()
    {
        _Didget2--;
        if (_Didget2 < 0)
        {
            _Didget2 = 9;
        }
        CheckPin();
        UpdateUI();
    }
    public void BT_nr3Up_Clicked()
    {
        _Didget3++;
        if (_Didget3 > 9)
        {
            _Didget3 = 0;
        }
        CheckPin();
        UpdateUI();
    }
    public void BT_nr3Down_Clicked()
    {
        _Didget3--;
        if (_Didget3 < 0)
        {
            _Didget3 = 9;
        }
        CheckPin();
        UpdateUI();
    }

    void CheckPin()
    {
        if(_Didget1 == 9 && _Didget2 == 8 && _Didget3 == 7)
        {
            menu.ToggleDebugMenu();
            ResetPin();
            gameObject.SetActive(false);
        }
    }

    public void ResetPin()
    {
        _Didget1 = 0;
        _Didget2 = 0;
        _Didget3 = 0;
        UpdateUI();
    }

    void UpdateUI()
    {
        T_Didget1.text = _Didget1.ToString();
        T_Didget2.text = _Didget2.ToString();
        T_Didget3.text = _Didget3.ToString();
    }
}
