using UnityEngine;
using System.Collections;

public class ReturnerTimeScale : MonoBehaviour
{
    private static ReturnerTimeScale _TimeScaler;
    public static ReturnerTimeScale Instance
    {
        get
        {
            if (_TimeScaler != null)
            {
                return _TimeScaler;
            }
            else
            {
                _TimeScaler = new GameObject("_TimeScaler", typeof(ReturnerTimeScale)).GetComponent<ReturnerTimeScale>();
                _TimeScaler.transform.SetParent(GameObject.FindWithTag("InstanceLogics").transform);
                return _TimeScaler;
            }
        }
    }

    void Awake()
    {
        Init();
    }

    void Init()
    {
        _TimeScaler = this;
    }

    internal void ReturnTimeScale()
    {
        if (Time.timeScale < 1.0f)
        {
            Time.timeScale = 1.0f;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
        }
    }
}
