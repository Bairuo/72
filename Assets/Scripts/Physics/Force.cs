using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface TimedPhysEffect
{
    float timelast{ get; set; }
}

public struct Force : TimedPhysEffect
{
    [SerializeField]
    public Vector2 value;
    [SerializeField]
    public float timelast{ get; set; }
    public Force(Vector2 dir, float time)
    {
        value = dir;
        timelast = time;
    }
}

public struct Torque : TimedPhysEffect
{
    [SerializeField]
    public float value;
    [SerializeField]
    public float timelast{ get; set; }
    public Torque(float torq, float time)
    {
        value = torq;
        timelast = time;
    }
}