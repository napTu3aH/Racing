using UnityEngine;
using System.Collections.Generic;

public class RespawnCars : MonoBehaviour
{
    public static RespawnCars Instance;

    [Header("Spawn")]
    [SerializeField] internal bool _Spawn;
    [SerializeField] internal bool _SpawnCycleNPC;
    [SerializeField] internal bool _PlayerSpawned;
    [SerializeField] internal List<PlayerTargetPoint> _Targets;

    [Header("Prefabs")]
    [SerializeField] internal GameObject[] _PlayerCars;
    [SerializeField] internal GameObject[] _Cars;
    [SerializeField] internal GameObject _CurrentWaypointPrefab;

    [Header("Count players and spawn position")]
    [SerializeField] internal int _MaximumCountPlayers;
    [SerializeField] internal int _MaximumCountNPC;
    [SerializeField] internal int _CountPlayersNow;
    [SerializeField] internal int _CountNPCNow;

    [SerializeField] internal Transform[] _SpawnPoints;
    [SerializeField] internal List<Transform> _CarsOnScene;
    [SerializeField] internal List<Transform> _CurrentWayPoints;


    int[] _RandomPoints;
    int _ID_NPC;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        Instance = this;
        _RandomPoints = new int[_SpawnPoints.Length];
        RandomValueForPosition();

        _MaximumCountNPC = _MaximumCountPlayers - 1;
        if (_Spawn)
        {
            if (NPCCalculatePath.Instance._MaximumNPC != _MaximumCountNPC)
            {
                NPCCalculatePath.Instance._MaximumNPC = _MaximumCountNPC;
            }
            SpawnPlayer();
            for (int i = 0; i < _MaximumCountNPC; i++)
            {
                SpawnNPC(i);
            }
        }
    }

    /// <summary>
    /// Метод вычисления случайного числа позиции без совпадения.
    /// </summary>
    void RandomValueForPosition()
    {
        for (int i = 0; i < _RandomPoints.Length; i++)
        {
            int _p = Random.Range(0, _SpawnPoints.Length);
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

    void SpawnPlayer()
    {
        if (!_PlayerSpawned)
        {
            Spawn(0, _RandomPoints[_CountPlayersNow], -1, true);
        }
    }

    void SpawnNPC(int _ID)
    {
        int c = Random.Range(0, _Cars.Length);
        Spawn(c, _RandomPoints[_CountPlayersNow], _ID, false);
    }

    /// <summary>
    /// Метод удаления игрока из списка целей.
    /// </summary>
    internal void RemoveCar(bool _Player, int _ID, Transform _CarTransform)
    {
        _CountPlayersNow--;
        _CarsOnScene.Remove(_CarTransform);
        PlayerTargetPoint _tp = _CarTransform.GetComponentInChildren<PlayerTargetPoint>();
        _Targets.Remove(_tp);
        if (!_Player)
        {
            NPCCalculatePath.Instance._NavPoints.Remove(_tp.transform);
            _CountNPCNow--;
            if (_SpawnCycleNPC)
            {
                SpawnNPC(_ID);
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
            _car = Instantiate(_PlayerCars[i], _SpawnPoints[j].position, _SpawnPoints[j].rotation) as GameObject;
        }
        else
        {
            _car = Instantiate(_Cars[i], _SpawnPoints[j].position, _SpawnPoints[j].rotation) as GameObject;
            if (_CurrentWayPoints.Count < _MaximumCountNPC)
            {
                _waypoint = Instantiate(_CurrentWaypointPrefab, transform.position, Quaternion.identity) as GameObject;
                _CurrentWayPoints.Add(_waypoint.transform);
            }

            CarAiController _AI = _car.GetComponent<CarAiController>();
            _AI._ID = _ID;
            _AI.SetTarget(_CurrentWayPoints[_CountNPCNow]);            
            _CountNPCNow++;
        }
        _Targets.Add(_car.GetComponentInChildren<PlayerTargetPoint>());
        _Targets[_CountPlayersNow].Set();
        _CarsOnScene.Add(_car.transform);
        _CountPlayersNow++;
    }
}
