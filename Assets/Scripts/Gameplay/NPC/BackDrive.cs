using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class BackDrive : MonoBehaviour {
	
	CarController _CarControl;
	CarAIControl _AILogic;
	
	public float _StayTimer;

    float _StartSpeed;
    float _TimeForСheck;
    float _DistanceFromStart;

    Vector3 _StartPointBrake;

	bool _HittedToWall, _AIStateChange, _Coroutined;

	void Awake () 
	{
        Init();
	}

    void Init()
    {
        _CarControl = GetComponent<CarController>();
        _AILogic = GetComponent<CarAIControl>();
        _StartSpeed = _CarControl._FullTorqueOverAllWheels;
        _TimeForСheck = Random.Range(1.5f, 1.6f);
    }

    public void HittedToWall()
    {
        _HittedToWall = true;
        _StayTimer = 0.0f;
        _CarControl._FullTorqueOverAllWheels = -5000f;
        _DistanceFromStart = 0.0f;
        if (!_Coroutined)
        {
            _Coroutined = true;
            StartCoroutine(ResetSpeed());
        }
        
    }

    IEnumerator ResetSpeed()
    {
        while (_HittedToWall)
        {
            _DistanceFromStart = Vector3.Distance(transform.position, _StartPointBrake);
            if (_DistanceFromStart >= 1.2f)
            {
                _CarControl._FullTorqueOverAllWheels = _StartSpeed;
                _HittedToWall = false;
            }
            yield return null;
        }
        _Coroutined = false;
        yield return null;
    }

	void FixedUpdate () 
	{
		if(_StayTimer >= _TimeForСheck)
		{
			if(_CarControl._FullTorqueOverAllWheels == _StartSpeed)
			{
                _StartPointBrake = transform.position;
                HittedToWall();				
			}	
		}

	}
}