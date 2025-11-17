using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance => _instance;
    [SerializeField] private AudioMixer audioMixer;


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

    public void LoadVolume(string parameter, Slider slider, TextMeshProUGUI label)
    {
        float value = PlayerPrefs.GetFloat(parameter, 0.8f);
        slider.value = value;
        label.text = Mathf.Ceil(value * 100f).ToString() + "%";
        audioMixer.SetFloat(parameter, LinearToDecibel(value));
    }

    public void ApplyVolume(string parameter, Slider slider, TextMeshProUGUI label)
    {
        float value = slider.value;
        label.text = Mathf.Ceil(value * 100f).ToString() + "%";
        PlayerPrefs.SetFloat(parameter, value);
        audioMixer.SetFloat(parameter, LinearToDecibel(value));
    }

    private float LinearToDecibel(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }
}
