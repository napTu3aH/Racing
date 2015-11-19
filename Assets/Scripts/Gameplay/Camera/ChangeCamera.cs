using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChangeCamera : MonoBehaviour {

    public static ChangeCamera Instance;

    public List<GameObject> _Cameras;
    internal int _CameraNumber;

	void Awake ()
    {
        Instance = this;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Change();
        }	
	}

    public void Change()
    {
        if (_CameraNumber < _Cameras.Count - 1)
        {
            _CameraNumber++;
        }
        else
        {
            _CameraNumber = 0;
        }
        
        for (int i = 0; i < _Cameras.Count; i++)
        {
            if (i == _CameraNumber)
            {
                _Cameras[i].SetActive(true);
            }
            else
            {
                _Cameras[i].SetActive(false);
            }
        }
    }
}
