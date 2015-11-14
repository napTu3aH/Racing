using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
/// <summary>
/// Класс, хранящий и взаимодействующий с характеристиками игрока.
/// </summary>
public class CarInfo : MonoBehaviour {

    internal CarController _Car;

    public bool _Player, _isAlive;
    public float _Health = 0.0f, _PercentHealthFactor, _CurrentHealth;
    public Transform _HitBoxParent;
    public HitBox[] _HitBoxs;
    public float _CarSpeed
    {
        set { value = _Car.CurrentSpeed; }
        get { return _Car.CurrentSpeed; }
    }
    public float _TopSpeed = 100.0f;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        _Car = GetComponent<CarController>();

        if (_Player)
        {
            CarUserControl.Instance.m_Car = _Car;
            CarGUI.Instance._Car = _Car;
            CarUserControl.Instance._CameraTarget = transform.SearchChildWithTag("Target");
            CarUserControl.Instance.SetCamera();
            SpawnPlayers.Instance._PlayerSpawned = true;
        } 
        _HitBoxParent = transform.SearchChildWithTag("HitBoxsParent");
        _HitBoxs = _HitBoxParent.GetComponentsInChildren<HitBox>();
        foreach (HitBox _ht in _HitBoxs)
        {
            _CurrentHealth += _ht._HitBoxHealth;
            _PercentHealthFactor += _ht._ArmorFactor;
        }
        _PercentHealthFactor -= _HitBoxs.Length;
        _Health = _CurrentHealth / _PercentHealthFactor;
        _Car.TopSpeed = _TopSpeed * (_Health / 100.0f);
        _isAlive = true;
    }

    public void DiePlayer()
    {
        _isAlive = false;
        SpawnPlayers.Instance.RemovePlayer();
        CarController _car = GetComponent<CarController>();
        _car.enabled = false;
        if (!_Player)
        {
            CalculatePath _calc = GetComponent<CalculatePath>();
            CarAIControl _carAi = GetComponent<CarAIControl>();
            _carAi.enabled = false;
            Destroy(_calc._CurWayPoint.gameObject);
        }
        Destroy(this.gameObject);        
    }

}
