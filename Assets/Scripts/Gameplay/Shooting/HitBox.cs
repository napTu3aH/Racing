using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
/// <summary>
/// Класс обработки урона по игроку.
/// </summary>
public class HitBox : MonoBehaviour {

    internal CarController _Car;
    BackDrive _CarBackDrive;
    [SerializeField] internal CarInfo _CarInfo;
    [SerializeField] internal WeaponRotate _CarWeapon;
    [SerializeField] internal float _ArmorFactor = 1.0f;
    [SerializeField] internal float _HitBoxHealth = 100.0f;
    [SerializeField] internal float _UnDamagedFactor = 0.0f;
    [SerializeField] internal float _HealthFactor;

    bool _CoroutineColor;
    public Color[] _Colors;
    public Color _CurrentColour;
    Color _ColorStart, _ColorEnd, _ColorChange;

    internal MeshRenderer _Mesh;
    internal Collider _Collider;
    internal Material _HitBoxMaterial;
    internal HitBoxComponents _HitBoxComponent;
    internal float _TimeChange = 0.0f;
    ParticleSystem _Particle;

    float _MaxHP;

    void Awake()
    {
        Init();
    }

    void Init()
    {

        _CarInfo = transform.root.GetComponent<CarInfo>();

        _Car = _CarInfo.GetComponent<CarController>();
        _CarWeapon = _CarInfo.GetComponent<WeaponRotate>();

        _Mesh = GetComponent<MeshRenderer>();
        _Collider = GetComponent<Collider>();  
        _HitBoxComponent = GetComponent<HitBoxComponents>();

        if (!_CarInfo._Player)
        {
            _CarBackDrive = _CarInfo.GetComponent<BackDrive>();
        }

        _HitBoxMaterial = _Mesh.material;

        _ColorStart = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        _ColorEnd = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        if (_Colors.Length > 0)
        {
            _CurrentColour = _Colors[0];
        }

        if (transform.name == "Forward")
        {
            _Particle = transform.GetComponentInChildren<ParticleSystem>();
            if (_Particle.gameObject.activeSelf)
            {
                _Particle.gameObject.SetActive(false);
            }
        }
        _MaxHP = _HitBoxHealth;
        Counting();
        _HitBoxComponent.Init();
        _HealthFactor = _HitBoxHealth;
    }

    public void DieHitBox()
    {
        _HitBoxHealth = 0.0f;
        Counting();
        for (int i = 0; i < _HitBoxComponent._FactorForDestruct; i++)
        {
            _HitBoxComponent.DestructComponent(_HitBoxHealth, false, true);
        }
        Destroy(_HitBoxComponent);
        Destroy(_Mesh);
        Destroy(this);  
    }

    public void Repair()
    {
        _HitBoxHealth = _MaxHP;
        Counting();
        _CarInfo.InitCounting();
        _HitBoxComponent.ReturnToBack();
    }



    /// <summary>
    /// Передача урона игроку.
    /// </summary>
    public void Hitted(float _damage, CarInfo _Info)
    {
        _damage = _damage * _Info._DamageFactor;
        Damage(_damage, _Info._Player);
        Counting();
        if (_CarInfo._Health <= 0.0f && _CarInfo._isAlive)
        {
            _CarInfo.DiePlayer(_Info);
        }
    }

