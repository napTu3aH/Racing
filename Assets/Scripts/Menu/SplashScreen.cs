using UnityEngine;
using System.Collections;

public class SplashScreen : MonoBehaviour
{
    void Start()
    {
        GetComponent<ImageLoading>().Init();
    }
}
