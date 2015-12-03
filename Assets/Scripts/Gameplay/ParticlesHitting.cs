using System;
using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class ParticlesHitting : MonoBehaviour
{
    public static ParticlesHitting Instance;

    
    [SerializeField] internal AudioClip[] _HitColliderSound;
    [SerializeField] internal AudioClip[] _HitShootSound;
    [SerializeField] internal GameObject _HitParticle;
    [SerializeField] internal GameObject _ShootHitParticle;
    [SerializeField] internal GameObject[] _Explosions;

    
    void Awake()
    {
        Init();
    }

    void Init()
    {
        Instance = this;               
    }

    /// <summary>
    /// Метод "выброса" частиц на месте столкновения collider'ов.
    /// </summary>
    /// <param name="_col">Collider</param>
    public void Hitting(Collision _col, CarInfo _car, WeaponRotate _weaponRotate)
    {
        if (GameSettings.Instance._Particles)
        {       
            if (_col.contacts.Length > 0 && _HitParticle)
            {
                double _contacts = Math.Round(_col.contacts.Length / 2.0);

                for (int i = 0; i < _contacts; i++)
                {
                    if (_car._Visibled)
                    {
                        GameObject _hit = Instantiate(_HitParticle, _col.contacts[i].point, Quaternion.identity) as GameObject;
                    }                        
                    
                }
            }
        }
        if(!_car._Player && _weaponRotate._DistanceToPlayer > 0.0f)
            AudioController.Instance.PlayOneShot(_HitColliderSound[UnityEngine.Random.Range(0, _HitShootSound.Length + 1)], 0.25f * _weaponRotate._DistanceToPlayer);
        else if(_car._Player)
            AudioController.Instance.PlayOneShot(_HitColliderSound[UnityEngine.Random.Range(0, _HitShootSound.Length + 1)], 0.25f);
    }

    /// <summary>
    /// Метод спавна частиц при попадании "пуль".
    /// </summary>
    /// <param name="_hitPoint">Точка попадания</param>
    /// <param name="_quat">Необходимое вращение</param>
    public void ShootHit(Vector3 _hitPoint, Quaternion _quat, bool _visibled)
    {
        if (GameSettings.Instance._Particles)
        {
            if (_visibled) Instantiate(_ShootHitParticle, _hitPoint, _quat);
        }
    }

    /// <summary>
    /// Спавн взрыва.
    /// </summary>
    /// <param name="_position">Позиция взрыва</param>
    /// <param name="_quat">Вращение взрыва</param>
    /// <param name="_index">Индекс с массива частиц взрыва</param>
    public void Explosion(Vector3 _position, Quaternion _quat, int _index, CarInfo _car)
    {
        if (GameSettings.Instance._Particles)
        {
            if (_car._Visibled) Instantiate(_Explosions[_index], _position, _quat);
        }
    }
}
