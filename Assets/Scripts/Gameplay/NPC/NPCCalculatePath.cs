using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCCalculatePath : MonoBehaviour
{
    public static NPCCalculatePath Instance;

    public List<Transform> _NPC_Cars;
    public List<Transform> _CurrentWayPoints;
    public GameObject _HeadNavPoints;
    public float _MinDistance = 5.0f;
    public int _MaximumNPC;
    public float[] _Distance;

    Transform[] _NavPoints;
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
        if (!_HeadNavPoints)
        {
            _HeadNavPoints = GameObject.FindWithTag("Waypoints");
        }

        if (_NavPoints == null)
        {
            _NavPoints = _HeadNavPoints.GetComponentsInChildren<Transform>();
        }
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
        _NumberOfPoints[i] = Random.Range(1, _NavPoints.Length);
    }

    public void DistaceUpdate(int _Id_NPC)
    {
        if (Instance)
        {
            _Distance[_Id_NPC] = Vector3.Distance(_NPC_Cars[_Id_NPC].position, _NavPoints[_NumberOfPoints[_Id_NPC]].position);
        }        
    }

    public void PathUpdate(int _Id_NPC)
    {
        _Pathed = NavMesh.CalculatePath(_NPC_Cars[_Id_NPC].position, _NavPoints[_NumberOfPoints[_Id_NPC]].position, -1, _NavPath[_Id_NPC]);

        if (_Pathed)
        {
            for (int j = 0; j < _NavPath[_Id_NPC].corners.Length - 1; j++)
            {
                Debugger.Instance.Line(_NavPath[_Id_NPC].corners[j], _NavPath[_Id_NPC].corners[j+1]);
            }
            _CurrentWayPoints[_Id_NPC].position = _NavPath[_Id_NPC].corners[1];
        }
        
        if (_Distance[_Id_NPC] < _MinDistance)
        {
            ChangeNumberPoint(_Id_NPC);
        }   
    }
}
