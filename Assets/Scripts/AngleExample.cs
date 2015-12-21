using UnityEngine;
using System.Collections;

public class AngleExample : MonoBehaviour {

    public Transform target;
    public float angle;
    void Update()
    {
        Vector3 targetDir = target.position - transform.position;
        Vector3 forward = transform.forward;
        Debug.DrawLine(transform.position, target.position, Color.red);
        angle = Vector3.Angle(targetDir, forward);
        if (angle < 5.0F)
            print("close");

    }
}
