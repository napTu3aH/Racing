using UnityEngine;
using System.Collections;

public class TweeningText : MonoBehaviour {

    [SerializeField] internal TweenTransform _TweenTransform;
    internal float _TimeStay;
    float _Time = 0.0f;

    internal void StartTweening(float _timeStay)
    {
        _TimeStay = _timeStay;
        StartCoroutine(_Tweening());
    }

    internal void ForceReverse()
    {
        _Time = _TimeStay;
    }

    IEnumerator _Tweening()
    {        
        _TweenTransform.PlayForward();
        while (_Time <= _TimeStay)
        {
            _Time += RealTime.deltaTime;
            yield return null;
        }
        _TweenTransform.PlayReverse();
        Notification.Instance._TweeningText.Remove(this);
        Destroy(gameObject, _TweenTransform.duration);
        Notification.Instance._Index--;
    }

}
