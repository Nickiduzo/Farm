using UnityEngine;
/// <summary>
/// Класс разрушение объекта спустя задержку
/// </summary>
public class SelfDestroying : MonoBehaviour
{
    [SerializeField] private float _delay;
    /// <summary>
    /// Разрушение спустя время
    /// </summary>
    private void Awake()
      => Destroy(gameObject, _delay);
}