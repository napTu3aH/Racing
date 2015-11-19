using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
/// <summary>
/// Класс обработки урона по игроку.
/// </summary>
public class HitBox : MonoBehaviour {

    CarController _Car;
    BackDrive _CarBackDrive;
    ParticlesSystem _ParticlesSystem;
    [SerializeField] internal CarInfo _CarInfo;
    [SerializeField] internal float _ArmorFactor = 1.0f;
    [SerializeField] internal float _HitBoxHealth = 100.0f;
    [SerializeField] internal float _UnDamagedFactor = 0.0f;
    [SerializeField] internal float _HealthFactor;

    bool _CoroutineColor;
    public Color[] _Colors;
    public Color _CurrentColour;
    Color _ColorStart, _ColorEnd, _ColorChange;

    internal Collider _Collider;
    internal Material _HitBoxMaterial;
    internal MeshRenderer _Mesh;
    internal float _TimeChange = 0.0f;
    internal int _RandomValueLeftWheel, _RandomValueRightWheel, _RandomValueBackWheels;
    ParticleSystem _Particle;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        _CarInfo = transform.root.GetComponent<CarInfo>();

        if (!_CarInfo._Player)
        {
            _CarBackDrive = _CarInfo.GetComponent<BackDrive>();
        }

        _Car = transform.root.GetComponent<CarController>();
        _ParticlesSystem = _Car.GetComponent<ParticlesSystem>();
        _Collider = GetComponent<Collider>();
        _Mesh = GetComponent<MeshRenderer>();
        _HitBoxMaterial = _Mesh.material;

        _ColorStart = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        _ColorEnd = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        if (_Colors.Length > 0)
        {
            _CurrentColour = _Colors[0];
        }
        _RandomValueLeftWheel = Random.Range(0, 2);
        _RandomValueRightWheel = Random.Range(0, 2);
        _RandomValueBackWheels = Random.Range(0, 2);

        if (transform.name == "Forward")
        {
            _Particle = transform.GetComponentInChildren<ParticleSystem>();
            if (_Particle.gameObject.activeSelf)
            {
                _Particle.gameObject.SetActive(false);
            }
        }

        Counting();
        _HealthFactor = _HitBoxHealth;
    }

    /// <summary>
    /// Передача урона игроку.
    /// </summary>
    public void Hitted(float _damage)
    {
        Damage(_damage);
        Counting();
        ColoringBox();

        if (_CarInfo._Health <= 0.0f && _CarInfo._isAlive)
        {
            _CarInfo.DiePlayer();
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

    void SpawnWheel(int _wheelIndex)
    {
        _Car._WheelMeshes[_wheelIndex].SetActive(false);
        _Car._WheelColliders[_wheelIndex].gameObject.SetActive(false);
        _ParticlesSystem.WheelSpawn(_Car._WheelMeshes[_wheelIndex].transform.position, _Car._WheelMeshes[_wheelIndex].transform.rotation);
    }

    /// <summary>
    /// Метод подсчёта значений защиты.
    /// </summary>
    void Counting()
    {
        _ArmorFactor = 1 + (_HitBoxHealth / 100.0f);
        _UnDamagedFactor = _ArmorFactor - 1;

        if (transform.name == "Forward")
        {
            if (_HitBoxHealth == 0.0f && _Particle && GameSettings.Instance._Particles)
            {
                if (!_Particle.gameObject.activeSelf)
                {
                    _Particle.gameObject.SetActive(true);
                }
                
            }
        }

        if (transform.name == "Right")
        {
            _Car._FactorRight = _UnDamagedFactor;
            if (_HitBoxHealth == 0.0f && _Car._WheelMeshes[0].activeSelf)
            {
                if (_RandomValueRightWheel == 1)
                {
                    SpawnWheel(0);
                }
            }
        }

        if (transform.name == "Left")
        {
            _Car._FactorLeft = _UnDamagedFactor;
            if (_HitBoxHealth == 0.0f && _Car._WheelMeshes[1].activeSelf)
            {
                if (_RandomValueLeftWheel == 1)
                {
                    SpawnWheel(1);
                }
            }
        }
        
        if (transform.name == "Back")
        {
            if (_HitBoxHealth == 0.0f && (_Car._WheelMeshes[2].activeSelf && _Car._WheelMeshes[3].activeSelf))
            {
                if (_RandomValueBackWheels == 0)
                {
                    SpawnWheel(2);
                }
                else
                {
                    SpawnWheel(3);
                }
            }
        }

    }

    void Damage(float _damage)
    {
        float _tmp = _damage / _ArmorFactor;
        
        if (_HitBoxHealth > _tmp)
        {
            _HitBoxHealth -= _tmp;
            if (_HitBoxHealth < _tmp)
            {
                _HitBoxHealth = 0.0f;
                _ArmorFactor = 1.0f;
            } 
        }

        _CarInfo._CurrentHealth -= _tmp;
        _CarInfo._Health = _CarInfo._CurrentHealth / _CarInfo._PercentHealthFactor;
        _Car.TopSpeed = _CarInfo._TopSpeed *(_CarInfo._Health / 100.0f);
        Debugger.Instance.Log("Damaged " + transform.root.tag + " in " + transform.name + " component: " + _tmp + " Health: "+ _CarInfo._Health+"%");
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
            } 
            _col.GetComponent<HitBox>().Hitted(_CarInfo._CarSpeed);     
        }
    }

    void OnTriggerStay(Collider _col)
    {
        if (_col.CompareTag("HitBox"))
        {
            if (!_CarInfo._Player && transform.name == "Forward")
            {
                _CarBackDrive._StayTimer += Time.deltaTime;
            }
        }
    }

    void OnTriggerExit(Collider _col)
    {
        if (_col.CompareTag("HitBox"))
        {
            if (!_CarInfo._Player)
            {
                _CarBackDrive._StayTimer = 0.0f;
            }
        }
    }
}
