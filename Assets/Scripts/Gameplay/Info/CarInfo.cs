using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
/// <summary>
/// Класс, хранящий и взаимодействующий с характеристиками игрока.
/// </summary>
public class CarInfo : MonoBehaviour {

    internal CarController _Car;

    public int _ID;
    public bool _Player;
    public float _Health = 100.0f;
    public Transform _HitBoxParent;
    public HitBox[] _HitBoxs;
    public float _CarSpeed
    {
        set { value = _Car.CurrentSpeed; }
        get { return _Car.CurrentSpeed; }
    }
    

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
            _Health += _ht._HitBoxHealth;
        }
        
    }

    public void DiePlayer()
    {
        SpawnPlayers.Instance.RemovePlayer(this);
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
