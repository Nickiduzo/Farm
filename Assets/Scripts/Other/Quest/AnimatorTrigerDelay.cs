using UnityEngine;
using AwesomeTools.Sound;

/// <summary>
/// Класс-аниматор для анимационных действий фермера
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimatorTrigerDelay : MonoBehaviour
{
    private const string BARKING = "Barking";

    [SerializeField] private SoundSystem _soundSystem;
    [SerializeField] private string _triger;
    [SerializeField] private float _delay;
    private Animator _animator;

    /// <summary>
    /// Включает анимацию, чтобы фермер махал покупателю, когда покупатель уходит
    /// </summary>
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        InvokeRepeating(nameof(ActiveTrigger), _delay, _delay);
    }
    /// <summary>
    /// Отправляет триггер по названию
    /// </summary>
    private void ActiveTrigger()
    {
        _animator.SetTrigger(_triger);
    }

    /// <summary>
    /// Відтворює звук [BARKING], викликається з "Animation Event"
    /// </summary>
    public void PlayBarkingSound()
    {
        if(_soundSystem != null)
            _soundSystem.PlaySound(BARKING);
    }
}
