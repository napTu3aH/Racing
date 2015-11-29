using UnityEngine;
using System.Collections;

public class PlayerTargetPoint : MonoBehaviour
{

    [SerializeField]
    internal Transform _PlayerTransform;

    public void Set()
    {
        transform.parent = _PlayerTransform;
        transform.position = _PlayerTransform.position;
        NPCCalculatePath.Instance._NavPoints.Add(this.transform);
    }
}
