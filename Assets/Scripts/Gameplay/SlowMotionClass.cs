using UnityEngine;
using System.Collections;

public class SlowMotionClass : MonoBehaviour {

    public static SlowMotionClass Instance;

    public SphereCollider _SphereCollider;
    [Range (0, 100)] public int _MinimumValueChance;
    public float _SpeedTimeSlowing = 1.0f;
    public bool _Slow { get { return _Slowing; } }

    float _TimeSlow;
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
        GameSettings.Instance.SlowMotion(false);
        while (_Slowing)
        {
            _TimeSlow += Time.deltaTime * _SpeedTimeSlowing;
            if (_TimeSlow > 1.0f)
            {
                if(GameSettings.Instance._SettingsLayer.activeSelf)
                    GameSettings.Instance.SlowMotion(true);
                else GameSettings.Instance.SlowMotion(false);
                _TimeSlow = 0.0f;
                _Slowing = false;
            }
            yield return null;
        }
        yield return null;
    }
}
