using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Notification : MonoBehaviour
{
    [SerializeField] internal GameObject _LabelPrefab;
    [SerializeField] internal Transform _StaticLabelTransform, _StaticTransformFrom, _StaticTransformTo;
    [SerializeField] internal Transform _Parent, _TransformFrom, _TransformTo;

    internal int _Index;
    float _Time, _TimeStay;
    bool _Showed;
    GameObject _LabelGameObject;
    internal UILabel _Label, _StaticLabel;
    internal List<TweeningText> _TweeningText;
    internal TweenTransform _StaticTweenTransform;
    private static Notification _Notification;
    public static Notification Instance
    {
        get
        {
            if (_Notification != null)
            {
                return _Notification;
            }
            else
            {
                GameObject _thisPrefab = Instantiate(Resources.Load("Interface/NotificationUI", typeof(GameObject))) as GameObject;
                _Notification = _thisPrefab.GetComponentInChildren<Notification>();
                return _Notification;
            }
        }
    }

    void Init()
    {
        _Notification = this;
        _TweeningText = new List<TweeningText>();
        _StaticTweenTransform = _StaticLabelTransform.GetComponent<TweenTransform>();
        _StaticLabel = _StaticLabelTransform.GetComponentInChildren<UILabel>();
    }

    void Awake()
    {
        Init();
    }

    internal void NotificateStatic(string _text)
    {
        SendTextStatic(_text, 3.0f);
    }

    internal void NotificateStatic(string _text, float _timestay)
    {
        SendTextStatic(_text, _timestay);
    }

    internal void Notificate(string _text)
    {
        SendText(_text, 3.0f);
    }

    internal void Notificate(string _text, float _timestay)
    {
        SendText(_text, _timestay);
    }

    void SendTextStatic(string _text, float _timestay)
    {
        _StaticLabel.text = _text;
        _Time = 0.0f;
        _TimeStay = _timestay;
        if (!_Showed)
        {
            _Showed = true;
            _StaticTweenTransform.PlayForward();
            StartCoroutine(StaticNotify());
        }         
    }

    void SendText(string _text, float _timestay)
    {
        CreateLabel();
        _Label.text = _text;
        _TweeningText[_Index].StartTweening(_timestay);
        if (_Index > 0)
        {
            _TweeningText[_Index - 1].ForceReverse();
        }
        _Index++;
    }

    internal void TestMessage()
    {
        Notificate("Test");
    }

    internal void TestStaticMessage()
    {
        NotificateStatic("Test");
    }

    void CreateLabel()
    {
        _LabelGameObject = Instantiate(_LabelPrefab, _TransformFrom.position, _TransformFrom.rotation) as GameObject;
        _LabelGameObject.transform.SetParent(_Parent);
        _Label = _LabelGameObject.GetComponent<UILabel>();
        TweeningText _TwT = _LabelGameObject.GetComponent<TweeningText>();
        _TweeningText.Add(_TwT);
        _TweeningText[_Index]._TweenTransform.from = _TransformFrom;
        _TweeningText[_Index]._TweenTransform.to = _TransformTo;        
    }

    IEnumerator StaticNotify()
    {
        while (_Time < _TimeStay)
        {
            _Time += RealTime.deltaTime;
            yield return null;
        }
        _StaticTweenTransform.PlayReverse();
        _Showed = false;
        yield return null;
    }
}
