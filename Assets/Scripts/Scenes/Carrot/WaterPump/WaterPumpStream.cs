﻿using UnityEngine;


    public class WaterPumpStream : MonoBehaviour
    {
        [SerializeField] private CarrotSprinkler _carrotSprinkler;

        private void Start()
        {
            _carrotSprinkler.InitStream();
        }
    }

