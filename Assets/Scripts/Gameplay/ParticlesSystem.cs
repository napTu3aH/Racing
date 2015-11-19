using UnityEngine;
using System.Collections;

public class ParticlesSystem : MonoBehaviour {


    [SerializeField] internal Collider _Collider;
    [SerializeField] internal GameObject _HitParticle;
    [SerializeField] internal GameObject _Wheel;
    [SerializeField] internal GameObject _ShootHitParticle;
    [SerializeField] internal GameObject[] _Explosions;

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
        if (GameSettings.Instance._Particles)
        {
            if (_col.contacts.Length > 0 && _HitParticle)
            {
                for (int i = 0; i < _col.contacts.Length; i++)
                {
                    GameObject _hit = Instantiate(_HitParticle, _col.contacts[i].point, Quaternion.identity) as GameObject;
                }
            }
        }        
    }

    public void ShootHit(Vector3 _hitPoint, Quaternion _quat)
    {
        if (GameSettings.Instance._Particles)
        {
            Instantiate(_ShootHitParticle, _hitPoint, _quat);
        }        
    }
    public void WheelSpawn(Vector3 _wheelPosition, Quaternion _wheelQuat)
    {
        if (GameSettings.Instance._Particles)
        {
            Instantiate(_Wheel, _wheelPosition, _wheelQuat);
        }
    }

    public void Explosion(Vector3 _position, Quaternion _quat, int _index)
    {
        if (GameSettings.Instance._Particles)
        {
            Instantiate(_Explosions[_index], _position, _quat);
        }
    }
}
