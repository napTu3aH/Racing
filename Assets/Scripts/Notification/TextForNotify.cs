using System;
using UnityEngine;
using System.Collections;

public class TextForNotify : MonoBehaviour
{
    [SerializeField] internal TextClassification[] _Classifications;
    [SerializeField] [Range(1, 100)] internal int _ChanceForNotify = 15;
    private static TextForNotify _TextForNotify;
    public static TextForNotify Instance
    {
        get
        {
            if (_TextForNotify != null)
            {
                return _TextForNotify;
            }
            else
            {
                GameObject _prefab = Instantiate(Resources.Load("Interface/NotificationText", typeof(GameObject))) as GameObject;
                _TextForNotify = _prefab.GetComponent<TextForNotify>();
                return _TextForNotify;
            }
        }
    }

    void Awake()
    {
        Init();
    }

    void Init()
    {
        _TextForNotify = this;
    }

    internal void PushText(int _indexClassification)
    {
        int _RandText = UnityEngine.Random.Range(0, _Classifications[_indexClassification]._Texts.Length);
        PushLogic(_indexClassification, _RandText);
    }

    internal void PushText(int _indexClassification, int _textIndex)
    {
        PushLogic(_indexClassification, _textIndex);
    }

    void PushLogic(int _indexClassification, int _textIndex)
    {
        //int _Chance = UnityEngine.Random.Range(1, 100);
        if (_Classifications[_indexClassification] != null)
        {
            if (_Classifications[_indexClassification]._Texts[_textIndex] != null)
            {
                string _text = _Classifications[_indexClassification]._Texts[_textIndex];
                Notification.Instance.NotificateStatic(_text);
            }
        }
    }

}

[Serializable]
public class TextClassification
{
    [SerializeField] internal string _NameClassification;
    [SerializeField] internal string[] _Texts;
}
