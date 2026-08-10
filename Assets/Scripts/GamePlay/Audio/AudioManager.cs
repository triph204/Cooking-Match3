using UnityEngine;
using UnityEngine.Audio;

namespace Match3.Gameplay.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;

        [Header("Mixer")]
        [SerializeField] private AudioMixer mainMixer;


        private const string SFX_KEY = "SFXVolume";
        private const string MUSIC_KEY = "MusicVolume";

        [Header("SFX")]
        public AudioSource gemMatchSound;
        public AudioSource comboDingSound;
        public AudioSource clickSound;
        public AudioSource winSound;
        public AudioSource loseSound;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            LoadVolume();
        }

        public void PlaySound(AudioSource audio)
        {
            if (audio == null)
            {
                Debug.LogWarning("[AudioManager] AudioSource chưa được gán trong Inspector!");
                return;
            }

            audio.Stop();
            audio.Play();
        }

        // dbVal: giá trị slider từ 0.0001 -> 1 (linear), sẽ convert sang dB
        public void SetSFXVolume(float sliderValue)
        {
            float dB = ConvertToDecibel(sliderValue);
            mainMixer.SetFloat(SFX_KEY, dB);
            PlayerPrefs.SetFloat(SFX_KEY, sliderValue);
        }

        public void SetMusicVolume(float sliderValue)
        {
            float dB = ConvertToDecibel(sliderValue);
            mainMixer.SetFloat(MUSIC_KEY, dB);
            PlayerPrefs.SetFloat(MUSIC_KEY, sliderValue);
        }

        private float ConvertToDecibel(float sliderValue)
        {
            // tránh log(0) = -infinity
            sliderValue = Mathf.Clamp(sliderValue, 0.0001f, 1f);
            return Mathf.Log10(sliderValue) * 20f;
        }

        private void LoadVolume()
        {
            float sfxValue = PlayerPrefs.GetFloat(SFX_KEY, 1f);
            float musicValue = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);

            SetSFXVolume(sfxValue);
            SetMusicVolume(musicValue);
        }

        // dùng để UI Slider load đúng giá trị đã lưu khi mở panel settings
        public float GetSFXVolume() => PlayerPrefs.GetFloat(SFX_KEY, 1f);
        public float GetMusicVolume() => PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
    }
}