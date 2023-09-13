using Carrot.Config;
using AwesomeTools.Sound;
using System;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UsefulComponents;

namespace Carrot.Spawners
{
    public class MoleSpawnController : MonoBehaviour, IProgressWriter
    {
        [SerializeField] private CarrotLevelConfig _config;
        [SerializeField] private MoleSpawner _moleSpawner;
        [SerializeField] private SoundSystem _soundSystem;
        public event Action OnProgressChanged;
        public event Action OnWholeMoleCatch;
        public int CurrentProgress => _wholeMoleCatch;
        public int MaxProgress => _config.CatchMoleToWin;
        private int _moleToSpawn;
        private int _wholeMoleCatch;
        private List<Mole> _moles = new List<Mole>();

        // spawn first mole
        public void StartMoleSpawning()
        {
            _moleToSpawn = 1;
            SpawnMole(true);
        }

        // hide all moles and spawn new one, subscribe Actions [OnDied] and [OnHide], add to [_moles] List 
        private void SpawnMole(bool isFirstSpawn = false)
        {
            if (CatchAllMoles())
            {
                HideAllMoles();
                OnWholeMoleCatch?.Invoke();
                return;
            }

            for (int i = 0; i < _moleToSpawn; i++)
            {
                _soundSystem.PlaySound("cameIn");
                Mole spawnedMole = _moleSpawner.SpawnMole();
                _moles.Add(spawnedMole);
                RegisterMole(spawnedMole);

            }

            if (isFirstSpawn)
            {
                ShowStaticPointer(_moles[0].transform.position);
                _moles[0].OnDied += HideHint;
                _moles[0].OnHied += HideHint;
            }
        }

        // disable hint, unsubscribe Actions [OnDied] and [OnHide]
        private void HideHint(Mole mole)
        {
            HidePointerSystem();
            mole.OnDied -= HideHint;
            mole.OnHied -= HideHint;
        }

        // invoke [Appear] in mole, subscribe Actions [OnDied], [OnHide] and [OnCatch]
        private void RegisterMole(Mole mole)
        {
            mole.Appear();
            mole.OnCatch += MoleCatch;
            mole.OnDied += MoleDie;
            mole.OnHied += MoleHie;
            mole.IsRegistered = true;
        }

        // remove certain mole from [_moles] List, unsubscribe Actions [OnDied], [OnHide] and [OnCatch]
        private void RemoveMole(Mole mole)
        {
            mole.OnCatch -= MoleCatch;
            mole.OnDied -= MoleDie;
            mole.OnHied -= MoleHie;
            mole.IsRegistered = false;
            _moles.Remove(mole);
        }

        // add plus one caught mole, invoke Action [OnProgressChanged]
        private void MoleCatch()
        {
            _wholeMoleCatch++;
            OnProgressChanged?.Invoke();
        }

        // remove mole from List and spawn new one
        private void MoleDie(Mole mole)
        {
            int flag = mole.SpawnPointIndex;
            RemoveMole(mole);
            CalculateSpawnNumber();
            SpawnMole();
            _moleSpawner.ResetFlag(flag);
        }

        // hide all moles from [_moles] List 
        private void HideAllMoles()
        {
            foreach (var mole in _moles)
            {
                mole.Hide();
            }
        }

        // set "flag" for mole spawn point and spwan mole
        private void MoleHie(Mole mole)
        {
            _moleToSpawn = 1;
            int flag = mole.SpawnPointIndex;
            RemoveMole(mole);

            SpawnMole();
            _moleSpawner.ResetFlag(flag);
        }

        // check if all moles are caught, if so, return true
        private bool CatchAllMoles() => _wholeMoleCatch >= _config.CatchMoleToWin;

        // set how many moles need to spawn
        private void CalculateSpawnNumber()
            => _moleToSpawn = _moles.Count + 2 <= _config.MaxMoleToSpawn ? 2 : 0;
        
        // appear hint on firs mole
        private void ShowStaticPointer(Vector3 position)
            => HintSystem.Instance.ShowPointerHint(position + Vector3.up);

        // disable hint
        private void HidePointerSystem()
            => HintSystem.Instance.HidePointerHint();
    }
}