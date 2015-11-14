using UnityEngine;
using System.Collections;

public class DelegateScript : MonoBehaviour {

    public delegate void Delegat(string s);
    public Delegat d;
    

	// Use this for initialization
	void Start () {
        d = mainFunc;  
    }


    void Update()
    {
        d("3");
    }

    void mainFunc(string s)
    {
        Debug.Log(s);
    }

}
