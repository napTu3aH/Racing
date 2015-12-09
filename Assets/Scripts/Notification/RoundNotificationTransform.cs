using UnityEngine;
using System.Collections;

public class RoundNotificationTransform : MonoBehaviour {

	void Start ()
    {
        Notification.Instance.OtherTransformSet(transform);
	}
	
}
