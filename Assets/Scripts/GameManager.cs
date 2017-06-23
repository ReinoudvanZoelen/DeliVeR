using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour {


	public float RestartTime;
	private bool Started = false;

	[Header("WinScreen")]
	public GameObject WinScreen;
	public Text Win_Packages;
	public Text Win_Time;
	public Text Win_Cooldown;

	[Header("LoseScreen")]
	public GameObject LoseScreen;
	public Text Lose_Packages;
	public Text Lose_Time;
	public Text Lose_Cooldown;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    if (Started == true)
	    {
		    if(RestartTime <= 0)
		    {
			    SceneManager.LoadScene(0);
		    }
		    RestartTime -= Time.deltaTime;
	    }
		
	    if(Player.ObjectivesToDo == Player.ObjectivesDone)
	    {
		    CreateWinScreen();
		    Started = true;
	    }
	    else if(Player.TimeLeft <= 0)
	    {
		    CreateLoseScreen();
		    Started = true;
	    }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
	}

	void CreateWinScreen()
	{
		WinScreen.SetActive(true);
		Win_Packages.text = Player.ObjectivesToDo.ToString();
        int minutes = Mathf.FloorToInt(Player.TimePassed / 60);
        int seconds = Mathf.FloorToInt(Player.TimePassed % 60);
        Win_Time.text = String.Format("{0}:{1:00}", minutes, seconds);
        Win_Cooldown.text = Math.Round(RestartTime).ToString();


	}

	void CreateLoseScreen()
	{
		LoseScreen.SetActive(true);
		Lose_Packages.text = Player.ObjectivesDone.ToString();
        int minutes = Mathf.FloorToInt(Player.TimeStart / 60);
        int seconds = Mathf.FloorToInt(Player.TimeStart % 60);
        Lose_Time.text = String.Format("{0}:{1:00}", minutes, seconds);
		Lose_Cooldown.text = Math.Round(RestartTime).ToString();
	}


}
