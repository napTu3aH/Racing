using UnityEngine;
using System.Collections;
public class AutoDestruct : MonoBehaviour
{
	public bool OnlyDeactivate;
    public float _TimeDestroy = 0.5f;
	
	void OnEnable()
	{
        if (!OnlyDeactivate)
        {
            Destroy(this.gameObject, _TimeDestroy);
        }
        else
        {
            gameObject.SetActive(false);
        }
        
	}

}