    void Damage(float _damage, bool _Player)
    {
        float _CountedDamage = _damage / _ArmorFactor;

        if (_HitBoxHealth > _CountedDamage)
        {
            _HitBoxHealth -= _CountedDamage;
            if (_HitBoxHealth < _CountedDamage)
            {
                _HitBoxHealth = 0.0f;
                _ArmorFactor = 1.0f;
            }
        }
      
        _CarInfo.Counting(_CountedDamage);
        if (_Player)
        {
            float _points = _CountedDamage / _CarInfo._PercentHealthFactor;
            GameplayInfo.Inscante.StartCounting(_points);
        }
        _HitBoxComponent.DestructComponent(_HitBoxHealth, _Player, false);
    }
    /// <summary>
    /// Метод подсчёта значений защиты.
    /// </summary>
    void Counting()
    {
        _ArmorFactor = 1 + (_HitBoxHealth / 100.0f);
        _UnDamagedFactor = _ArmorFactor - 1;
        ColoringBox();
        if (transform.name == "Forward")
        {
            if (GameSettings.Instance._Particles && _Particle)
            {
                if (_HitBoxHealth == 0.0f && !_Particle.gameObject.activeSelf)
                {
                     _Particle.gameObject.SetActive(true);                    
                }
                else
                    if (_HitBoxHealth > 0.0f && _Particle.gameObject.activeSelf)
                {
                    _Particle.gameObject.SetActive(false);                    
                }
            }
        }

        if (transform.name == "Right")
        {
            _Car._FactorRight = _UnDamagedFactor;
        }

        if (transform.name == "Left")
        {
            _Car._FactorLeft = _UnDamagedFactor;
        }

    }

    void ColoringBox()
    {
        if (_HitBoxHealth >= _HealthFactor / 2.0f)
        {
            float _value = (1.0f - (_HitBoxHealth / _HealthFactor)) * 2.0f;
            ChangeColor(_Colors[0], _Colors[1], _value);
        }
        else
        if (_HitBoxHealth < _HealthFactor / 2.0f && _HitBoxHealth > 0.1f)
        {
            float _value = (1.0f - (_HitBoxHealth / (_HealthFactor / 2.0f)));
            ChangeColor(_Colors[1], _Colors[2], _value);
        }
        else
        if (_HitBoxHealth <= 0.1f && _HitBoxMaterial.color != _CurrentColour)
        {
            if (_CoroutineColor)
            {
                _CoroutineColor = false;
            }
            _CurrentColour = _Colors[2];
            _ColorStart = _CurrentColour;
            _HitBoxMaterial.color = _ColorStart;
        }
    }

    /// <summary>
    /// Метод подсчёта цвета в зависимости от здоровья.
    /// </summary>
    /// <param name="_currentColor">Начальный цвет</param>
    /// <param name="_nextColor">Конечный цвет</param>
    /// <param name="_value">Значение</param>
    void ChangeColor(Color _currentColor, Color _nextColor, float _value)
    {
        _CurrentColour = Color.Lerp(_currentColor, _nextColor, _value);
        _ColorStart = _CurrentColour;
        _TimeChange = 0.0f;
        if (!_CoroutineColor)
        {
            _CoroutineColor = true;
            StartCoroutine(ColorChanger());
        }
    }

    /// <summary>
    /// Сопрограмма подкрутки значения цвета у HitBox'а.
    /// </summary>
    /// <returns></returns>
    IEnumerator ColorChanger()
    {
        while (_TimeChange < 1.0f)
        {
            _TimeChange += Time.deltaTime * 0.5f;

            _ColorChange = Color.Lerp(_ColorStart, _ColorEnd, _TimeChange);
            _HitBoxMaterial.color = _ColorChange;
            if (_HitBoxHealth < 0.1f)
            {
                _CurrentColour = _Colors[2];
                _ColorStart = _CurrentColour;
                _HitBoxMaterial.color = _ColorStart;
                _CoroutineColor = false;
            }
            yield return null;
        }
        _TimeChange = 0.0f;
        _CoroutineColor = false;
        yield return null;
    }

    void OnTriggerEnter(Collider _col)
    {
        if (_col.CompareTag("HitBox"))
        {
            if (_CarInfo._Player)
            {
                GameSettings.Instance.Vibrate();                
            }
            else
            {
                NPCCalculatePath.Instance.PathUpdate(_CarInfo._ID);                              
                if (transform.name == "Forward")
                {
                    _CarBackDrive.Staying();
                }
            }

            _CarWeapon.ChangeTarget(_col);
            HitBox _box = _col.GetComponent<HitBox>();
            if (_box)
            {
                _box.Hitted(_CarInfo._CarSpeed, _CarInfo);
            }            
        }
    }

    void OnTriggerExit(Collider _col)
    {
        if (_col.CompareTag("HitBox"))
        {
            if (!_CarInfo._Player)
            {
                _CarBackDrive.UnStaying();
            }
        }
    }
}
