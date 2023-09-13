using UnityEngine;
using DG.Tweening;
using System.Collections;
/// <summary>
/// Класс жизненого цикла объекта и перемещения семян
/// </summary>
public class ContactArea : MonoBehaviour
{
    [SerializeField] private bool DontDestroyOnLoad;

    public static ContactArea Instance { get; set; }

    private GameObject objectToDestroy;
    /// <summary>
    /// Инициализирует класс ContactArea, если уже был, то уничтожает объект
    /// </summary>
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
    /// <summary>
    /// Ожидание окончания жизненого цикла, а после уничтожает объект
    /// </summary>
    /// <param name="objectToDestroy">объект, что нужно уничтожить</param>
    /// <param name="timeToDestruction">время жизни объекта в секундах</param>
    public void WaitingForEndLifecycle(GameObject objectToDestroy, float timeToDestruction)
    {
        this.objectToDestroy = objectToDestroy;
        if(timeToDestruction != 0)
        {
            StartCoroutine(CalculateLifetime(timeToDestruction));
        }
    }
    /// <summary>
    /// подсчёт секунд до уничтожения объекта
    /// </summary>
    /// <param name="timeToDestruction">время жизни объекта в секундах</param>
    /// <returns>возвращает текущее время</returns>
    private IEnumerator CalculateLifetime(float timeToDestruction)
    {
        Debug.Log("CalculateLifetime");
        yield return new WaitForSeconds(timeToDestruction);
        Destroy(objectToDestroy);
    }
    /// <summary>
    /// Уничтожение объекта
    /// </summary>
    public void DestroyNow()
    {
        Destroy(objectToDestroy);
    }
    
    /// <summary>
    /// Анимация Tween для перемещения объекта в позицию с указанием времени
    /// </summary>
    /// <param name="Point">вектор позиции перемещения</param>
    /// <param name="obj">объект перемещения</param>
    /// <param name="duration">длительность перемещения</param>
    /// <returns></returns>
    public Tween MoveSeedToHole(Vector3 Point, Transform obj, float duration)
        => obj.DOMove(Point, duration);
    /// <summary>
    /// Дополнительная анимация Tween для перемещения объекта в позицию с указанием времени
    /// </summary>
    /// <param name="Point">вектор позиции перемещения</param>
    /// <param name="obj">объект перемещения</param>
    /// <param name="duration">длительность перемещения</param>
    /// <returns></returns>
    public Tween AdditionalMovementToHole(Vector3 Point, Transform obj, float duration)
        => obj.DOMove(Point, duration);
}
