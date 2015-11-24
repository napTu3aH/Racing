using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/// <summary>
/// Класс, хранящий и взаимодействующий с характеристиками игрока.
/// </summary>

namespace UnityStandardAssets.Vehicles.Car
{
    public class CarInfo : MonoBehaviour
    {
        Color _ColorHealthImage;
        internal CarController _Car;
        internal ParticlesSystem _ParticlesSystem;

        public int _ID;
        public HitBox[] _HitBoxs;
        public Transform _HitBoxParent;
        public bool _Player, _isAlive;
        public Image _PlayerHealthImage;
        public float _Health = 0.0f, _PercentHealthFactor, _CurrentHealth;
               
        public float _CarSpeed
        {
            set { value = _Car.CurrentSpeed; }
            get { return _Car.CurrentSpeed; }
        }
        public float _TopSpeed = 100.0f;
        public float _TimeUpdateFactor, _TimeUpdate;     

        void Awake()
        {
            Init();
        }

        void Init()
        {
            _isAlive = true;
            _Car = GetComponent<CarController>();
            _ParticlesSystem = GetComponent<ParticlesSystem>();

            if (_Player)
            {
                _PlayerHealthImage = GameObject.FindWithTag("HealthBar").GetComponent<Image>();
                CarUserControl.Instance.m_Car = _Car;
                CarGUI.Instance._Car = _Car;
                CarUserControl.Instance._CameraTarget = transform.SearchChildWithTag("Target");
                CarUserControl.Instance.SetCamera();
                SpawnPlayers.Instance._PlayerSpawned = true;

                _ColorHealthImage = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            }
            else
            {
                NPCCalculatePath.Instance._NPC_Cars.Add(this.transform);
            }
            _HitBoxParent = transform.SearchChildWithTag("HitBoxsParent");
            _HitBoxs = _HitBoxParent.GetComponentsInChildren<HitBox>();
            Counting();            
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
            Count();
            
        }

        public void Count()
        {
            _Health = _CurrentHealth / _PercentHealthFactor;
            _Car.TopSpeed = _TopSpeed * (_Health / 100.0f);
            if (_Player)
            {
                _ColorHealthImage.a = 1.0f - (_Health / 100.0f);
                _PlayerHealthImage.color = _ColorHealthImage;
            }
        }

        public void DiePlayer()
        {
            _isAlive = false;
            WeaponRotate _rotate = GetComponent<WeaponRotate>();
            CarController _car = GetComponent<CarController>();
            
            if (!_Player)
            {
                NPCCalculatePath.Instance._NPC_Cars.Remove(transform);
                Destroy(NPCCalculatePath.Instance._CurrentWayPoints[_ID].gameObject);
                NPCCalculatePath.Instance._CurrentWayPoints.Remove(NPCCalculatePath.Instance._CurrentWayPoints[_ID]);
                CarAIControl _carAi = GetComponent<CarAIControl>();
                _carAi.enabled = false;
            }

            SpawnPlayers.Instance.RemovePlayer(_Player, _ID);
            _rotate._Cars.Remove(transform);
            _car.enabled = false;
            _ParticlesSystem.Explosion(transform.position, transform.rotation, 0);
            Destroy(this.gameObject);
        }

        void FixedUpdate()
        {
            if (!_Player && _isAlive)
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

