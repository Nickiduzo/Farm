using AwesomeTools.Sound;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SoundSystem))]
public class SoundSystemToggle : MonoBehaviour
{
    [SerializeField] private Toggle _musicToggle;
    [SerializeField] private Toggle _soundToggle;
    private SoundSystem _soundSystem;

    private const string MUSIC_KEY = "isMusicOn";
    private const string SOUND_KEY = "isSoundOn";


    /// <summary>
    /// Присв'язує до повзунків звуку та музики ф-ції "OnSoundToggleValueChanged", "OnMusicToggleValueChanged"
    /// </summary>
    void Start()
    {
        _soundSystem = GetComponent<SoundSystem>();
        _musicToggle.isOn = PlayerPrefs.GetInt(MUSIC_KEY, 1) == 1;
        _soundToggle.isOn = PlayerPrefs.GetInt(SOUND_KEY, 1) == 1;

        _musicToggle.onValueChanged.AddListener(OnMusicToggleValueChanged);
        _soundToggle.onValueChanged.AddListener(OnSoundToggleValueChanged);

    }

    /// <summary>
    /// вводимо булеве значення [value]- змінює значення ключа "MUSIC_KEY"
    /// </summary>
    private void OnMusicToggleValueChanged(bool value)
    {
        PlayerPrefs.SetInt(MUSIC_KEY, value ? 1 : 0);
        PlayerPrefs.Save();
        ResetMusic();
    }

    /// <summary>
    /// вводимо булеве значення [value]- змінює значення ключа "SOUND_KEY"
    /// </summary>
    private void OnSoundToggleValueChanged(bool value)
    {
        PlayerPrefs.SetInt(SOUND_KEY, value ? 1 : 0);
        PlayerPrefs.Save();
        ResetMusic();
    }

    /// <summary>
    /// Викликає ф-цію стстеми звуку "InitSetting"
    /// </summary>
    private void ResetMusic()
    {
        _soundSystem.InitSetting();
    }

}
