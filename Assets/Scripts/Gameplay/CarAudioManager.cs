using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class CarAudioManager : MonoBehaviour {

    public AudioClip _HighAccelClip;
    public float _PitchMultiplier = 1f;
    public float _LowPitchMin = 1f;
    public float _LowPitchMax = 6f;
    public float _HighPitchMultiplier = 0.25f;
    public float _MaxRolloffDistance = 500;
    public float _DopplerLevel = 1;
    public bool _UseDoppler = true;

    bool _StartedSound, _Player;
    AudioSource _AudioSource;
    CarController _CarController;


    void Awake()
    {
        Init();
    }

    void Init()
    {
        _AudioSource = GetComponent<AudioSource>();
        _AudioSource.maxDistance = _MaxRolloffDistance;

        if (!_AudioSource.loop)
        {
            _AudioSource.loop = true;
        }
        _AudioSource.enabled = false;

        _CarController = GetComponent<CarController>();  
              
        if (transform.CompareTag("Player"))
        {
            _Player = true;
            StartSound();
        }
        if (!_Player)
        {
            StartCoroutine(DistanceToCamera());
        }        
    }

    void StartSound()
    {
        _AudioSource.enabled = true;
        _AudioSource.clip = _HighAccelClip;
        _AudioSource.Play();
        _StartedSound = true;
        StartCoroutine(_SoundEngine());
    }

    void StopSound()
    {
        _AudioSource.Stop();
        _AudioSource.enabled = false;
        _StartedSound = false;
    }

    public void Destroying()
    {
        StopAllCoroutines();
        Destroy(_AudioSource);
        Destroy(this);
    }

    IEnumerator _SoundEngine()
    {
        while (_StartedSound)
        {
            float _pitch = ULerp(_LowPitchMin * GameSettings.Instance._SlowingFactor, _LowPitchMax * GameSettings.Instance._SlowingFactor, _CarController.Revs);
            _pitch = Mathf.Min(_LowPitchMax, _pitch);

            _AudioSource.pitch = _pitch * _PitchMultiplier * _HighPitchMultiplier;
            _AudioSource.dopplerLevel = _UseDoppler ? _DopplerLevel : 0;
            _AudioSource.volume = GameSettings.Instance._SoundSlider.value;    
                    
            yield return null;
        }
        yield return null;
    }

    IEnumerator DistanceToCamera()
    {
        while (true)
        {
            float _camDist = Vector3.Distance(Camera.main.transform.position, transform.position);
            if (!_StartedSound && _camDist < _MaxRolloffDistance)
            {
                StartSound();
            }

            if (_StartedSound && _camDist > _MaxRolloffDistance)
            {
                StopSound();
            }
            yield return null;
        }
    }

    static float ULerp(float from, float to, float value)
    {
        return (1.0f - value) * from + value * to;
    }
}
