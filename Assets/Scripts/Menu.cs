using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VR;

public class Menu : MonoBehaviour {

	void Start()
	{
		VRSettings.enabled = false;
	}

	public void Play()
    {
        Player.resetValues();
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Debug.Log("Quit!!");
        Application.Quit();
    }
}
