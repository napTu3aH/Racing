using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;
/// <summary>
/// Класс обработки урона по игроку.
/// </summary>
public class HitBox : MonoBehaviour {

    CarController _Car;
    BackDrive _CarBackDrive;
    [SerializeField] internal CarInfo _CarInfo;
    [SerializeField] internal float _ArmorFactor = 1.0f;
    [SerializeField] internal float _HitBoxHealth = 100.0f;
    [SerializeField] internal float _UnDamagedFactor = 0.0f;
    [SerializeField] internal float _HealthFactor;

    bool _CoroutineColor;
    public Color[] _Colors;
    public Color _CurrentColour;
    int _ColorIndex = 0;
    Color _ColorStart, _ColorEnd;
    internal Collider _Collider;
    internal Material _HitBoxMaterial;
    internal MeshRenderer _Mesh;
    float _TimeChange = 0.0f;

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
        _Collider = GetComponent<Collider>();
        _Mesh = GetComponent<MeshRenderer>();
        _HitBoxMaterial = _Mesh.material;

        _ColorStart = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        _ColorEnd = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        if (_Colors.Length > 0)
        {
            _CurrentColour = _Colors[0];
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

        if (_HitBoxHealth >= _HealthFactor / 2.0f)
        {
            float _value = (1.0f - (_HitBoxHealth / _HealthFactor)) * 2.0f;
            ChangeColor(_Colors[0], _Colors[1], _value);
        }
        else
        {
            float _value = (1.0f - (_HitBoxHealth / (_HealthFactor / 2.0f)));
            ChangeColor(_Colors[1], _Colors[2], _value);
        }

        if (_CarInfo._Health <= 0.0f && _CarInfo._isAlive)
        {
            _CarInfo.DiePlayer();
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
        if (!_CoroutineColor)
        {
            _CoroutineColor = true;
        }
    }


    void FixedUpdate()
    {
        if (_CoroutineColor)
        {
            if (_TimeChange < 1.0f)
            {
                _TimeChange += Time.deltaTime * 0.01f;
                _ColorStart.a = Mathf.Lerp(_ColorStart.a, _ColorEnd.a, _TimeChange);
                _HitBoxMaterial.color = _ColorStart;
            }
            else
            if (_TimeChange > 1.0f)
            {
                _TimeChange = 0.0f;
                _ColorStart.a = _TimeChange;
                _HitBoxMaterial.color = _ColorStart;
                Debug.Log(_TimeChange);
                _CoroutineColor = false;
                
            }
        }

    }

    IEnumerator ColorSetter()
    {
        while (_CoroutineColor)
        {
            if (_TimeChange < 1.0f)
            {
                _TimeChange += Time.deltaTime * 0.01f;
                _ColorStart.a = Mathf.Lerp(_ColorStart.a, _ColorEnd.a, _TimeChange);
                _HitBoxMaterial.color = _ColorStart;
                yield return null;
            }
            else
            if(_TimeChange > 1.0f)
            {
                _TimeChange = 0.0f;
                _ColorStart.a = _TimeChange;                                
                _HitBoxMaterial.color = _ColorStart;
                _CoroutineColor = false;
                yield return null;
            }            
        }        
        yield return null;
    }

    /// <summary>
    /// Метод подсчёта значений защиты.
    /// </summary>
    void Counting()
    {
        _ArmorFactor = 1 + (_HitBoxHealth / 100.0f);
        _UnDamagedFactor = _ArmorFactor - 1;
        
        if (transform.name == "Left")
        {
            _Car._FactorLeft = _UnDamagedFactor;
        }
        if (transform.name == "Right")
        {
            _Car._FactorRight = _UnDamagedFactor;
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
            if (!_CarInfo._Player)
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
