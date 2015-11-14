using UnityEngine;
using System.Collections;

public class PlayerTargetPoint : MonoBehaviour {

    internal Transform _PlayerTransform;

    void Start()
    {
        _PlayerTransform = GameObject.FindWithTag("Player").GetComponent<Transform>();
        StartCoroutine(WaitAndPrint(2.0F));
    }
    IEnumerator WaitAndPrint(float waitTime)
    {
        while (_PlayerTransform)
        {
            transform.position = _PlayerTransform.position;
            yield return null;
        }
        yield return null;
    }
}
