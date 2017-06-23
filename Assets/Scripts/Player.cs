using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    public static List<Transform> DeliveryPoints = new List<Transform>();
	private static bool TimerStarted = false;
	public static float TimeStart;
	public static float TimePassed;
	public static float _ActualSpeed;
    public static int ObjectivesToDo;
    public static int ObjectivesDone = 0;

    public Text KmText;
    public Text Progress;

    public GameObject Arrow;
    public GameObject TargetArrow;

	public static float TimeLeft
	{
		get
		{
			return TimeStart - TimePassed;
		}
	}
	public static float SetTime
	{
		set
		{
			TimeStart = value;
			TimePassed = 0;
			TimerStarted = true;
		}
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (TimerStarted && TimePassed < TimeStart)
		{
			TimePassed += Time.deltaTime;
			if (TimePassed >= TimeStart)
			{
				TimePassed = TimeStart;
				TimerStarted = false;
			}
		}
		GetNearestMailbox();
        UpdateUI();
    }

    private void UpdateUI()
    {
        Progress.text = ObjectivesDone + " / " + ObjectivesToDo;
        KmText.text = Mathf.Round(_ActualSpeed * 50).ToString();
		if (TargetArrow != null)
		{
			Arrow.transform.LookAt(TargetArrow.transform);
		}
	}

    private void GetNearestMailbox()
    {
        if(DeliveryPoints.Count <= 0)
        {
            return;
        }

        Transform CurrentNearest = DeliveryPoints[0];

        foreach (var DeliveryPoint in DeliveryPoints)
        {
            if(Vector3.Distance(DeliveryPoint.position, this.transform.position) < Vector3.Distance(CurrentNearest.position, this.transform.position))
            {
                CurrentNearest = DeliveryPoint;
            }
        }

        TargetArrow = CurrentNearest.gameObject;
    }

    public static void CompleteObjective(Transform hit)
    {
        DeliveryPoints.Remove(hit);
        ObjectivesDone++;
        if (ObjectivesDone >= ObjectivesToDo)
        {
            TimerStarted = false;
        }
    }

    public static void resetValues()
    {
        DeliveryPoints = new List<Transform>();
        TimerStarted = false;
        TimeStart = 0;
        TimePassed = 0;
        _ActualSpeed = 0;
        ObjectivesToDo = 0;
        ObjectivesDone = 0;
    }

}
