using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(HitBox))]
public class HitBoxComponents : MonoBehaviour
{
    [SerializeField] internal HitBox _HitBox;
    [SerializeField] internal Transform _ParentMeshes;
    [SerializeField] internal List<GameObject> _Components;

    int _FactorForDestruct;
    float _HealthToDestruct;

    public void Init()
    {
        _HitBox = GetComponent<HitBox>();
        CountingHealthForDestruct();
    }

    [ContextMenu("Getting Components")]
    void GetMeshComponents()
    {
        if (_ParentMeshes)
        {
            MeshRenderer[] _meshRenderers = _ParentMeshes.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer _ms in _meshRenderers)
            {
                _Components.Add(_ms.gameObject);
            }
        }
    }

    void CountingHealthForDestruct()
    {
        if (_Components != null)
        {
            _FactorForDestruct = _Components.Count;
            _HealthToDestruct = _HitBox._HitBoxHealth / _FactorForDestruct;            
        }
    }

    public void DestructComponent(float _currentHealth)
    {
        if (_Components.Count > 0)
        {
            if (_currentHealth < _HealthToDestruct * _FactorForDestruct)
            {
                _FactorForDestruct--;
                GameObject _component = _Components[0];
                _component.transform.SetParent(null);
                _component.AddComponent<BoxCollider>();
                _component.AddComponent<Rigidbody>();

                _Components.Remove(_component);
                Destroy(_component, 10.0f);
            }
        }
    }
}
