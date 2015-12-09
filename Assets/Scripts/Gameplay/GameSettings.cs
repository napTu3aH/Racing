using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Класс настроек геймплея.
/// </summary>
public class GameSettings : MonoBehaviour {
    public static GameSettings Instance;

    public bool _ActiveScript;

    [Header("Main buttons")]
    public UIButton _MenuButton;
    public UIButton _RestartButton;

    [Header("UI layers")]
    public GameObject _GameplayUI;
    public GameObject _SettingsLayer;
    public GameObject _ControlsLayer;

    [Header("Sound")]
    public string[] _SoundSprite; // 0 - off, 1 - on
    public UISprite _SoundButton;
    public UISlider _SoundSlider;
    public AudioSource _SoundSource;
    public float _Sound;

    [Header("Music")]
    public string[] _MusicSprite; // 0 - off, 1 - on
    public UISprite _MusicButton;
    public UISlider _MusicSlider;
    public AudioSource _MusicSource;
    public float _Music;

    [Header("Other settings")]
    public bool _Vibrate;
    public bool _SlowMotion;
    public bool _Particles;
    public bool _Destructions;
    public bool _ShowBackgroundForLoading;
    [Range (0.0f, 1.0f)]public float _SlowValue;
    public float _SmoothRate = 0.005f;

    internal float _SlowingFactor = 1.0f;
    bool _Slowed, _SlowCoroutine;

    void Awake()
    {
        if(_ActiveScript) Init();
    }

    void Init()
    {
        Instance = this;

        if (!_SettingsLayer)
        {
            _SettingsLayer = GameObject.FindWithTag("Settings");
        }
        _SettingsLayer.SetActive(false);

        _Vibrate = Convert.ToBoolean(PlayerPrefs.GetInt("Vibration", 1));
        _Sound = PlayerPrefsHelper.GetFloat("Sound", 0.5f); _SoundSlider.value = _Sound;
        _Music = PlayerPrefsHelper.GetFloat("Music", 0.5f); _MusicSlider.value = _Music;
        _Particles = Convert.ToBoolean(PlayerPrefsHelper.GetInt("Particles", 1));
        _Destructions = Convert.ToBoolean(PlayerPrefsHelper.GetInt("Destructions", 1));
        SetAudioSourceValue(_SoundSprite, _SoundButton, _SoundSource, _SoundSlider);
        SetAudioSourceValue(_MusicSprite, _MusicButton, _MusicSource, _MusicSlider);

        ReturnerTimeScale.Instance.ReturnTimeScale();       
    }


    /// <summary>
    /// Метод установки значения звуков.
    /// </summary>
    public void SetSoundValue()
    {
        SetAudioSourceValue(_SoundSprite, _SoundButton, _SoundSource, _SoundSlider);
    }

    /// <summary>
    /// Метод сохранения значений звука (для слайдера).
    /// </summary>
    public void SaveSoundValue()
    {
        PlayerPrefsHelper.SetFloat("Sound", _SoundSource.volume);
        PlayerPrefsHelper.SetInt("Sound bool", 1);
    }

    /// <summary>
    /// Метод сохранения значений звука (для кнопки).
    /// </summary>
    public void SoundButton()
    {
        SetAudioSourceValueButton(_SoundSlider, _SoundSource, "Sound", "Sound bool");
    }

    /// <summary>
    /// Метод установки значения музыки.
    /// </summary>
    public void SetMusicValue()
    {
        SetAudioSourceValue(_MusicSprite, _MusicButton, _MusicSource, _MusicSlider);
    }

    /// <summary>
    /// Метод сохранения значений музыки (для слайдера).
    /// </summary>
    public void SaveMusicValue()
    {
        PlayerPrefsHelper.SetFloat("Music", _MusicSource.volume);
        PlayerPrefsHelper.SetInt("Music bool", 1);
    }

    /// <summary>
    /// Метод сохранения значений музыки (для кнопки).
    /// </summary>
    public void MusicButton()
    {
        SetAudioSourceValueButton(_MusicSlider, _MusicSource, "Music", "Music bool");
    }

    /// <summary>
    /// Метод обработки AudioSource'а (кнопкой).
    /// </summary>
    /// <param name="_slider">Слайдер</param>
    /// <param name="_source">AudioSource</param>
    /// <param name="_nameChangerFloat">Название типа AudioSource'а для ползунка громкости</param>
    /// <param name="_nameChangerBool">Название типа AudioSource'а для bool значения</param>
    void SetAudioSourceValueButton(UISlider _slider, AudioSource _source, string _nameChangerFloat, string _nameChangerBool)
    {
        if (_slider.value > 0.0f)
        {
            _slider.value = 0.0f;
            PlayerPrefsHelper.SetInt(_nameChangerBool, 0);
        }
        else
            if (_slider.value == 0.0f && PlayerPrefsHelper.GetFloat(_nameChangerFloat, 0.5f) != 0.0f)
        {
            _slider.value = PlayerPrefsHelper.GetFloat(_nameChangerFloat, 0.5f);
            PlayerPrefsHelper.SetInt(_nameChangerBool, 1);
        }
        else
        {
            _slider.value = 1.0f;
            PlayerPrefsHelper.SetFloat(_nameChangerFloat, _source.volume);
            PlayerPrefsHelper.SetInt(_nameChangerBool, 1);
        }
        _source.volume = _slider.value;
    }

