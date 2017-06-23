using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mailboxen : MonoBehaviour {

    public static Transform[] Mailbox;

    void Awake()
    {
        Mailbox = new Transform[transform.childCount];
        for (int i = 0; i < Mailbox.Length; i++)
        {
            Mailbox[i] = transform.GetChild(i);
        }
    }
}
