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
        internal CarAIControl _AiLogic;
        internal BackDrive _BackDrive;
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
        bool _UpdatedPath;

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
                CarUserControl.Instance.CarSet(_Car, transform);                
                SpawnPlayers.Instance._PlayerSpawned = true;

                _ColorHealthImage = new Color(1.0f, 1.0f, 1.0f, 0.0f);
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
        void InitCounting()
        {
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

            /*if (_Player)
            {
                _ColorHealthImage.a = 1.0f - (_Health / 100.0f);
                _PlayerHealthImage.color = _ColorHealthImage;
            }*/
        }

        public void DiePlayer(CarInfo _info)
        {
            _isAlive = false;
            if (!_Player)
            {
                if (_info._Player) GameplayInfo.Inscante.Kills();
                NPCCalculatePath.Instance.RemoveNPC(_ID, transform, _AiLogic, _BackDrive);
            }

            SpawnPlayers.Instance.RemoveCar(_Player, _ID, transform);
            //Destroy(this.gameObject);

            ParticlesHitting.Instance.Explosion(transform.position, transform.rotation, 0, this);
            Destroy(_Car);
            for (int i = 0; i < _HitBoxs.Length; i++)
            {
                _HitBoxs[i].DieHitBox();
            }
            Destroy(_WeaponRotate._ShootingScript);
            Destroy(_WeaponRotate);
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

