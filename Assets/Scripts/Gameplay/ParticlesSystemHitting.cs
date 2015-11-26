using UnityEngine;
using System.Collections;

public class ParticlesSystemHitting : MonoBehaviour {

    [SerializeField] internal Collider _Collider;
    [SerializeField] internal AudioClip[] _HitColliderSound;
    [SerializeField] internal AudioClip[] _HitShootSound;
    [SerializeField] internal GameObject _HitParticle;
    [SerializeField] internal GameObject _Wheel;
    [SerializeField] internal GameObject _ShootHitParticle;
    [SerializeField] internal GameObject[] _Explosions;
    [SerializeField] internal ParticleSystem[] _Muzzels;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        _Collider = transform.SearchChildWithTag("HitBoxsParent").GetComponent<Collider>();
    }

    void OnCollisionEnter(Collision _col)
    {
        Hitting(_col);        
    }

    /// <summary>
    /// Метод "выброса" частиц на месте столкновения collider'ов.
    /// </summary>
    /// <param name="_col">Collider</param>
    void Hitting(Collision _col)
    {
        if (GameSettings.Instance._Particles)
        {
            if (_col.contacts.Length > 0 && _HitParticle)
            {
                for (int i = 0; i < _col.contacts.Length; i++)
                {
                    GameObject _hit = Instantiate(_HitParticle, _col.contacts[i].point, Quaternion.identity) as GameObject;
                    AudioController.Instance.PlayOneShot(_HitColliderSound[Random.Range(0, _HitShootSound.Length + 1)], 0.25f);
                }
            }
        }
    }

    /// <summary>
    /// Метод спавна частиц при попадании "пуль".
    /// </summary>
    /// <param name="_hitPoint">Точка попадания</param>
    /// <param name="_quat">Необходимое вращение</param>
    public void ShootHit(Vector3 _hitPoint, Quaternion _quat)
    {
        if (GameSettings.Instance._Particles)
        {
            Instantiate(_ShootHitParticle, _hitPoint, _quat);
        }        
    }

    /// <summary>
    /// Спавн колёс.
    /// </summary>
    /// <param name="_wheelPosition">Позиция колеса</param>
    /// <param name="_wheelQuat">Вращение колеса</param>
    public void WheelSpawn(Vector3 _wheelPosition, Quaternion _wheelQuat)
    {
        if (GameSettings.Instance._Particles)
        {
            Instantiate(_Wheel, _wheelPosition, _wheelQuat);
        }
    }

    /// <summary>
    /// Спавн взрыва.
    /// </summary>
    /// <param name="_position">Позиция взрыва</param>
    /// <param name="_quat">Вращение взрыва</param>
    /// <param name="_index">Индекс с массива частиц взрыва</param>
    public void Explosion(Vector3 _position, Quaternion _quat, int _index)
    {
        if (GameSettings.Instance._Particles)
        {
            Instantiate(_Explosions[_index], _position, _quat);
        }
    }
}
