using System.Collections;
using AwesomeTools.Inputs;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using AwesomeTools;

namespace SunflowerScene
{
    public class Seed : MonoBehaviour, ICollectable
    {
        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private Transform[] _parts;
        [SerializeField] private PlacementByOffset _placement;
        [SerializeField] private SpriteRenderer[] _sprites;
        private string _def = "Default";
        private WaitForSeconds timI = new WaitForSeconds(.36f);
        private IEnumerator _coroutine;
        public Transform[] Parts => _parts;
        private SunflowerSeedPosInBasket _seedPosInBasket;
        public bool Collected { get; private set; }

        public void Construct(Vector3 destination, InputSystem input,SunflowerSeedPosInBasket seedPosInBasket)
        {
            _dragAndDrop.Construct(input);
            _destinationOnDragEnd.Construct(destination);
            _dragAndDrop.IsDraggable = false;
            _seedPosInBasket = seedPosInBasket;
        }

        public void SetCollected()
        {
            _dragAndDrop.IsDraggable = false;
            Collected = true;
        }
        
        private void SetDraggable()
        {
            foreach (var item in _sprites)
            {
                item.GetComponent<SpriteRenderer>().sortingOrder = 20;
            }

            _dragAndDrop.IsDraggable = true;
        }

        public void Enable(Vector3 startPos, Vector3 offset)
        {
            transform.position += offset;
            foreach (var part in _parts)
            {
                part.position = startPos;
                part.localScale = Vector3.zero;
            }

            var placer = new SeedPlacer(_parts, SetDraggable, _placement);
            placer.Place();
        }

        public void MakeVisualPartOf(SpriteRenderer _frontBasketSprite, int _sortingIndex, int _sortingLayer)
        {
            foreach (var item in _sprites)
            {
                item.sortingOrder = _sortingIndex;
            }

            MoveOtherParts();
        }

        private void MoveOtherParts()
        {
            foreach (var sprite in _sprites)
                sprite.sortingLayerName = _def;

            for (int i = 0; i < 3; i++)
            {
                _parts[i].transform.parent = transform.parent;
            }
            _coroutine = NextMove();
            StartCoroutine(_coroutine);
        }

        private IEnumerator NextMove()
        {
            ChangePosSeed(_parts[2]);
            yield return timI;
            ChangePosSeed(_parts[1]);
            yield return timI;
            ChangePosSeed(_parts[0]);
            Invoke(nameof(SetParentSeed),.7f);
            StopCoroutine(_coroutine);
        }

        private void SetParentSeed()
        {
            for (int i = 0; i < 3; i++)
            {
                _parts[i].transform.parent = transform;
            }
        }
        private void ChangePosSeed(Transform seed)
        {
            seed.transform.DOMove(_seedPosInBasket.GetPos(), .65f).SetEase(Ease.OutCirc)
                .SetLink(seed.gameObject);
            seed.transform.DORotate(Vector3.zero, .45f).SetEase(Ease.OutCirc)
                .SetLink(seed.gameObject);
        }
    }
}