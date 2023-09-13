using AwesomeTools.Sound;
using UnityEngine;
/// <summary>
/// Класс, что отвечает за звуки NPC
/// </summary>
public class NPCSound : MonoBehaviour
{
    //[SerializeField] private SoundSystem _soundSystem;
    [SerializeField] private float _delay = 1f;
    [SerializeField] private string[] _soundName;
    /// <summary>
    /// Повторяет проигрывать звуки через задержку
    /// </summary>
    private void Start()
    {
        InvokeRepeating(nameof(PlayRandomSound), _delay, _delay);
    }
    /// <summary>
    /// Воспроизведение рандомного звука
    /// </summary>
    private void PlayRandomSound()
    {
        int randomIndex = Random.Range(0, _soundName.Length);
        SoundSystemUser.Instance.PlaySound(_soundName[randomIndex]);
    }
}
