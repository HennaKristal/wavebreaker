using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance => _instance;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider MusicVolumeSlider;
    [SerializeField] private Slider AmbientVolumeSlider;
    [SerializeField] private Slider SFXVolumeSlider;
    [SerializeField] private Slider UIVolumeSlider;

    [SerializeField] private TextMeshProUGUI MusicVolumeSliderText;
    [SerializeField] private TextMeshProUGUI AmbientVolumeSliderText;
    [SerializeField] private TextMeshProUGUI SFXVolumeSliderText;
    [SerializeField] private TextMeshProUGUI UIVolumeSliderText;

    private bool isInitializing = true;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        LoadVolume("MusicVolume", MusicVolumeSlider, MusicVolumeSliderText);
        LoadVolume("AmbientVolume", AmbientVolumeSlider, AmbientVolumeSliderText);
        LoadVolume("SFXVolume", SFXVolumeSlider, SFXVolumeSliderText);
        LoadVolume("UIVolume", UIVolumeSlider, UIVolumeSliderText);

        isInitializing = false;
    }

    public void OnVolumeSliderChanged()
    {
        if (isInitializing)
        {
            return;
        }

        ApplyVolume("MusicVolume", MusicVolumeSlider, MusicVolumeSliderText);
        ApplyVolume("AmbientVolume", AmbientVolumeSlider, AmbientVolumeSliderText);
        ApplyVolume("SFXVolume", SFXVolumeSlider, SFXVolumeSliderText);
        ApplyVolume("UIVolume", UIVolumeSlider, UIVolumeSliderText);
    }

    private void LoadVolume(string parameter, Slider slider, TextMeshProUGUI label)
    {
        float value = PlayerPrefs.GetFloat(parameter, 0.8f);
        slider.value = value;
        label.text = Mathf.Ceil(value * 100f).ToString() + "%";
        audioMixer.SetFloat(parameter, LinearToDecibel(value));
    }

    private void ApplyVolume(string parameter, Slider slider, TextMeshProUGUI label)
    {
        float value = slider.value;
        label.text = Mathf.Ceil(value * 100f).ToString() + "%";
        PlayerPrefs.SetFloat(parameter, value);
        audioMixer.SetFloat(parameter, LinearToDecibel(value));
    }

    public void SaveAudioSettings()
    {
        PlayerPrefs.Save();
    }

    private float LinearToDecibel(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }
}
