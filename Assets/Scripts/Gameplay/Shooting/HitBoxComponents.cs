using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(HitBox))]
public class HitBoxComponents : MonoBehaviour
{
    [SerializeField] internal HitBox _HitBox;
    [SerializeField] internal Transform _ParentMeshes;
    [SerializeField] internal List<GameObject> _Components;
    [SerializeField] internal List<Vector3> _Positions;
    [SerializeField] internal float _TimeToDeactivate = 10.0f;

    internal Color _ColorComponent;
    internal int _FactorForDestruct, _IndexComponent;
    float _HealthToDestruct;

    public void Init()
    {
        _HitBox = GetComponent<HitBox>();
        _ColorComponent = new Color(0.15f, 0.15f, 0.15f);
        CountingHealthForDestruct();
    }

    [ContextMenu("Getting Components")]
    void GetMeshComponents()
    {
        if (_ParentMeshes)
        {
            if (_Components.Count > 0)
            {
                _Components.Clear();
            }
            MeshRenderer[] _meshRenderers = _ParentMeshes.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer _ms in _meshRenderers)
            {
                _Components.Add(_ms.gameObject);
                _Positions.Add(_ms.transform.position);
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

    public void DestructComponent(float _currentHealth, bool _player, bool _die)
    {

        if (_Components.Count > 0)
        {
            if (_currentHealth < _HealthToDestruct * _FactorForDestruct)
            {
                DestructionsComponents.Instance.PrepareToDestruct(this, _player, _die);
            }
        }       
    }

    public void ReturnToBack()
    {
        if (_Components.Count > 0)
        {
            DestructionsComponents.Instance.PrepareToRepair(this);
        }        
    }
}
