using UnityEngine;
using System.Collections;

public class ChangeCamera : MonoBehaviour {

    public static ChangeCamera Instance;

    public GameObject[] _Cameras;
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
        if (_CameraNumber < _Cameras.Length - 1)
        {
            _CameraNumber++;
        }
        else
        {
            _CameraNumber = 0;
        }
        
        for (int i = 0; i < _Cameras.Length; i++)
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
