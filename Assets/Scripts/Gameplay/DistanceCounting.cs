using UnityEngine;

public class DistanceCounting : MonoBehaviour
{
    private static DistanceCounting _DistanceCounting;
    public static DistanceCounting Instance
    {
        get
        {
            if (_DistanceCounting != null)
            {
                return _DistanceCounting;
            }
            else
            {
                _DistanceCounting = new GameObject("_DistanceCounting", typeof(DistanceCounting)).GetComponent<DistanceCounting>();
                _DistanceCounting.transform.SetParent(GameObject.FindWithTag("InstanceLogics").transform);
                return _DistanceCounting;
            }
        }
    }


    void Awake()
    {
        Init();
    }

    void Init()
    {
        if (!_DistanceCounting)
        {
            _DistanceCounting = this;
        }
    }

    internal float _Distance(Vector3 _start, Vector3 _end)
    {        
        return Vector3.Distance(_start, _end);
    }
}
