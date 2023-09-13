using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FishBody : MonoBehaviour
{
    [field:SerializeField] public List<SpriteRenderer> SpriteRenderers { get; private set; }
    public Animator Animator { get; private set; }
    
    /// <summary>
    /// Присвоюємо публічному аніматору елементу [Animator] аніматор з ігрового об'єкту
    /// </summary>
    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }
    
    /// <summary>
    /// Вводимо номер в слою [SortOrderIndex] -
    /// присвоюємо всім рендерерам зі списку "SpriteRenderers" номер в слою [SortOrderIndex]
    /// </summary>    
    public void ChangeSpriteSortOrder(int SortOrderIndex)
    {
        foreach (SpriteRenderer spriteRenderer in SpriteRenderers)
        {
            spriteRenderer.sortingOrder += SortOrderIndex;
        }
    }
}