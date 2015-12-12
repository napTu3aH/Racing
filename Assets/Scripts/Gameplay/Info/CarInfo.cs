﻿using UnityEngine;
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
        //internal CarAIControl _AiLogic;
        internal CarAiController _AiLogic;
        internal CarAudioManager _CarAudio;
        internal CarSelfRighting _CarSelfRighting;        
        internal WeaponRotate _WeaponRotate;

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

        internal bool _Visibled;
        internal float _TopSpeedFactor = 1.0f, _TimeToping,
            _ArmorFactor = 1.0f, _TimeArmor,
            _DamageFactor = 1.0f, _TimeDamage;
        bool _TopSpeedCoroutine, _ArmorCoroutine, _DamageCoroutine;
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
                //SpawnPlayers.Instance._PlayerSpawned = true;
                RespawnCars.Instance._PlayerSpawned = true;
                _SlowMotion = GetComponentInChildren<SlowMotionClass>();

            }
            else
            {
                _AiLogic = GetComponent<CarAiController>();
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
        internal void InitCounting()
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

        internal void TopSpeedFactoring(float _newTopSpeed, float _time)
        {            
            if (!_TopSpeedCoroutine)
            {
                _TopSpeedCoroutine = true;
                _TopSpeedFactor = _newTopSpeed;
                _TimeToping = _time;
                Counting(0);
                StartCoroutine(TopSpeedCoroutine());
            }
        }

        internal void ArmorFactoring(float _newArmorFactor, float _time)
        {
            if (!_ArmorCoroutine)
            {
                _ArmorCoroutine = true;
                _ArmorFactor = _newArmorFactor;
                _TimeArmor = _time;
                Counting(0);
                StartCoroutine(ArmorCoroutine());
            }
        }

        internal void Repair()
        {
            foreach (HitBox _hitBox in _HitBoxs)
            {
                _hitBox.Repair();
            }
        }

        internal void Damage(float _damageFactor, float _time)
        {
            if (!_DamageCoroutine)
            {
                _DamageCoroutine = true;
                _DamageFactor = _damageFactor;
                _TimeDamage = _time;
                StartCoroutine(DamageFactor());
            }
        }

        internal void Counting(float _damage)
        {
            _CurrentHealth -= _damage;
            _Health = (_CurrentHealth / _PercentHealthFactor) * _ArmorFactor;
            float _currentTopSpeed = _TopSpeedMax * (_Health / 100.0f);
            _Car.TopSpeed = Mathf.Clamp(_currentTopSpeed, 50.0f, Mathf.Infinity) * _TopSpeedFactor;

            if (_Player)
            {
                float _hp = 1.0f - (_Health / 100.0f);
                //FrostEffect.Instance.FrostAmount = _hp;
            }
        }

        internal void DiePlayer(CarInfo _info)
        {
            _isAlive = false;
            if (!_Player)
            {
                if (_info._Player)
                {
                    GameplayInfo.Inscante.Kills();
                    TextForNotify.Instance.PushText(2);
                }
                NPCCalculatePath.Instance.RemoveNPC(_AiLogic, _BackDrive);
                RespawnCars.Instance.RemoveCar(_Player, _AiLogic._ID, transform);
                //SpawnPlayers.Instance.RemoveCar(_Player, _AiLogic._ID, transform);
            }
            else
            {
                Destroy(_SlowMotion);
                TextForNotify.Instance.PushText(3);
                RespawnCars.Instance.RemoveCar(_Player, -1, transform);
                //SpawnPlayers.Instance.RemoveCar(_Player, -1, transform);
            }

            BonusesLogic.Instance.SpawnBonus(transform, new Vector3(0.0f, 5.0f, 0.0f));

            ParticlesHitting.Instance.Explosion(transform.position, transform.rotation, 0, this);

            for (int i = 0; i < _HitBoxs.Length; i++)
            {
                _HitBoxs[i].DieHitBox();
            }
            Rigidbody _rig = GetComponent<Rigidbody>();
            _rig.drag = 15.0f;
            _rig.angularDrag = 15.0f;
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

        IEnumerator TopSpeedCoroutine()
        {
            yield return new WaitForSeconds(_TimeToping);
            _TopSpeedFactor = 1.0f;
            Counting(0);
        }

        IEnumerator ArmorCoroutine()
        {
            yield return new WaitForSeconds(_TimeArmor);
            _ArmorFactor = 1.0f;
            Counting(0);
        }

        IEnumerator DamageFactor()
        {
            yield return new WaitForSeconds(_TimeDamage);
            _DamageFactor = 1.0f;
        }

    }
}

