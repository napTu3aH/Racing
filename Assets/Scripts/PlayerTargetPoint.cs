using UnityEngine;
using System.Collections;

public class PlayerTargetPoint : MonoBehaviour
{
    public void Set()
    {
        NPCCalculatePath.Instance._NavPoints.Add(this.transform);
    }
}
