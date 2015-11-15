using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadLevel : MonoBehaviour {

    public Button _BtnPlay;


    void Awake()
    {
        _BtnPlay = GetComponent<Button>();
    }

    IEnumerator Load()
    {
#if UNITY_IPHONE
            Handheld.SetActivityIndicatorStyle(iOS.ActivityIndicatorStyle.Gray);
#elif UNITY_ANDROID
        Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Large);
#endif

        Handheld.StartActivityIndicator();
        yield return new WaitForSeconds(0);
        Application.LoadLevel(1);
    }

    public void StartLevel()
    {
        _BtnPlay.interactable = false;
        StartCoroutine(Load());
    }
}
