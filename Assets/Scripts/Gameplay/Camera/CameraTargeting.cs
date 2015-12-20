using UnityEngine;
using System.Collections;

public class CameraTargeting : MonoBehaviour
{
    [SerializeField] internal Transform _Camera;
    [SerializeField] internal Transform _Target;
    [SerializeField] internal bool _Start
    {
        set
        {
            _start = value;
            if (_start)
            {
                StartCoroutine(Targeting());
            }
            else
            {
                StopCoroutine(Targeting());
            }            
        }
        get
        {
            return _start;
        }
    }

    internal bool _start;

    void Start()
    {
        _Start = !_Start;
    }

    Vector3 _CurrentPosition;
    Quaternion _quat;

    IEnumerator Targeting()
    {
        while (_Target)
        {
            _CurrentPosition = (_Target.position - _Camera.position) * 0.5f;
            _CurrentPosition.y = 0.0f;
            Debug.DrawRay(_Camera.position, _CurrentPosition, Color.red);
            _quat = Quaternion.LookRotation(_CurrentPosition);
            _Camera.localEulerAngles = new Vector3(0.0f, _quat.eulerAngles.y, 0.0f);
            yield return null;
        }
        yield return null;
    }
}
