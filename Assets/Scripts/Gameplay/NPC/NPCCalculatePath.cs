using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCCalculatePath : MonoBehaviour
{
    public static NPCCalculatePath Instance;

    public List<Transform> _NPC_Cars;
    public List<Transform> _CurrentWayPoints;
    public float _MinDistance = 5.0f;
    public int _MaximumNPC;
    public float[] _Distance;

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
        Instance = this;
    }

    void Start()
    {
        _NumberOfPoints = new int[_MaximumNPC];
        _NavPath = new NavMeshPath[_MaximumNPC];
        _Distance = new float[_MaximumNPC];
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

    public void DistaceUpdate(int _Id_NPC)
    {
        if (Instance)
        {
            if (_NavPoints[_NumberOfPoints[_Id_NPC]])
            {
                _Distance[_Id_NPC] = Vector3.Distance(_NPC_Cars[_Id_NPC].position, _NavPoints[_NumberOfPoints[_Id_NPC]].position);
            }
            else
            {
                ChangeNumberPoint(_Id_NPC);
            }
            
        }        
    }

    public void PathUpdate(int _Id_NPC)
    {
        if (_NavPoints[_NumberOfPoints[_Id_NPC]])
        {
            _Pathed = NavMesh.CalculatePath(_NPC_Cars[_Id_NPC].position, _NavPoints[_NumberOfPoints[_Id_NPC]].position, -1, _NavPath[_Id_NPC]);
        }
        else
        {
            ChangeNumberPoint(_Id_NPC);
        }
        

        if (_Pathed)
        {
            for (int j = 0; j < _NavPath[_Id_NPC].corners.Length - 1; j++)
            {
                Debugger.Instance.Line(_NavPath[_Id_NPC].corners[j], _NavPath[_Id_NPC].corners[j+1]);
            }
            if (_NavPath[_Id_NPC].corners.Length > 1)
            {
                _CurrentWayPoints[_Id_NPC].position = _NavPath[_Id_NPC].corners[1];
            }
            else
            {
                _CurrentWayPoints[_Id_NPC].position = _NavPath[_Id_NPC].corners[0];
            }
            
        }
        
        if (_Distance[_Id_NPC] < _MinDistance)
        {
            ChangeNumberPoint(_Id_NPC);
        }   
    }

    public void RemoveNPC(int _id, Transform _transform)
    {
        _NPC_Cars.Remove(_transform);
        Destroy(_CurrentWayPoints[_id].gameObject);
        _CurrentWayPoints.Remove(_CurrentWayPoints[_id]);
    }
}
