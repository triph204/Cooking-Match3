using Match3.Gameplay.Audio;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    private void Start()
    {
        if (AudioManager.Instance == null) return;

        // load giá trị đã lưu lên slider, không gọi callback khi set giá trị ban đầu
        sfxSlider.SetValueWithoutNotify(AudioManager.Instance.GetSFXVolume());
        musicSlider.SetValueWithoutNotify(AudioManager.Instance.GetMusicVolume());

        sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        musicSlider.onValueChanged.AddListener(OnMusicChanged);
    }

    private void OnSFXChanged(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    private void OnMusicChanged(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnDestroy()
    {
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
        if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
    }
}