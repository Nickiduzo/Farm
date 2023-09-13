using System;
using Apple;
using AwesomeTools.Sound;
using SunflowerScene;
using Tomato;
using UnityEngine;
using UsefulComponents;
using AwesomeTools;

/// <summary>
/// Массивный класс обработки событий и инициализации уровней Sunflower, Apple, Carrot
/// </summary>
public class Sprinkler : MonoBehaviour
{
    [SerializeField] private bool DontDestroyOnLoad;

    private Action OnTriggerEnterSub;
    private Action OnTriggerStaySub;
    private Action OnTriggerEndSub;
    private Action OnTriggerEnterUnsub;
    private Action OnTriggerStayUnsub;
    private Action OnTriggerEndUnsub;

    private FlowView _flowView;
    private Action StoppedFlow;
    private Action StartedFlow;
    private DragAndDrop _dragAndDrop;
    private WaterPumpTriggerObserver _observerSunflower;
    
    private TomatoSeedsTriggerObserver _observerTomato;
    private SpriteRenderer _spriteTomatoWaterStream;
    private Collider2D _colliderTomato;
    private const string boolNameTomatoAnimation = "Triggering";
    private Animator _animator;
    [SerializeField] private float valueGrowingTomato;

    private Action StartDragCarrot;
    private BaseHoleTriggerObserver _baseHoleTriggerObserverCarrot;
    private Collider2D _colliderCarrotStream;
    private SpriteRenderer _spriteRendererCarrotStream;
    
    private AppleSeedlingTriggerObserver _appleSeedlingTriggerObserverApple;
    private BaseHoleTriggerObserver _observerApple;
    private Collider2D _colliderAppleStream;
    private SpriteRenderer _spriteRendererAppleStream;
    private SpriteRenderer _spriteRendererAppleStreamPipe;
    
    #region Singleton

