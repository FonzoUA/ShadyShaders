using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public Transform hoursTransform;
    public Transform minutesTransform;
    public Transform secondsTransform;

    private float degreePerHour = 30.0f;
    private float degreePerMinute;
    private float degreePerSecond;

    private void Awake()
    {
        degreePerMinute = 6.0f;
        degreePerSecond = 6.0f;
    }

    private void Update()
    {
        System.DateTime time = System.DateTime.Now;

        hoursTransform.localRotation = Quaternion.Euler(0, time.Hour * degreePerHour, 0);
        minutesTransform.localRotation = Quaternion.Euler(0, time.Minute * degreePerMinute, 0);
        secondsTransform.localRotation = Quaternion.Euler(0, time.Second * degreePerSecond, 0);
    }

}
