using UnityEngine;
using System.Collections;
/// <summary>
/// Класс, хранящий и взаимодействующий с характеристиками игрока.
/// </summary>

namespace UnityStandardAssets.Vehicles.Car
{
    public class CarInfo : MonoBehaviour
    {

        internal CarController _Car;

        public int _ID;
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

        public float _TimeUpdateFactor;
        float _TimeUpdate;

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
            else
            {
                NPCCalculatePath.Instance._NPC_Cars.Add(this.transform);
            }
            _HitBoxParent = transform.SearchChildWithTag("HitBoxsParent");
            _HitBoxs = _HitBoxParent.GetComponentsInChildren<HitBox>();
            Counting();
            _isAlive = true;
        }

        void Start()
        {
            if (!_Player)
            {
                StartCoroutine(MoveToPoint());
            }
        }

        void Counting()
        {
            foreach (HitBox _ht in _HitBoxs)
            {
                _CurrentHealth += _ht._HitBoxHealth;
                _PercentHealthFactor += _ht._ArmorFactor;
            }
            _PercentHealthFactor -= _HitBoxs.Length;
            _Health = _CurrentHealth / _PercentHealthFactor;
            _Car.TopSpeed = _TopSpeed * (_Health / 100.0f);
        }

        public void DiePlayer()
        {
            _isAlive = false;
            SpawnPlayers.Instance.RemovePlayer(_Player);
            CarController _car = GetComponent<CarController>();
            _car.enabled = false;
            if (!_Player)
            {
                CarAIControl _carAi = GetComponent<CarAIControl>();
                _carAi.enabled = false;
                NPCCalculatePath.Instance._CurrentWayPoints.Remove(NPCCalculatePath.Instance._CurrentWayPoints[_ID]);
                Destroy(NPCCalculatePath.Instance._CurrentWayPoints[_ID].gameObject);
            }
            Destroy(this.gameObject);
        }

        void FixedUpdate()
        {
            if (!_Player)
            {
                NPCCalculatePath.Instance.DistaceUpdate(_ID);
                _TimeUpdateFactor = 1.0f + (NPCCalculatePath.Instance._Distance[_ID] / 100.0f);
            }            
        }

        IEnumerator MoveToPoint()
        {        
            while (_isAlive)
            {                
                if (_TimeUpdate < _TimeUpdateFactor)
                {
                    _TimeUpdate += Time.deltaTime;
                    if (_TimeUpdate > _TimeUpdateFactor)
                    {                        
                        NPCCalculatePath.Instance.PathUpdate(_ID);
                        _TimeUpdate = 0.0f;
                        yield return null;
                    }
                }else
                    if (_TimeUpdate > _TimeUpdateFactor)
                {
                    NPCCalculatePath.Instance.PathUpdate(_ID);
                    _TimeUpdate = 0.0f;
                    yield return null;
                }

                yield return null;
            }
            yield return null;
        }
    }
}

