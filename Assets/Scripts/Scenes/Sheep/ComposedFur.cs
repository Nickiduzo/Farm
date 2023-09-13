using DG.Tweening;
using AwesomeTools.Inputs;
using System.Collections.Generic;
using UnityEngine;
using AwesomeTools;
using AwesomeTools.Sound;
using System.Collections;

namespace Sheep
{
    public class ComposedFur : MonoBehaviour, ICollectable
    {
        private const string FALL_FUR = "FallFur";

        public List<SpriteRenderer> FurSprites => _furSprites;

        [SerializeField] private List<SpriteRenderer> _furSprites;
        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private float _movingDuration;
        private int _sortingLayerID;
        private Vector3 _startPoint;
        private SoundSystem _soundSystem;

        // It subscribes to events
        private void Awake()
        {
            _dragAndDrop.OnDragEnded += MoveToStartPosition;
            _dragAndDrop.OnDragEnded += TurnOffCollider;
            _sortingLayerID = SortingLayer.NameToID("Product");
        }

        /// <summary>
        /// Вимикає колайдер
        /// </summary>
        private void TurnOffCollider()
        {
            transform.GetComponent<Collider2D>().enabled = false;
        }

        // It unsubscribes from events
        private void OnDestroy()
        {
            _dragAndDrop.OnDragEnded -= MoveToStartPosition;
            _dragAndDrop.OnDragEnded -= TurnOffCollider;
        }

        // Disable dragging and make the object non-interactable
        public void Stored()
        {
            _dragAndDrop.IsDraggable = false;
            transform.GetComponent<BoxCollider2D>().enabled = false;
        }

        // Initialize the object with the given input system and starting point
        public void Construct(InputSystem inputSystem, Vector3 startPoint, SoundSystem soundSystem)
        {
            _startPoint = startPoint;
            _soundSystem = soundSystem;
            _dragAndDrop.Construct(inputSystem);
        }

        // Enable dragging and make the object interactable
        public void MakeInteractable()
        {
            _dragAndDrop.IsDraggable = true;
            SetSortingLayer();
        }

        // Set the sorting layer for the visual part of the object
        private void SetSortingLayer()
        {
            foreach (var item in FurSprites)
            {
                item.sortingLayerID = _sortingLayerID;
            }
        }

        /// <summary>
        /// Виконує ф-цію "MoveTo" та "StartCoroutine"
        /// </summary>
        private void MoveToStartPosition(){
            StartCoroutine(PlayWoolSound());
            MoveTo(_startPoint).OnComplete(() => 
            {
                transform.GetComponent<Collider2D>().enabled = true;
            });
        }

        /// <summary>
        /// Запускає звук приземлення вовни
        /// </summary>        
        private IEnumerator PlayWoolSound()
        {
            yield return new WaitForSeconds(_movingDuration -.2f);
            _soundSystem.PlaySound(FALL_FUR);
            StopCoroutine(PlayWoolSound());
        }

        // Move the object to the specified point using a tween animation
        public Tween MoveTo(Vector3 point)
            => transform.DOMove(point, _movingDuration);

        // Set the visual part of the object and its sorting order and layer
        public void MakeVisualPartOf(SpriteRenderer frontBasketSprite, int sortingIndex, int sortingLayer)
        {
            foreach (var item in FurSprites)
            {
                item.sortingOrder = sortingIndex;
                item.sortingLayerID = sortingLayer;
            }
        }

    }
}