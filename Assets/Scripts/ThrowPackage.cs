using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowPackage : MonoBehaviour {
    
    public GameObject startPosition;
    public GameObject package;
    public float maxThrowDistance = 500f;
    public float yOffset = 0.4f;
    public float xOffset = 0.4f;
    public Vector3 adjustment;

    public float timer = 0f;
    public float cooldown = 2f;

    void Start()
    {
        cooldown *= 60;
    }

    void Update()
    {
        if (Input.GetKeyDown("space") && timer >= cooldown)
        {
            Throw ();
        }
    }

    public void Throw()
    {
        //GameObject[] mailboxes = GameObject.FindGameObjectsWithTag("Mailbox");
        Camera camera = GetComponentInChildren<Camera>();

        Transform closest = null;
        float dot = -2;
        float shortestDistance = maxThrowDistance;
        foreach (Transform mailbox in Player.DeliveryPoints)
        {
            Vector3 cameraRelative = camera.transform.InverseTransformPoint(mailbox.transform.position).normalized;
            float distance = Vector3.Distance(camera.transform.position, mailbox.transform.position);
            float test = Vector3.Dot(cameraRelative, Vector3.forward);
            bool inSight = (cameraRelative.x <= xOffset && cameraRelative.x >= -xOffset
                && cameraRelative.y <= yOffset && cameraRelative.y >= -yOffset
                && cameraRelative.z > 0);
            if (test > dot && inSight && distance < shortestDistance)
            {
                dot = test;
                shortestDistance = distance;
                closest = mailbox;
            }
        }

        GameObject throwPackage;
        Vector3 a = (startPosition.transform.right * adjustment.x) + (startPosition.transform.up * adjustment.y) + (startPosition.transform.forward * adjustment.z);
        if (closest != null)
        {
            timer = 0f;
            throwPackage = Instantiate(package, startPosition.transform.position + a, Quaternion.LookRotation((closest.transform.position + Vector3.up * 0.4f - (startPosition.transform.position + a)).normalized));
            Physics.IgnoreCollision(GetComponent<CharacterController>(), throwPackage.GetComponent<BoxCollider>());
        }
    }

    private void FixedUpdate()
    {
        timer += 1;
    }
}
