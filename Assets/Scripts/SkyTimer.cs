using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkyTimer : MonoBehaviour {

    public List<Text> timerTexts;

    void Start()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("TimerText");
        foreach (var text in gos)
        {
            timerTexts.Add(text.GetComponent<Text>());
        }
    }

	void Update ()
    {
        int minutes = Mathf.FloorToInt(Player.TimeLeft / 60);
        int seconds = Mathf.FloorToInt(Player.TimeLeft % 60);
        foreach (Text t in timerTexts)
        {
            t.text = String.Format("{0}:{1:00}", minutes, seconds);
        }
    }

}
