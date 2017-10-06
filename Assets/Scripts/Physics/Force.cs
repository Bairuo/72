using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TimedPhysEffect
{
    float timelast{ get; set; }
}

public class Force : TimedPhysEffect
{
    [SerializeField] public Vector2 value;
    public float timelast{ get; set; }
    public Force(Vector2 dir, float time)
    {
        value = dir;
        timelast = time;
    }
    public Force()
    {
        value = Vector2.zero;
        timelast = 0f;
    }
}

public class Torque : TimedPhysEffect
{
    [SerializeField] public float value;
    public float timelast{ get; set; }
    public Torque(float torq, float time)
    {
        value = torq;
        timelast = time;
    }
    public Torque()
    {
        value = timelast = 0f;
    }
}