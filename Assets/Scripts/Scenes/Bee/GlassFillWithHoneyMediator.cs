using Bee.Config;
using Bee.Spawners;
using DG.Tweening;
using AwesomeTools.Sound;
using System;
using System.Collections.Generic;
using UnityEngine;
using UsefulComponents;
using Random = UnityEngine.Random;

namespace Bee
{
    public class GlassFillWithHoneyMediator : MonoBehaviour
    {
        private const string CLOSE = "Close";

        public event Action OnAllGlassFilled;

        [SerializeField] private BeeLevelConfig _config;
        [SerializeField] private List<Transform> _afterFillPoints;
        [SerializeField] private HoneyGlassSpawner _glassSpawner;
        private IHoneyTap _honeyTap;
        private IHoneyGlass _currentGlass;
        private List<IHoneyGlass> _honeyGlasses = new();

        private SoundSystem _soundSystem;
        private FxSystem _fxSystem;

        private Vector3 _glassHintPosition;

        //Initialize
        public void Init(IHoneyTap honeyTap)
        {
            _honeyTap = honeyTap;
            _honeyTap.OnHoneyStreamEnabled += FillGlassWithHoney;
        }

        public void FirstExecute(SoundSystem soundSystem, FxSystem fxSystem)
        {
            _soundSystem = soundSystem;
            _fxSystem = fxSystem;
            Execute();
        }

        // Executes the main logic of filling honey glasses
        private void Execute()
        {
            _honeyGlasses.Add(_currentGlass = SpawnGlass());
            _currentGlass.GoToDestination()
                .OnComplete(() => _currentGlass.Open()
                    .OnComplete(_honeyTap.MakeInteractable));
        }

        // Handles the event when all honey glasses are filled
        private void AllHoneyFilled()
        {
            foreach (var glass in _honeyGlasses)
            {
                glass.InitOnDragEnd();
                glass.MakeInteractable();
            }

            _honeyTap.SetNewSortingOrder();
            OnAllGlassFilled?.Invoke();
            _honeyGlasses.Clear();
            HintSystem.Instance.ShowPointerHint(_glassHintPosition, _glassSpawner.HintPosition, 0.5f);
        }

        // Checks if enough honey glasses are spawned
        private bool IsSpawnEnough()
            => _honeyGlasses.Count >= _config.GlassToSpawn;

        // Fills the current glass with honey
        private void FillGlassWithHoney()
        {
            HintSystem.Instance.HidePointerHint();
            DOVirtual.DelayedCall(0.2f, () =>
            {
                _currentGlass.FillWithHoney().OnComplete(() =>
                {
                    _honeyTap.MakeNonInteractable();
                    _honeyTap.DisableHoneyStream().OnComplete(
                        () =>
                        {
                            _currentGlass.ShowSuccess();
                            _currentGlass.SetDestination(GetRandomPosition());
                            _currentGlass.MakeNonInteractable();
                            DOVirtual.DelayedCall(0.7f, () =>
                            {
                                _soundSystem.PlaySound(CLOSE);
                            });
                            _currentGlass.Close()
                                .OnComplete(()
                                    => 
                                    {
                                        _currentGlass.GoToDestination()
                                            .OnComplete(NextGlass);
                                    }
                                    );
                        });
                });
            }); 
        }

        // Gets a random position for the next glass.
        private Vector3 GetRandomPosition()
        {
            if (_afterFillPoints.Count == 1)
            {
                _glassHintPosition = _afterFillPoints[0].transform.position;
            }

            var point = _afterFillPoints[Random.Range(0, _afterFillPoints.Count)];
            _afterFillPoints.Remove(point);

            return point.position;
        }

        // Handles the next glass in the sequence
        private void NextGlass()
        {
            if (IsSpawnEnough())
            {
                AllHoneyFilled();
            }
            else
            {
                Execute();
            }
        }

        // Spawns a honey glass
        private IHoneyGlass SpawnGlass()
            => _glassSpawner.SpawnGlass(_soundSystem, _fxSystem);
    }
}