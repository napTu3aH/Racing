using UnityEngine;
using System.Collections;

public class Aeroplane : MonoBehaviour
{
    [SerializeField] internal GameObject _Plane;
    [SerializeField] internal Transform _Target;
    [SerializeField] internal float _SpeedFlight;
    
    Vector3 _StartPosition;
    float _Distance, _TimeFlight;
    bool _Alive = true;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        _Plane = gameObject;   
        _StartPosition = transform.position;
        transform.rotation = Quaternion.LookRotation(_Target.position - transform.position);
        StartCoroutine(Flight());
    }

    IEnumerator Flight()
    {
        while (_Alive)
        {
            _TimeFlight += Time.deltaTime * _SpeedFlight;
            if (_TimeFlight > 1.0f)
            {
                _Alive = false;
            }
            transform.position = Vector3.Lerp(_StartPosition, _Target.position, _TimeFlight);
            yield return null;
        }
        _Plane.SetActive(false);
        yield return null;
    }
}
