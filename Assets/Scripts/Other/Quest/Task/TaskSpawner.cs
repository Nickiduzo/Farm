using System.Collections.Generic;
using Scene;
using AwesomeTools.Sound;
using UnityEngine;

namespace Quest
{
    /// <summary>
    /// Спавнер задания
    /// </summary>
    public class TaskSpawner : MonoBehaviour
    {
        private const string PreviousTaskId = "PreviousTaskId";

        [SerializeField] private Transform _taskSpawnPoint;
        [SerializeField] private SaveLoadSystem _saveLoad;
        [SerializeField] private List<Task> _tasks;
        [SerializeField] private List<bool> _tasksEnabled;
        [SerializeField] private SceneLoader _sceneLoader;
        [SerializeField] private SoundSystem _soundSystem;

        private int _previousTaskId;
        private List<int> _enabledTasks = new();

        /// <summary>
        /// При запуску скрипту виконує ф-цію "CheckTasksCount"  
        /// </summary>
        void Start()
        {
            CheckTasksCount();
        }

        /// <summary>
        /// Перевіряє к-сть завдань в списку "_tasks" та к-сть дозволених завдань в списку "_tasksEnabled" 
        /// </summary>
        private void CheckTasksCount()
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                if (_tasks[i] == null)
                    _tasks.RemoveAt(i);
            }
            if (_tasksEnabled.Count < _tasks.Count)
            {
                int addEnabledTasksCount = _tasks.Count - _tasksEnabled.Count;
                for (int i = 0; i < addEnabledTasksCount; i++)
                {
                    _tasksEnabled.Add(true);
                }
            }
            else
            {
                while (_tasksEnabled.Count > _tasks.Count)
                {
                    _tasksEnabled.RemoveAt(_tasksEnabled.Count - 1);
                }
            }
        }

        /// <summary>
        /// Используется для создания рандомного задания
        /// </summary>
        /// <returns>возвращает id задания</returns>
        public Task SpawnRandomTask()
        {            
            GetEnabledTasks();
            _previousTaskId = GetPreviousTaskId();
            int taskId = GetRndTaskId();
            SetPreviousTaskId(taskId);
            return SpawnTask(taskId);
        }

        /// <summary>
        /// Додає дозволені завдання в список "_enabledTasks"
        /// </summary>
        private void GetEnabledTasks()
        {
            _enabledTasks.Clear();
            for (int i = 0; i < _tasks.Count; i++)
            {
                if(_tasksEnabled[i]) _enabledTasks.Add(i);   
            }
        }

        /// <summary>
        /// Создаёт задачу на основе ID
        /// </summary>
        /// <param name="taskID">id задания</param>
        /// <returns>возвращает задания по id</returns>
        public Task SpawnTask(int taskID)
        {
            Task spawnedTask = Instantiate(_tasks[taskID], _taskSpawnPoint.position, _tasks[taskID].transform.rotation);
            spawnedTask.Construct(_soundSystem, _sceneLoader, _saveLoad);
            return spawnedTask;
        }

        /// <summary>
        /// Движение к следующему заданию
        /// </summary>
        /// <returns>возвращает рандомное значение</returns>
        private int GetRndTaskId()
        {
            print(_enabledTasks.Count);
            if (_enabledTasks.Count <= 1)
            {
                return -1;
            }
            else if (_enabledTasks.Count == 1)
            {
                return _enabledTasks[0];
            }
            else
            {
                int rndNumber = Random.Range(0, _enabledTasks.Count);

                while (_enabledTasks[rndNumber] == _previousTaskId)
                {
                    rndNumber = Random.Range(0, _enabledTasks.Count);
                }

                return _enabledTasks[rndNumber];
            }
        }
        
        /// <summary>
        /// Сохраняет предыдущий id задачи в префабе
        /// </summary>
        /// <param name="id">id задания</param>
        private void SetPreviousTaskId(int id)
            => PlayerPrefs.SetInt(PreviousTaskId, id);

        /// <summary>
        /// Получает предыдущий id задания
        /// </summary>
        /// <returns>возвращает id задания</returns>
        private int GetPreviousTaskId()
            => PlayerPrefs.GetInt(PreviousTaskId);
    }
}