using UnityEngine;
using System.Collections;

/// <summary>
/// Класс обработки урона по игроку.
/// </summary>
public class HitBox : MonoBehaviour {

    [SerializeField] internal CarInfo _CarInfo;
    [SerializeField] internal float _HealthFactor = 1.0f;
    [SerializeField] internal float _HitBoxHealth = 100.0f;
    [SerializeField] internal float _DamageFactor = 0.01f;
    internal Collider _Collider;
    void Awake()
    {
        _CarInfo = transform.root.GetComponent<CarInfo>();
        _Collider = GetComponent<Collider>();
        _HitBoxHealth *= _HealthFactor;
    }

    /// <summary>
    /// Передача урона игроку.
    /// </summary>
    public void Hitted(float _damage)
    {
        Damage(_damage);

        if (_CarInfo._Health <= 0.0f)
        {
            _CarInfo.DiePlayer();
        }
    }

    void Damage(float _damage)
    {
        if (_HitBoxHealth > _damage * _DamageFactor)
        {
            _DamageFactor = 100.0f / _HitBoxHealth;
            _HitBoxHealth -= _damage * _DamageFactor;
        }
        else
        {
            _HitBoxHealth = 0.0f;
            _DamageFactor = _HealthFactor * 10.0f;
        }
        _CarInfo._Health -= _damage * _DamageFactor;
        Debugger.Instance.Log("Damaged " + transform.root.tag + " in " + transform.name + " component: " + _damage * _DamageFactor + " Health: "+ _CarInfo._Health);
    }

    void OnTriggerEnter(Collider _col)
    {
        if (_col.CompareTag("HitBox"))
        {
            _col.GetComponent<HitBox>().Hitted(_CarInfo._CarSpeed);     
        }
    }
}
