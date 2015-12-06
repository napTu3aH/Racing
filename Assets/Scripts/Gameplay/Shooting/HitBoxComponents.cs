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

    Color _ColorComponent;
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

    public void DestructComponent(float _currentHealth)
    {
        if (GameSettings.Instance._Destructions)
        {
            if (_Components.Count > 0)
            {
                if (_currentHealth < _HealthToDestruct * _FactorForDestruct)
                {
                    _FactorForDestruct--;
                    GameObject _component = _Components[_IndexComponent];
                    

                    if (_component.CompareTag("Wheel"))
                    {
                        _HitBox._Car._WheelColliders[_component.GetComponent<WheelIndex>()._Index].gameObject.SetActive(false);
                    }

                    GameObject _clonedComponent = Instantiate(_component, _component.transform.position, _component.transform.rotation) as GameObject;
                    _clonedComponent.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                    _component.SetActive(false);

                    if (_HitBox._CarInfo._Visibled)
                    {
                        _clonedComponent.AddComponent<BoxCollider>();
                        _clonedComponent.AddComponent<Rigidbody>().mass = 100.0f;
                        _clonedComponent.GetComponent<MeshRenderer>().material.color = _ColorComponent;
                        Destroy(_clonedComponent, _TimeToDeactivate);
                    }
                    else
                    {
                        Destroy(_clonedComponent);
                    }
                    _IndexComponent++;
                }
            }
        }       
    }

    public void ReturnToBack()
    {
        for (int i = 0; i < _Components.Count; i++)
        {
            if (!_Components[i].activeSelf)
            {
                _Components[i].SetActive(true);
                if (_Components[i].CompareTag("Wheel"))
                {
                    _HitBox._Car._WheelColliders[_Components[i].GetComponent<WheelIndex>()._Index].gameObject.SetActive(true);
                }
            }
        }
    }
}
