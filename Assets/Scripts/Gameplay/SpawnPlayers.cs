using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.Vehicles.Car;

/// <summary>
/// Класс создания игроков (NPC) на сцене.
/// </summary>
public class SpawnPlayers : MonoBehaviour {

    public static SpawnPlayers Instance;

    [Header("Spawn")]
    public bool _Spawn;
    public bool _PlayerSpawned;
    public List<PlayerTargetPoint> _Targets;

    [Header("Prefabs")]
    [SerializeField] internal GameObject[] _PlayerCars;
    [SerializeField] internal GameObject[] _Cars;
    [SerializeField] internal GameObject _CurrentWayPoint;
    [SerializeField] internal GameObject _HeaderWayPoints;

    [Header("Count players and spawn position")]
    public int _MaximumCountPlayers;
    [SerializeField] internal int _CountPlayersNow;
    [SerializeField] internal Transform[] _SpawnPoints;
    int[] _RandomPoints;
    int _ID_NPC;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        Instance = this;
        _SpawnPoints = GameObject.FindWithTag("Respawn").GetComponentsInChildren<Transform>();
        _RandomPoints = new int[_SpawnPoints.Length - 1];
        _HeaderWayPoints = GameObject.FindWithTag("Waypoints");

        if (_HeaderWayPoints)
        {
            PlayerTargetPoint[] _tp = _HeaderWayPoints.GetComponentsInChildren<PlayerTargetPoint>();
            foreach (PlayerTargetPoint _p in _tp)
            {
                _Targets.Add(_p);
            }
        }

        RandomValueForPosition();
        if (_Spawn)
        {
            AddList();
        }
            
    }

    /// <summary>
    /// Метод добавления игрока в список целей.
    /// </summary>
    void AddList()
    {
        if (!_PlayerSpawned)
        {
            Spawn(0, _RandomPoints[_CountPlayersNow], -1, true);
        }
        for (int i = _CountPlayersNow; i < _MaximumCountPlayers; i++)
        {
            if (i < _MaximumCountPlayers)
            {
                int c = Random.Range(0, _Cars.Length);
                Spawn(c, _RandomPoints[_CountPlayersNow], (i - 1), false);
            }
        }
        
    }
    void AddList(int _ID)
    {
        int c = Random.Range(0, _Cars.Length);
        Spawn(c, _RandomPoints[_CountPlayersNow], _ID, false);
    }

    /// <summary>
    /// Метод вычисления случайного числа позиции без совпадения.
    /// </summary>
    void RandomValueForPosition()
    {
        for (int i = 0; i < _RandomPoints.Length; i++)
        {
            int _p = Random.Range(1, _SpawnPoints.Length);
            _RandomPoints[i] = _p;
            if (i > 0)
            {
                for (int j = 0; j < i; j++)
                {
                    if (_RandomPoints[i] == _RandomPoints[j])
                    {
                        i--;
                        break;
                    }                    
                    
                }
            }
        }        
    }

    /// <summary>
    /// Метод спауна игрока (NPC) на сцену.
    /// </summary>
    void Spawn(int i, int j, int _ID, bool _player)
    {
        GameObject _car, _waypoint;
        if (_player)
        {
            _car = Instantiate(_PlayerCars[i], _SpawnPoints[j].position, Quaternion.identity) as GameObject;
        }
        else
        {
            _car = Instantiate(_Cars[i], _SpawnPoints[j].position, Quaternion.identity) as GameObject;
            _waypoint = Instantiate(_CurrentWayPoint, transform.position, Quaternion.identity) as GameObject;
            NPCCalculatePath.Instance._CurrentWayPoints.Add(_waypoint.transform);

            if (NPCCalculatePath.Instance._MaximumNPC != _MaximumCountPlayers - 1)
            {
                NPCCalculatePath.Instance._MaximumNPC = _MaximumCountPlayers - 1;
            }
            
            CarAIControl _AI = _car.GetComponent<CarAIControl>();
            _AI.SetTarget(_waypoint.transform);
            _car.GetComponent<CarInfo>()._ID = _ID;
        }
        _Targets[_CountPlayersNow]._PlayerTransform = _car.transform;
        _Targets[_CountPlayersNow].Set();
        _CountPlayersNow++;
    }

    /// <summary>
    /// Метод удаления игрока из списка целей.
    /// </summary>
    public void RemovePlayer(bool _Player, int _ID)
    {
        _CountPlayersNow--;
        if (!_Player)
        {
            AddList(_ID);
        }       
        
    }

}
