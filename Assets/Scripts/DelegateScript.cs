using UnityEngine;
using System.Collections;

public class DelegateScript : MonoBehaviour {

    public delegate void Delegat(string s);
    public Delegat d;
    

	// Use this for initialization
	void Start () {
        d += mainFunc;
        d += Test;
    }


    void Update()
    {
        d("3");
    }

    void mainFunc(string s)
    {
        Debug.Log(s);
    }
    void Test(string s)
    {
        Debug.Log(s+s);
    }

}
