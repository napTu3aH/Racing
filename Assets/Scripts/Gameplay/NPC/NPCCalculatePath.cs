using UnityEngine;
using System.Collections.Generic;

public class NPCCalculatePath : MonoBehaviour
{
    private static NPCCalculatePath _NPCCalculatePath;
    public static NPCCalculatePath Instance
    {
        get
        {
            if (_NPCCalculatePath != null)
            {
                return _NPCCalculatePath;
            }
            else
            {
                _NPCCalculatePath = new GameObject("_NPCCalculatePath", typeof(NPCCalculatePath)).GetComponent<NPCCalculatePath>();
                _NPCCalculatePath.transform.SetParent(GameObject.FindWithTag("InstanceLogics").transform);
                return _NPCCalculatePath;
            }
        }
    }

    public float _MinDistance = 5.0f;
    internal int _MaximumNPC;

    public List<Transform> _NavPoints;
    NavMeshPath[] _NavPath;
    int[] _NumberOfPoints;
    bool _Pathed;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        if (!_NPCCalculatePath)
        {
            _NPCCalculatePath = this;
        }
        _NavPoints = new List<Transform>();
    }

    void Start()
    {
        _NumberOfPoints = new int[_MaximumNPC];
        _NavPath = new NavMeshPath[_MaximumNPC];
        for (int i = 0; i < _NumberOfPoints.Length; i++)
        {
            _NavPath[i] = new NavMeshPath();
            ChangeNumberPoint(i);
        }
    }

    void ChangeNumberPoint(int i)
    {
        if(_NavPoints[_NumberOfPoints[0]]) _NumberOfPoints[i] = Random.Range(0, _NavPoints.Count);
        else _NumberOfPoints[i] = Random.Range(1, _NavPoints.Count);

        if (_NumberOfPoints[i] == i + 1)
        {
            ChangeNumberPoint(i);
        }
    }

    public void PathUpdate(CarAiController _AI, Transform _currentPoint)
    {
        if (_NavPoints[_NumberOfPoints[_AI._ID]])
        {
            _Pathed = NavMesh.CalculatePath(_AI.transform.position, _NavPoints[_NumberOfPoints[_AI._ID]].position, -1, _NavPath[_AI._ID]);
        }
        else
        {
            ChangeNumberPoint(_AI._ID);
        }
        
        if (_Pathed)
        {
            if (Debugger.Instance._DebugLine)
            {
                for (int j = 0; j < _NavPath[_AI._ID].corners.Length - 1; j++)
                {
                    Debugger.Instance.Line(_NavPath[_AI._ID].corners[j], _NavPath[_AI._ID].corners[j + 1]);
                }
            }
                       
            if (_NavPath[_AI._ID].corners.Length > 1)
            {
                _currentPoint.position = _NavPath[_AI._ID].corners[1];
            }
            else
            {
                _currentPoint.position = _NavPath[_AI._ID].corners[0];
            }
            
        }
                
        if (_AI._Distance < _MinDistance)
        {
            ChangeNumberPoint(_AI._ID);
        }   
    }

    public void RemoveNPC(CarAiController _aiControl, BackDrive _backDrive)
    {
        Destroy(_aiControl);
        Destroy(_backDrive);
    }
}
