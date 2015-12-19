using UnityEngine;
using System.Collections;

public class SlowMotionObject : MonoBehaviour {

    public static SlowMotionObject Instance;

    public SphereCollider _SphereCollider;
    [Range (0, 100)] public int _MinimumValueChance;
    public float _SpeedTimeSlowing = 1.0f;
    public bool _Slow { get { return _Slowing; } }

    bool _Slowing;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        Instance = this;
        _SphereCollider = GetComponent<SphereCollider>();
    }

    void SlowMotiong()
    {
        if (!_Slowing)
        {
            int _randomChance = Random.Range(0, 100);
            if (_randomChance <= _MinimumValueChance)
            {
                _Slowing = true;
                StartCoroutine(CountingSlow());
            }
        }
    }

    void OnTriggerEnter(Collider _col)
    {
        if (_col.CompareTag("HitBoxsParent"))
        {
            SlowMotiong();
        }
    }

    IEnumerator CountingSlow()
    {
        SlowMotionClass.Instance.SlowMotion(false);
        yield return new WaitForSeconds(_SpeedTimeSlowing);
        if(GameSettings.Instance._SettingsLayer.activeSelf) SlowMotionClass.Instance.SlowMotion(true);
        else SlowMotionClass.Instance.SlowMotion(false);
        _Slowing = false;
        yield return null;
    }
}
