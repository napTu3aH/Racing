using System;
using UnityEngine;

public class WaitForSecondsRealTime : CustomYieldInstruction
{
    float _waitTime;

    public override bool keepWaiting
    {
        get { return Time.realtimeSinceStartup < _waitTime;}    
    }

    public WaitForSecondsRealTime(float _time)
    {
        _waitTime = Time.realtimeSinceStartup + _time;
    }
}
