using System;
using UnityEngine;
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
    public int _MainMenuLevel;
    public int _ThisSceneLevel;
    public bool _Vibrate;
    public bool _SlowMotion;
    public bool _Particles;
    public bool _Destructions;
    public bool _ShowBackgroundForLoading;


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
            SlowMotionClass.Instance.SlowMotion(SlowMotionObject.Instance._Slow);
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
        StartCoroutine(LoadingLevelCoroutine(_MainMenuLevel));
    }

    /// <summary>
    /// Метод перезагрузки уровня.
    /// </summary>
    public void RestartButton()
    {
        StartCoroutine(LoadingLevelCoroutine(_ThisSceneLevel));        
    }

    IEnumerator LoadingLevelCoroutine(int _index)
    {        
        _MenuButton.isEnabled = false;
        _RestartButton.isEnabled = false;     
        _GameplayUI.SetActive(false);
        LoadingLevel.Instance._Indicator.Init(_index, _ShowBackgroundForLoading);
        yield return null;
    }

}
