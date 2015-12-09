using UnityEngine;
using System.Collections;

public class Debugger : MonoBehaviour {

    public bool _DebugLog = false;
    public bool _DebugLine = false;

    private static Debugger _Debugger;
    public static Debugger Instance
    {
        get
        {
            if (_Debugger != null)
            {
                return _Debugger;
            }
            else
            {
                _Debugger = new GameObject("Debugger", typeof(Debugger)).GetComponent<Debugger>();
                return _Debugger;
            }
        }
    }

    void Awake()
    {
        _Debugger = this;
    }

    public void Log(object _msg)
    {
        if (_DebugLog)
        {
            Debug.Log(_msg);
        }
    }

    public void Line(Vector3 _start, Vector3 _end, Color _color, float _duration = 0.0f, bool _depthTest = true)
    {
        if (_DebugLine)
        {
            Debug.DrawLine(_start, _end, _color, _duration, _depthTest);
        }
    }

    public void Line(Vector3 _start, Vector3 _end, float _duration = 0.0f, bool _depthTest = true)
    {
        if (_DebugLine)
        {
            Color _color = Color.white;
            Debug.DrawLine(_start, _end, _color, _duration, _depthTest);
        }
    }
}
