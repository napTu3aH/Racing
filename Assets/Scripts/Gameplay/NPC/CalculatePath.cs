using UnityEngine;
using System.Collections;

public class CalculatePath : MonoBehaviour 
{

	NavMeshPath _MainPath;
	public GameObject _HeadNavPoints;
	Transform _CarTransform;
	Transform[] _NavigationPoints;
	bool _FlagPath;
	public Transform _CurWayPoint;
	int _NumberNavPoint;

	public float _MinDistance = 3.0f;

	void Awake () 
	{
        _HeadNavPoints = GameObject.FindWithTag("Waypoints");

		_NavigationPoints = _HeadNavPoints.GetComponentsInChildren<Transform>();
		_CarTransform = transform;
		_MainPath = new NavMeshPath();
        ChangeNumberPoint();
    }

	void Update () 
	{
		_FlagPath = NavMesh.CalculatePath(_CarTransform.position,_NavigationPoints[_NumberNavPoint].position,-1,_MainPath);
		if(_FlagPath)
		{
			for(int i = 0; i < _MainPath.corners.Length - 1; i++)
			{
				//Debugger.Instance.Line(_MainPath.corners[i],_MainPath.corners[i+1]);
				_CurWayPoint.position = _MainPath.corners[1];
			}
		}
		if(Vector3.Distance(_CarTransform.position,_NavigationPoints[_NumberNavPoint].position) <= _MinDistance)
		{
            ChangeNumberPoint();
        }
	}

    void ChangeNumberPoint()
    {
        _NumberNavPoint = Random.Range(1, _NavigationPoints.Length);
    }

	void OnTriggerEnter(Collider col)
	{
		if(col.CompareTag("Walls"))
		{
            ChangeNumberPoint();
		}
	}
}