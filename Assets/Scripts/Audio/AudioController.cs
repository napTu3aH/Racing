using UnityEngine;

public class AudioController : MonoBehaviour {

    public static AudioController Instance;

    [Header("Music")]
    public AudioSource _Music;
    public AudioClip[] _Clips;
    public bool _RandomMusic;
    public bool _AwakeMusic;
    public bool _LoopMusic;

    [Header("Sound")]
    public AudioSource _Sound;
    public bool _AwakeSound;
    public bool _LoopSound;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        Instance = this;
        _Music = transform.SearchChildWithName("Music").GetComponent<AudioSource>();
        _Sound = transform.SearchChildWithName("Sound").GetComponent<AudioSource>();
        PlayMusic();
    }

    void PlayMusic()
    {
        if (_RandomMusic && _Clips.Length > 0)
        {
            int _index = Random.Range(0, _Clips.Length);
            _Music.clip = _Clips[_index];
        }
        if (_Music.clip != null)
        {
            _Music.Play();
        }      
    }

    public void PlayOneShot(AudioClip _clip)
    {
        if (_clip)
        {
            _Sound.PlayOneShot(_clip);
        }        
    }

    public void PlayOneShot(AudioClip _clip, float _volume)
    {
        if (_clip && _volume > 0.0f)
        {
            _Sound.PlayOneShot(_clip, _volume);
        }
    }

    public static void PlayOneShot(AudioSource _source, AudioClip _clip)
    {
        if (_clip && _source)
        {
            _source.PlayOneShot(_clip);
        }
    }

    public static void PlayOneShot(AudioSource _source, AudioClip _clip, float _volume)
    {
        if (_clip && _source && _volume > 0.0f)
        {
            _source.PlayOneShot(_clip, _volume);
        }
    }

    public static void PlayClip(AudioSource _source, AudioClip _clip)
    {
        if (_clip && _source)
        {
            _source.clip = _clip;
            _source.Play();
        }
    }

    public static void PlayOnAwake(AudioSource _source, bool _loop)
    {
        if(_source)
        _source.playOnAwake = _loop;
    }

    public static void Looping(AudioSource _source, bool _loop)
    {
        if(_source)
        _source.loop = _loop;
    }

}
