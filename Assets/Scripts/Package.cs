using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Package : MonoBehaviour {

    public float forceForward = 4f;
    public float forceUp = 1f;
    public float decayTime = 5f;
    private BikeMovement player;

    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<BikeMovement>();
        forceForward += player._ActualSpeed * 10;
        forceForward *= 1000f;
        forceUp *= 1000f;
        GetComponent<Rigidbody>().AddForce(transform.forward * forceForward + transform.up * forceUp);
        Destroy(gameObject, decayTime);
    }
	
	void Update () {

    }

    void OnTriggerEnter(Collider hit)
    {
        if (hit.transform.gameObject.tag == "Mailbox" && Player.DeliveryPoints.Contains(hit.transform))
        {
            Destroy(gameObject);
            Player.CompleteObjective(hit.transform);
            if (hit.transform.childCount > 1)
            {
                Transform childTransform = hit.transform.GetChild(1);
                childTransform.SetParent(null);
                GameObject.Destroy(childTransform.gameObject);
            }
        }
    }
}