    public static Sprinkler Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance == this)
        {
            Destroy(gameObject);
        }

        if (DontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    #endregion Singleton
    /// <summary>
    /// При разрушении объекта вызывается отключение событий в методах и вызывает назначенные события
    /// </summary>
    private void OnDestroy()
    {
        OnTriggerEnterUnsub?.Invoke();
        OnTriggerStayUnsub?.Invoke();
        OnTriggerEndUnsub?.Invoke();
        ClearAllUnsub();
        OnDragAndDropDisable();
        OnCarrotPumpStreamDisable();
        OnApplePumpStreamDisable();
    }

    #region Sunflower realization
    /// <summary>
    /// Инициализация подсолнуха
    /// </summary>
    /// <param name="_observer">наблюдатель для подсолнуха</param>
    /// <param name="_dragAndDrop">класс для хватания и перемещения</param>
    /// <param name="_flowView">видимый поток при помощи спрайтов</param>
    /// <param name="StartedFlow">событие на начало потока</param>
    /// <param name="StoppedFlow">событие на окончание потока</param>
    public void InitSunflower(WaterPumpTriggerObserver _observer, DragAndDrop _dragAndDrop, FlowView _flowView, Action StartedFlow, Action StoppedFlow)
    {
        this._flowView = _flowView;
        this.StartedFlow = StartedFlow;
        this.StoppedFlow = StoppedFlow;
        this._dragAndDrop = _dragAndDrop;
        _observerSunflower = _observer;

        SunflowerSubActions();
        SetActionsToDragAndDrop(ShowFlow, onDragEnd:StopFlow);
    }
    /// <summary>
    /// добавление метода в событие наблюдателя на метод OnTriggerStay
    /// </summary>
    private void SunflowerSubActions()
    {
        _observerSunflower.OnTriggerStay += PrecessWatering;
    }
    /// <summary>
    /// процесс поливавания растения
    /// </summary>
    /// <param name="plant">растение, что поливается</param>
    private void PrecessWatering(PlantGrowing plant)
    {
        if (plant.Growing)
            plant.ProcessWatering();
    }
    /// <summary>
    /// Показывает поток визуально и вызывает событие старта потока
    /// </summary>
    private void ShowFlow()
    {
        _flowView.ShowFlow();
        StartedFlow?.Invoke();
    }
    /// <summary>
    /// Останавливает поток и вызывает событие остановки потока
    /// </summary>
    private void StopFlow()
    {
        _flowView.HideFlow();
        StoppedFlow?.Invoke();
    }

    #endregion Sunflower realization

    #region Carrot realization
    /// <summary>
    /// Инициализация моркови с присваиванием класса DragAndDrop и включения метода с событиями
    /// </summary>
    /// <param name="_dragAndDrop">класс для хватания и перемещения</param>
    public void InitCarrot(DragAndDrop _dragAndDrop)
    {
       
        this._dragAndDrop = _dragAndDrop;

        CarrotSubAction();
    }
    /// <summary>
    /// Инициализация моркови с текстурой, коллайдером и ямой
    /// </summary>
    /// <param name="_baseHoleTriggerObserver">базовый триггер ямы</param>
    /// <param name="_collider">коллайдер для моркови</param>
    /// <param name="_sprite">спрайт для моркови</param>
    public void InitCarrotStream(BaseHoleTriggerObserver _baseHoleTriggerObserver, Collider2D _collider, SpriteRenderer _sprite)
    {
        _baseHoleTriggerObserverCarrot = _baseHoleTriggerObserver;
        _colliderCarrotStream = _collider;
        _spriteRendererCarrotStream = _sprite;
        StartDragCarrot?.Invoke();
        
        CarrotStreamSubAction();
    }
    /// <summary>
    /// События на начало и окончание перемещения игроком
    /// </summary>
    private void CarrotSubAction()
    {
        SetActionsToDragAndDrop(onDragStart: EnableWaterCarrot, onDragEnd: DisableWaterCarrot);
    }
    /// <summary>
    /// Присваивагие события при нахождении в коллайдере ямы
    /// </summary>
    private void CarrotStreamSubAction()
    {
        _baseHoleTriggerObserverCarrot.OnTriggerStay += PureHole;
    }
    /// <summary>
    /// Отписка от события с нахождением в коллайдере ямы
    /// </summary>
    private void OnCarrotPumpStreamDisable()
    {
        if (_baseHoleTriggerObserverCarrot != null)
        {
            _baseHoleTriggerObserverCarrot.OnTriggerStay -= PureHole;
        }
    }
    /// <summary>
    /// Использование воды и включение спрайта
    /// </summary>
    private void EnableWaterCarrot()
    {
        EnableWater(_colliderCarrotStream, _spriteRendererCarrotStream);
    }
    /// <summary>
    /// отмена потока воды и выключение спрайта
    /// </summary>
    private void DisableWaterCarrot()
    {
        DisableWater(_colliderCarrotStream, _spriteRendererCarrotStream);
    }

    #endregion Carrot realization

    #region Apple realization
    /// <summary>
    /// Инициализация яблока с присваиванием класса DragAndDrop и включения метода с событиями
    /// </summary>
    /// <param name="dragAndDrop">класс для хватания и перемещения</param>
    public void InitApple(DragAndDrop dragAndDrop)
    {
        _dragAndDrop = dragAndDrop;

        AppleSubAction();
    }
    /// <summary>
    /// Инициализация яблока с текстурой, коллайдером
    /// </summary>
    /// <param name="_collider">коллайдер яблока</param>
    /// <param name="_sprite">текстура яблока</param>
    /// <param name="_appleSeedlingTriggerObserverApple">класс, что наследует TriggerObserverWithPayload</param>
    /// <param name="_spritePipe">спрайт трубы</param>
    /// <param name="_observerAppleBaseHole">наблюдатель за ямой для яблони</param>
    public void InitAppleStream(Collider2D _collider, SpriteRenderer _sprite, AppleSeedlingTriggerObserver _appleSeedlingTriggerObserverApple = null, SpriteRenderer _spritePipe = null, BaseHoleTriggerObserver _observerAppleBaseHole = null)
    {

        _colliderAppleStream = _collider;
        _spriteRendererAppleStream = _sprite;
        
        if (_appleSeedlingTriggerObserverApple != null)
        {
            this._appleSeedlingTriggerObserverApple = _appleSeedlingTriggerObserverApple;
        }
        if (_spritePipe != null)
        {
            _spriteRendererAppleStreamPipe = _spritePipe;
        }

        AppleStreamSubAction();
    }
    /// <summary>
    /// События на начало полива и окончание полива игроком, так же деактивация подсказки
    /// </summary>
    private void AppleSubAction()
    {
        SetActionsToDragAndDrop(onDragStart: DeactivateHint);
        SetActionsToDragAndDrop(onDragStart: StartWateringVisual, onDragEnd: StopWateringVisual);
    }
    /// <summary>
    /// Подписка на события при попадании потока воды или выход - совершается действие
    /// </summary>
    private void AppleStreamSubAction()
    {
        if (_observerApple != null)
        {
            _observerApple.OnTriggerStay += PureHole;
        }
        this._appleSeedlingTriggerObserverApple.OnTriggerEnter += StartWateringHole;
        this._appleSeedlingTriggerObserverApple.OnTriggerExit += StopWateringHole;
    }
    /// <summary>
    /// Отмена событий на полив
    /// </summary>
    private void OnApplePumpStreamDisable()
    {
        if (_observerApple != null)
        {
            _observerApple.OnTriggerStay -= PureHole;
        }

        if (_appleSeedlingTriggerObserverApple != null)
        {
            _appleSeedlingTriggerObserverApple.OnTriggerEnter -= StartWateringHole;
            _appleSeedlingTriggerObserverApple.OnTriggerEnter -= StopWateringHole;
        }
    }
    /// <summary>
    /// Включение потока воды для полива
    /// </summary>
    private void EnableWaterApple()
    {
        EnableWater(_colliderAppleStream, _spriteRendererAppleStream);
        _spriteRendererAppleStreamPipe.enabled = true;
    }
    /// <summary>
    /// Отключение потока воды для полива
    /// </summary>
    private void DisableWaterApple()
    {
        DisableWater(_colliderAppleStream, _spriteRendererAppleStream);
        _spriteRendererAppleStreamPipe.enabled = false;
    }
    /// <summary>
    /// Визуализирует воду и включает звук полива
    /// </summary>
    private void StartWateringVisual()
    {
        StartSound();
        EnableWaterApple();
    }
    /// <summary>
    /// Отключает воду и выключает звук полива
    /// </summary>
    private void StopWateringVisual()
    {
        StopSound();
        DisableWaterApple();
    }
    /// <summary>
    /// Начало поливании ямы
    /// </summary>
    /// <param name="seedlingsApple">росток яблони</param>
    private void StartWateringHole(SeedlingsApple seedlingsApple)
    {
        seedlingsApple.StopWaterHint();
        seedlingsApple.StartGrow();
    }
    /// <summary>
    /// Остановка роста ростка яблони
    /// </summary>
    /// <param name="seedlingsApple">росток яблони</param>
    private void StopWateringHole(SeedlingsApple seedlingsApple)
    {
        seedlingsApple.StopGrow();
    }

    #endregion Apple realization

    #region Regular Functions
    /// <summary>
    /// Использование воды, включение коллайдера, спрайта и звука
    /// </summary>
    /// <param name="_collider">коллайдер воды</param>
    /// <param name="_spriteRenderer">текстура воды</param>
    private void EnableWater(Collider2D _collider, SpriteRenderer _spriteRenderer)
    {

        _collider.enabled = true;
        _spriteRenderer.enabled = true;
        StartSound();
    }
    /// <summary>
    /// Отключение воды, выключение коллайдера, спрайта и звука
    /// </summary>
    /// <param name="_collider"></param>
    /// <param name="_spriteRenderer"></param>
    private void DisableWater(Collider2D _collider, SpriteRenderer _spriteRenderer)
    {
        _collider.enabled = false;
        _spriteRenderer.enabled = false;
        StopSound();
    }
    /// <summary>
    /// Подключение событий на начало, во время и окончание перемещения
    /// </summary>
    /// <param name="onDragStart">событие на начало перемещения</param>
    /// <param name="onDragStay">событие действие перемещения</param>
    /// <param name="onDragEnd">событие на окончание перемещения</param>
    private void SetActionsToDragAndDrop(Action onDragStart = null, Action onDragStay = null, 
        Action onDragEnd = null)
    {
        _dragAndDrop.OnDragStart += onDragStart;
        _dragAndDrop.OnDrag += onDragStay;
        _dragAndDrop.OnDragEnded += onDragEnd;
    }

    #endregion Regular Functions
    
    #region Trivial Functions
    /// <summary>
    /// Отписывает всех от событий триггера
    /// </summary>
    private void ClearAllSub()
    {
        OnTriggerEnterSub = null;
        OnTriggerStaySub = null;
        OnTriggerEndSub = null;
    }
    /// <summary>
    /// Отписывает всех от событий триггера
    /// </summary>
    private void ClearAllUnsub()
    {
        OnTriggerEnterUnsub = null;
        OnTriggerStayUnsub = null;
        OnTriggerEndUnsub = null;
    }
    /// <summary>
    /// Прогресс полива
    /// </summary>
    /// <param name="hole">прогресс полива</param>
    private void PureHole(BaseHole hole)
        => hole.AddProgress(Time.deltaTime * 5);
    /// <summary>
    /// Воспроизвести звук воды
    /// </summary>
    private void StartSound() => SoundSystemUser.Instance.PlaySound("Water");
    /// <summary>
    /// Остановить звук воды
    /// </summary>
    private void StopSound() => SoundSystemUser.Instance.StopSound("Water");
    /// <summary>
    /// Отключить подсказку
    /// </summary>
    private void DeactivateHint() => HintSystem.Instance.HidePointerHint();
    /// <summary>
    /// Отмена класса на перемещение и хватание
    /// </summary>
    private void OnDragAndDropDisable()
    {
        if (_dragAndDrop != null) _dragAndDrop = null;
    }

    #endregion Trivial Functions
}
