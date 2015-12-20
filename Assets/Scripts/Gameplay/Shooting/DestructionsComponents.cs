using UnityEngine;

public class DestructionsComponents : MonoBehaviour
{
    private static DestructionsComponents _DestructionsComponents;
    public static DestructionsComponents Instance
    {
        get
        {
            if (_DestructionsComponents != null)
            {
                return _DestructionsComponents;
            }
            else
            {
                _DestructionsComponents = new GameObject("_DestructionsComponents", typeof(DestructionsComponents)).GetComponent<DestructionsComponents>();
                _DestructionsComponents.transform.SetParent(GameObject.FindWithTag("InstanceLogics").transform);
                return _DestructionsComponents;
            }
        }
    }

    internal HitBoxComponents _HitBoxComponents;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        if (!_DestructionsComponents)
        {
            _DestructionsComponents = this;
        }
    }

    internal void PrepareToRepair(HitBoxComponents _hitBoxComponents)
    {
        _HitBoxComponents = _hitBoxComponents;
        Repair();
    }

    void Repair()
    {
        _HitBoxComponents._FactorForDestruct = _HitBoxComponents._Components.Count;
        _HitBoxComponents._IndexComponent = 0;
        for (int i = 0; i < _HitBoxComponents._Components.Count; i++)
        {
            if (!_HitBoxComponents._Components[i].activeSelf)
            {
                _HitBoxComponents._Components[i].SetActive(true);
                if (_HitBoxComponents._Components[i].CompareTag("Wheel"))
                {
                    _HitBoxComponents._HitBox._Car._WheelColliders[_HitBoxComponents._Components[i].GetComponent<WheelIndex>()._Index].gameObject.SetActive(true);
                }
            }
        }
    }


    internal void PrepareToDestruct(HitBoxComponents _hitBoxComponents, bool _player, bool _die)
    {
        _HitBoxComponents = _hitBoxComponents;
        DestroyComponent(_player, _die);
    }

    void DestroyComponent(bool _player, bool _die)
    {
        if (GameSettings.Instance._Destructions)
        {
            _HitBoxComponents._FactorForDestruct--;
            GameObject _component = _HitBoxComponents._Components[_HitBoxComponents._IndexComponent];

            if (_component.CompareTag("Wheel"))
            {
                _HitBoxComponents._HitBox._Car._WheelColliders[_component.GetComponent<WheelIndex>()._Index].gameObject.SetActive(false);
            }

            GameObject _clonedComponent = Instantiate(_component, _component.transform.position, _component.transform.rotation) as GameObject;
            _clonedComponent.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            _component.SetActive(false);

            if (_HitBoxComponents._HitBox._CarInfo._Visibled)
            {
                _clonedComponent.AddComponent<BoxCollider>();
                _clonedComponent.AddComponent<Rigidbody>().mass = 100.0f;
                _clonedComponent.AddComponent<PhysicsComponent>();
                _clonedComponent.GetComponent<MeshRenderer>().material.color = _HitBoxComponents._ColorComponent;
                Destroy(_clonedComponent, _HitBoxComponents._TimeToDeactivate);
            }
            else
            {
                Destroy(_clonedComponent);
            }
            _HitBoxComponents._IndexComponent++;
        }
        if (!_die)
        {
            NotifyMessage(_player);
        }
    }



    void NotifyMessage(bool _player)
    {
        if (_HitBoxComponents._HitBox._CarInfo._Player && !_player)
        {
            TextForNotify.Instance.PushText(1);
        }
        else
        if (!_HitBoxComponents._HitBox._CarInfo._Player && _player)
        {
            TextForNotify.Instance.PushText(0);
        }
    }
}
