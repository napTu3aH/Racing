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
        public int _ID;
        public bool _Player, _isAlive;
        [Header("Healths")]
        public float _Health = 0.0f, _PercentHealthFactor, _CurrentHealth;
        public Image _PlayerHealthImage;
        public Transform _HitBoxParent;
        public HitBox[] _HitBoxs;
        public float _CarSpeed
        {
            set { value = _Car.CurrentSpeed; }
            get { return _Car.CurrentSpeed; }
        }
        public float _TopSpeedMax = 100.0f;

        public float _TimeUpdateFactor;

        internal bool _Visibled;
        internal WeaponRotate _WeaponRotate;
        bool _Updated;

        void Awake()
        {
            Init();
        }

        void Init()
        {
            _Car = GetComponent<CarController>();
            _WeaponRotate = GetComponent<WeaponRotate>();
            if (_Player)
            {
                _Visibled = true;
                CarUserControl.Instance.m_Car = _Car;
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

            InitCounting();
            _isAlive = true;
        }

        /// <summary>
        /// Метод подсчёта значений здоровья и процентажа здоровья при старте.
        /// </summary>
        void InitCounting()
        {
            foreach (HitBox _ht in _HitBoxs)
            {
                _CurrentHealth += _ht._HitBoxHealth;
                _PercentHealthFactor += _ht._ArmorFactor;
            }
            _PercentHealthFactor -= _HitBoxs.Length;
            Counting();

        }

        void Start()
        {
            if (!_Player)
            {
                StartCoroutine(NPC_Updater());
            }
        }

        public void Counting()
        {
            _Health = _CurrentHealth / _PercentHealthFactor;
            float _currentTopSpeed = _TopSpeedMax * (_Health / 100.0f);
            _Car.TopSpeed = Mathf.Clamp(_currentTopSpeed, 50.0f, Mathf.Infinity);

            /*if (_Player)
            {
                _ColorHealthImage.a = 1.0f - (_Health / 100.0f);
                _PlayerHealthImage.color = _ColorHealthImage;
            }*/
        }

        public void DiePlayer()
        {
            _isAlive = false;
            ParticlesHitting.Instance.Explosion(transform.position, transform.rotation, 0, this);

            if (!_Player)
            {
                NPCCalculatePath.Instance.RemoveNPC(_ID, transform);
            }

            SpawnPlayers.Instance.RemoveCar(_Player, _ID, transform);
            Destroy(this.gameObject);
        }

        void OnCollisionEnter(Collision _col)
        {
            ParticlesHitting.Instance.Hitting(_col, this, _WeaponRotate);
        }

        IEnumerator NPC_Updater()
        {
            while (_isAlive)
            {
                if (!_Updated)
                {
                    _Updated = true;
                    Invoke("UpdatePath", _TimeUpdateFactor);
                }
                yield return null;
            }
            yield return null;
        }

        void UpdatePath()
        {          
            UpdateDistance();
            NPCCalculatePath.Instance.PathUpdate(_ID);
        }

        void UpdateDistance()
        {
            NPCCalculatePath.Instance.DistaceUpdate(_ID);
            _TimeUpdateFactor = 1.0f + (NPCCalculatePath.Instance._Distance[_ID] / 100.0f);
            _Updated = false;
        }
    }
}

