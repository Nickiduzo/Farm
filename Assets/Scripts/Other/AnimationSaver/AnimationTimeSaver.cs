using UnityEngine;
/// <summary>
/// Класс контроля времени анимации
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimationTimeSaver : MonoBehaviour
{
    private Animator _animator;
    [SerializeField] private string _prefAnimationName;

    /// <summary>
    /// Получение аниматора у объекта
    /// </summary>
    private void Awake() => _animator = GetComponent<Animator>();

    /// <summary>
    /// Загрузка данных при появлении объекта
    /// </summary>
    private void OnEnable() => LoadData();

    /// <summary>
    /// Сохраниение стадии анимации при деактивации объекта
    /// </summary>
    private void OnDisable() => SaveAnimationState();

    /// <summary>
    /// Загрузка анимационных данных из PlayerPrefs
    /// </summary>
    private void LoadData()
    {
        float animationTime = PlayerPrefs.GetFloat(_prefAnimationName, 0);
        _animator.Play(0, 0, animationTime);
    }

    /// <summary>
    /// Сохраняет текушую стадию анимации в PlayerPrefs
    /// </summary>
    private void SaveAnimationState()
    {
        float currentTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        PlayerPrefs.SetFloat(_prefAnimationName, currentTime);
        PlayerPrefs.Save();
    }

}