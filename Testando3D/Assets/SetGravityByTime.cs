using Assets.Scripts.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGravityByTime : MonoBehaviour
{
    public float time;
    CountDown cd;
    Rigidbody r;

    void Start()
    {
        cd = new CountDown(time);
        r = GetComponent<Rigidbody>();
        r.isKinematic = true;
        cd.StartToCount();
    }

    void Update()
    {
        if (r.isKinematic)
        {
            cd.DecreaseTime();
            if (cd.ReturnedToZero)
                r.isKinematic = false;
        }
    }
}
