using UnityEngine;
using System.Collections;

public class PhysicsComponent : MonoBehaviour
{
    Collider _Col;
    Rigidbody _Rig;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        _Col = GetComponent<Collider>();
        _Rig = GetComponent<Rigidbody>();
        StartCoroutine(RemoveComponents());
    }

    IEnumerator RemoveComponents()
    {
        yield return new WaitForSeconds(1.0f);
        while (_Rig.velocity.sqrMagnitude > 0.01f)
        {            
            yield return null;
        }
        Destroy(_Rig);
        Destroy(_Col);
        yield return null;
    }
}
