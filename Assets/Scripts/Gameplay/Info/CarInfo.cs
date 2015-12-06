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
        internal CarController _Car;
        internal BackDrive _BackDrive;
        internal CarAIControl _AiLogic;
        internal CarAudioManager _CarAudio;
        internal CarSelfRighting _CarSelfRighting;        
        internal WeaponRotate _WeaponRotate;
        public int _ID;
        public bool _Player, _isAlive;
        [Header("Healths")]
        public float _Health = 0.0f, _PercentHealthFactor, _CurrentHealth;
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
        
        bool _UpdatedPath;
        SlowMotionClass _SlowMotion;

        void Awake()
        {
            Init();
        }

        void Init()
        {
            _Car = GetComponent<CarController>();
            _WeaponRotate = GetComponent<WeaponRotate>();
            _CarAudio = GetComponent<CarAudioManager>();
            _CarSelfRighting = GetComponent<CarSelfRighting>();
            if (_Player)
            {
                _Visibled = true;
                CarUserControl.Instance.CarSet(_Car, transform);                
                SpawnPlayers.Instance._PlayerSpawned = true;
                _SlowMotion = GetComponentInChildren<SlowMotionClass>();

            }
            else
            {
                NPCCalculatePath.Instance._NPC_Cars.Add(this.transform);
                _AiLogic = GetComponent<CarAIControl>();
                _BackDrive = GetComponent<BackDrive>();
            }

            _HitBoxParent = transform.SearchChildWithTag("HitBoxsParent");
            _HitBoxs = _HitBoxParent.GetComponentsInChildren<HitBox>();

            InitCounting();
            _isAlive = true;
        }

        /// <summary>
        /// Метод подсчёта значений здоровья и процентажа здоровья при старте.
        /// </summary>
        public void InitCounting()
        {
            _CurrentHealth = 0.0f;
            _PercentHealthFactor = 0.0f;
            foreach (HitBox _ht in _HitBoxs)
            {
                _CurrentHealth += _ht._HitBoxHealth;
                _PercentHealthFactor += _ht._ArmorFactor;
            }
            _PercentHealthFactor -= _HitBoxs.Length;
            Counting(0);

        }

        void Start()
        {
            if (!_Player)
            {
                StartCoroutine(NPC_Updater());
            }
        }

        public void Counting(float _damage)
        {
            _CurrentHealth -= _damage;
            _Health = _CurrentHealth / _PercentHealthFactor;
            float _currentTopSpeed = _TopSpeedMax * (_Health / 100.0f);
            _Car.TopSpeed = Mathf.Clamp(_currentTopSpeed, 50.0f, Mathf.Infinity);

            if (_Player)
            {
                float _hp = 1.0f - (_Health / 100.0f);
                FrostEffect.Instance.FrostAmount = _hp;
            }
        }

        public void DiePlayer(CarInfo _info)
        {
            _isAlive = false;
            if (!_Player)
            {
                if (_info._Player) GameplayInfo.Inscante.Kills();
                NPCCalculatePath.Instance.RemoveNPC(_ID, transform, _AiLogic, _BackDrive);
            }
            else
            {
                Destroy(_SlowMotion);
            }

            SpawnPlayers.Instance.RemoveCar(_Player, _ID, transform);

            BonusesLogic.Instance.SpawnBonus(transform);

            ParticlesHitting.Instance.Explosion(transform.position, transform.rotation, 0, this);

            for (int i = 0; i < _HitBoxs.Length; i++)
            {
                _HitBoxs[i].DieHitBox();
            }
            GetComponent<Rigidbody>().mass *= 5.0f;
            Destroy(_Car);
            Destroy(_WeaponRotate._ShootingScript);
            Destroy(_WeaponRotate);
            Destroy(_CarSelfRighting);
            _CarAudio.Destroying();
            gameObject.AddComponent<CarDestroyed>();
            Destroy(this);
        }

        void OnCollisionEnter(Collision _col)
        {
            ParticlesHitting.Instance.Hitting(_col, this, _WeaponRotate);
        }

        IEnumerator NPC_Updater()
        {
            while (_isAlive)
            {
                if (!_UpdatedPath)
                {
                    _UpdatedPath = true;
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
            _UpdatedPath = false;
        }
    }
}

