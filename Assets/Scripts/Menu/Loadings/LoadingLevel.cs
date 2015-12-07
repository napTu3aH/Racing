using UnityEngine;
using System.Collections;

public class LoadingLevel : MonoBehaviour
{
    [SerializeField] internal LoadingLevelLogics _LoadingLevelLogics;    
    [SerializeField] internal ImageLoading _ImageLoading;
    [SerializeField] internal Indicator _Indicator;
    [SerializeField] internal bool _DontDestroy = true;
   
    private static LoadingLevel _LoadingLevel;
    public static LoadingLevel Instance
    {
        get
        {
            if (_LoadingLevel != null)
            {
                return _LoadingLevel;
            }
            else
            {
                GameObject _thisPrefab = Instantiate(Resources.Load("Interface/Loading", typeof(GameObject))) as GameObject;
                _LoadingLevel = _thisPrefab.GetComponent<LoadingLevel>();
                return _LoadingLevel;
            }
        }
    }

    void Awake()
    {
        Init();
    }

    void Init()
    {
        _LoadingLevel = this;
        _LoadingLevelLogics = GetComponentInChildren<LoadingLevelLogics>();
        _ImageLoading = GetComponent<ImageLoading>();
        _Indicator = GetComponentInChildren<Indicator>();
        if (_DontDestroy) DontDestroyOnLoad(gameObject);
    }
}
