using UnityEngine;
using System.Collections;

public class AeroplaneEvent : MonoBehaviour
{
    public static AeroplaneEvent Instance;

    [SerializeField] internal Aeroplane _Aeroplane;
    [SerializeField] internal float _Min, _Max;
    [SerializeField] internal bool _Flighted, _Randomized;

    [Header("Diagonal A")]
    [SerializeField] internal Transform[] _DiagonalA;

    [Header("Diagonal B")]
    [SerializeField] internal Transform[] _DiagonalB;

    internal bool _StartedRandom;
    Transform _start, _end;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        Instance = this;
    }

    void FixedUpdate()
    {
        if (_Randomized && !_StartedRandom)
        {
            _StartedRandom = true;
            StartCoroutine(_RandomTimeToFlight());
        }
    }

    internal void StartFlight()
    {
        int _rand = Random.Range(0, 2);
        switch (_rand)
        {
            case 0:
                ChoosenDiagonals(_DiagonalA);
                break;

            case 1:
                ChoosenDiagonals(_DiagonalB);
                break;
        }

    }

    void ChoosenDiagonals(Transform[] _diagonal)
    {
        int _rand = Random.Range(0, 2);
        switch (_rand)
        {
            case 0:
                _start = _diagonal[0];
                _end = _diagonal[1];
                break;

            case 1:
                _start = _diagonal[1];
                _end = _diagonal[0];
                break;
        }
        _Flighted = true;
        _Aeroplane.SetValues(_start, _end);
    }

    IEnumerator _RandomTimeToFlight()
    {
        float _rand = Random.Range(_Min, _Max);
        if (!_Flighted)
        {
            yield return new WaitForSeconds(_rand);
            StartFlight();
        }
    }
}
