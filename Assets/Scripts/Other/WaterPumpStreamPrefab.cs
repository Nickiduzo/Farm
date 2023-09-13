using AwesomeTools.Sound;
using UnityEngine;
/// <summary>
/// Класс, что отвечает за водяной насос
/// </summary>
public class WaterPumpStreamPrefab : MonoBehaviour
{
    private const string WATER_SOUND_NAME = "Water";

    [SerializeField] private BaseHoleTriggerObserver _observer;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private ISoundSystem _soundSystem;
    private bool _isWorking;
    /// <summary>
    /// Конструктор получения звуковой системы
    /// </summary>
    /// <param name="soundSystem">звуковая система</param>
    public void Construct(SoundSystem soundSystem)
    {
        _soundSystem = soundSystem;
    }
    /// <summary>
    /// Подписка на событие
    /// </summary>
    private void Awake()
    {
        _observer.OnTriggerStay += PureHole;
    }
    /// <summary>
    /// Отписка от события
    /// </summary>
    private void OnDestroy()
    {
        _observer.OnTriggerStay -= PureHole;
    }
    /// <summary>
    /// Добавление прогресса для класса ямы
    /// </summary>
    /// <param name="hole">базовый класс ямы</param>
    private void PureHole(BaseHole hole)
    => hole.AddProgress(Time.deltaTime * 5);
    /// <summary>
    /// Метод на получение значения
    /// </summary>
    /// <returns>возвращает булевое значение нынешней работы насоса</returns>
    public bool IsWorking()
    {
        return _isWorking;
    }
    /// <summary>
    /// Включает все компоненты для работы насоса
    /// </summary>
    public void EnableWater()
    {
        _isWorking = true;
        _collider.enabled = true;
        _spriteRenderer.enabled = true;
        _soundSystem.PlaySound(WATER_SOUND_NAME);
    }
    /// <summary>
    /// Включить воду для томатов
    /// </summary>
    public void EnableWaterInTomato()
    {
        _spriteRenderer.enabled = true;
        _soundSystem.PlaySound(WATER_SOUND_NAME);
    }
    /// <summary>
    /// Отключить воду для томатов
    /// </summary>
    public void DisableWaterInTomato()
    {
        _spriteRenderer.enabled = false;
        _soundSystem.StopSound(WATER_SOUND_NAME);
    }
    /// <summary>
    /// Отключает все компоненты для работы насоса
    /// </summary>
    public void DisableWater()
    {
        _isWorking = false;
        _collider.enabled = false;
        _spriteRenderer.enabled = false;
        _soundSystem.StopSound(WATER_SOUND_NAME);
    }
    /// <summary>
    /// Контролирует активность коллайдера
    /// </summary>
    /// <param name="enabled">параметр для активности</param>
    public void EnableCollider(bool enabled)
    {
        _collider.enabled = enabled;
    }
}
