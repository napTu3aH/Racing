using System;
using System.Threading;
using UnityEngine;
using System.Collections;

public class EventSystemScript : MonoBehaviour {


    public static EventSystemScript Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new GameObject("EventSystem").AddComponent<EventSystemScript>();
            }
            return _Instance;
        }
    }

    static EventSystemScript _Instance;

    public Action<string> Moving;

    void Start()
    {
        Moving += Message;
    }

    void Message(string _mes)
    {
        Debug.Log(_mes);
    }

    public void Move(int _distance)
    {
        for (int i = 0; i < _distance; i++)
        {            
            Moving(string.Format("Moving... {0}", i));
        }
    }
}
