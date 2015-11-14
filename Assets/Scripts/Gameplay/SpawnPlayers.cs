using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

/// <summary>
/// Класс создания игроков (NPC) на сцене.
/// </summary>
public class SpawnPlayers : MonoBehaviour {

    public static SpawnPlayers Instance;

    [Header("Spawn")]
    public bool _Spawn;
    public bool _PlayerSpawned;

    [Header("Prefabs")]
    [SerializeField] internal GameObject[] _PlayerCars;
    [SerializeField] internal GameObject[] _Cars;
    [SerializeField] internal GameObject _CurrentWayPoint;
    [SerializeField] internal GameObject _HeaderWayPoints;

    [Header("Count players and spawn position")]
    public int _MaximumCountPlayers;
    [SerializeField] internal int _CountPlayersNow;
    [SerializeField] internal Transform[] _SpawnPoints;
    int _tmp;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        Instance = this;
        _tmp = _SpawnPoints.Length + 1;
        _SpawnPoints = GameObject.FindWithTag("Respawn").GetComponentsInChildren<Transform>();
        _HeaderWayPoints = GameObject.FindWithTag("Waypoints");
        if (_Spawn) AddList();
    }

    /// <summary>
    /// Метод добавления игрока в список целей.
    /// </summary>
    public void AddList()
    {
        if (!_PlayerSpawned)
        {
            int p = Random.Range(0, _SpawnPoints.Length);
            Spawn(0, RandomValueForPosition(), true);
        }
        for (int i = _CountPlayersNow; i < _MaximumCountPlayers; i++)
        {
            if (i < _MaximumCountPlayers)
            {
                int c = Random.Range(0, _Cars.Length);
                Spawn(c, RandomValueForPosition(), false);
            }
        }
        
    }

    /// <summary>
    /// Метод вычисления случайного числа позиции без совпадения.
    /// </summary>
    int RandomValueForPosition()
    {
        int p = Random.Range(0, _SpawnPoints.Length);
        if (_tmp != p)
        {
            _tmp = p;
            return p;
        }
        else
        {
            RandomValueForPosition();
        }
        return 0;
    }

    /// <summary>
    /// Метод спауна игрока (NPC) на сцену.
    /// </summary>
    void Spawn(int i, int j, bool _player)
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
            CalculatePath _calc = _car.GetComponent<CalculatePath>();
            CarAIControl _AI = _car.GetComponent<CarAIControl>();
            _AI.SetTarget(_waypoint.transform);
            _calc._CurWayPoint = _waypoint.transform;
            _calc._HeadNavPoints = _HeaderWayPoints;
        }
        
        CarInfo _carInfo = _car.GetComponent<CarInfo>();

        
        _CountPlayersNow++;
        //TargetList.Instance._CarsInfo.Add(_carInfo);
    }

    /// <summary>
    /// Метод удаления игрока из списка целей.
    /// </summary>
    public void RemovePlayer()
    {
        _CountPlayersNow--;
        AddList();
    }

}
