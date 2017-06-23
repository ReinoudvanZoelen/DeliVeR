using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public GameObject target;

    float mSpeed = 50f; // Scale. Speed of the movement

    // Update is called once per frame
    void Update()
    {
        AutoRotate();
    }

    void AutoRotate()
    {
        this.transform.LookAt(target.transform);
        this.transform.Translate(Vector3.right * (Time.deltaTime * mSpeed));
    }
}