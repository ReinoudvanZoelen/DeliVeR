using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using UnityEngine.VR;

public class Loadout_Menu : MonoBehaviour
{
    public GameObject Base_Player;
	public GameObject Game_Manager;
	public GameObject L_Menu;

    public GameObject Marker;

    [Header("Range")]
    public int MinPackages;
    public int MaxPackages;
    public int MinMinutes;
    public int MaxMinutes;
    public int StepMinutes;

    [Header("Packages")]
    public int PackagesAmount;
    public string PackagesTextBase;
    public Text PackagesText;
    [Header("Time")]
    public int TimeAmount;
    public string TimeTextBase;
    public Text TimeText;

	void Start ()
	{
        VRSettings.enabled = false;
        UpdateMenu();
	}
	

    public void AddPackages()
    {
        Debug.Log("AddPackages");
        if (!(PackagesAmount + 1 > MaxPackages))
        {
            PackagesAmount++;
        }
        UpdateMenu();
    }

    public void MinusPackages()
    {
        Debug.Log("MinusPackages");
        if (!(PackagesAmount - 1 < MinPackages))
        {
            PackagesAmount--;
        }
        UpdateMenu();
    }
    public void AddTime()
    {
        Debug.Log("AddTime");
        if (!(TimeAmount + StepMinutes > MaxMinutes))
        {
            TimeAmount = TimeAmount + StepMinutes;
        }
        UpdateMenu();
    }
    public void MinusTime()
    {
        Debug.Log("MinusTime");
        if (!(TimeAmount - StepMinutes < MinMinutes))
        {
            TimeAmount = TimeAmount - StepMinutes;
        }
        UpdateMenu();
    }

    public void Generate()
    {
        RemoveGeneratedPoints();
        Random rnd = new Random();
        List<Transform> MyPoints = Mailboxen.Mailbox.ToList();
        Transform[] GeneratedPoints = new Transform[PackagesAmount];

        for (int i = 0; i < PackagesAmount; i++)
        {
            int index = Random.Range(0, MyPoints.Count);
            GeneratedPoints[i] = MyPoints[index];
            MyPoints.RemoveAt(index);
        }

        foreach (var p in GeneratedPoints)
        {
            Instantiate(Marker, p);
        }

        Debug.Log("Gererenenenen");
        Player.DeliveryPoints = GeneratedPoints.ToList();
    }

    void RemoveGeneratedPoints()
    {
        GameObject[] Mailboxen = GameObject.FindGameObjectsWithTag("Mailbox");
        foreach (var Mailbox in Mailboxen)
        {

            if (Mailbox.transform.childCount > 1)
            {
                Transform childTransform = Mailbox.transform.GetChild(1);
                childTransform.SetParent(null);
                GameObject.Destroy(childTransform.gameObject);
            }
        }
    }

    public void StartMap()
    {
        if(Player.DeliveryPoints.Count <= 0)
        {
            Generate();
        }
        Player.ObjectivesToDo = PackagesAmount;
        Player.SetTime = TimeAmount * 60;

        VRSettings.enabled = true;
        Base_Player.SetActive(true);
		Game_Manager.SetActive(true);
		L_Menu.SetActive(false);
        //GameObject.Find("LoseScreen").SetActive(false);
        //GameObject.Find("WinScreen").SetActive(false);

        Debug.Log("Fietsen Maar!");
    }

    void UpdateMenu()
    {
        PackagesText.text = PackagesTextBase + " " + PackagesAmount;
        TimeText.text = TimeTextBase + " " + TimeAmount;
    }
}