    /// <summary>
    /// Метод обработки AudioSource'а (слайдером).
    /// </summary>
    /// <param name="_sprites">Спрайт</param>
    /// <param name="_btn">Кнопка</param>
    /// <param name="_source">AudioSource</param>
    /// <param name="_slider">Слайдер</param>
    void SetAudioSourceValue(string[] _sprites, UISprite _btn, AudioSource _source, UISlider _slider)
    {
        _source.volume = _slider.value;
        if (_source.volume > 0.0f && _btn.spriteName != _sprites[1])
        {
            _btn.spriteName = _sprites[1];
            _btn.MarkAsChanged();
        }
        else
            if (_source.volume == 0.0f)
        {
            _btn.spriteName = _sprites[0];
            _btn.MarkAsChanged();
        }
    }

    /// <summary>
    /// Метод установки значения вибрации (ВКЛ/ВЫКЛ).
    /// </summary>
    public void SetVibration()
    {
        _Vibrate = !_Vibrate;
        PlayerPrefsHelper.SetInt("Vibration", Convert.ToInt32(_Vibrate));
        Vibrate();
        if (_Vibrate)
        {
            Notification.Instance.Notificate("Vibrate On");
        }
        else
        {
            Notification.Instance.Notificate("Vibrate Off");
        }
        

    }

    /// <summary>
    /// Метод вызова вибрации.
    /// </summary>
    public void Vibrate()
    {
        if (_Vibrate)
        {
            Handheld.Vibrate();
        }
    }

    /// <summary>
    /// Вкл/Выкл частицы.
    /// </summary>
    public void Particles()
    {
        _Particles = !_Particles;
        PlayerPrefsHelper.SetInt("Particles", Convert.ToInt32(_Particles));

        if (_Particles)
        {
            Notification.Instance.Notificate("Particles On");
        }
        else
        {
            Notification.Instance.Notificate("Particles Off");
        }
    }

    /// <summary>
    /// Показать/Скрыть настройки.
    /// </summary>
    public void ShowHideSettings()
    {
        _SettingsLayer.SetActive(!_SettingsLayer.activeSelf);
        _ControlsLayer.SetActive(!_ControlsLayer.activeSelf);
        if (_SlowMotion)
        {
            SlowMotion(SlowMotionClass.Instance._Slow);
        }
        else
        {
            if (Time.timeScale == 1.0f)
            {
                Time.timeScale = 0.0f;
            }
            else
            {
                Time.timeScale = 1.0f;
            }
        }
    }

    /// <summary>
    /// Метод загрузки меню.
    /// </summary>
    public void MenuButton()
    {
        StartCoroutine(LoadingLevelCoroutine(1));
    }

    /// <summary>
    /// Метод перезагрузки уровня.
    /// </summary>
    public void RestartButton()
    {
        StartCoroutine(LoadingLevelCoroutine(Application.loadedLevel));        
    }

    /// <summary>
    /// Метод замедления времени.
    /// </summary>
    public void SlowMotion(bool _Slow)
    {
        if (!_Slow)
        {
            _Slowed = !_Slowed;
            if (!_SlowCoroutine)
            {
                _SlowCoroutine = true;
                StartCoroutine(Slowing());
            }
        }        
    }

    /// <summary>
    /// Метод изменения значения кадра.
    /// </summary>
    void FrameChangeValue()
    {
        Time.timeScale = _SlowingFactor;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        _SoundSource.pitch = _SlowingFactor;
        _MusicSource.pitch = Mathf.Clamp(_SlowingFactor + 0.4f, -1.0f, 1.0f);
        Debugger.Instance.Log(Time.fixedDeltaTime);
    }

    /// <summary>
    /// Сопрограмма плавного замедления времени.
    /// </summary>
    IEnumerator Slowing()
    {

        while (_Slowed && _SlowingFactor != _SlowValue)
        {
            if (_SlowingFactor > _SlowValue)
            {
                _SlowingFactor -= _SmoothRate;
            }
            else
            if (_SlowingFactor < _SlowValue)
            {
                _SlowingFactor = _SlowValue;
            }
            FrameChangeValue();
            yield return null;
        }

        while (!_Slowed && _SlowingFactor != 1.0f)
        {
            if (_SlowingFactor < 1.0f)
            {
                _SlowingFactor += _SmoothRate;
            }
            else
            if (_SlowingFactor > 1.0f)
            {
                _SlowingFactor = 1.0f;
            }
            FrameChangeValue();
            yield return null;
        }
        
        _SlowCoroutine = false;
        yield return null;
    }

    IEnumerator LoadingLevelCoroutine(int _index)
    {        
        _MenuButton.isEnabled = false;
        _RestartButton.isEnabled = false;     
        yield return new WaitForSeconds(0);
        _GameplayUI.SetActive(false);
        LoadingLevel.Instance._Indicator.Init(_index, _ShowBackgroundForLoading);
    }

}
