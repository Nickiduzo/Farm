using Apple.Spawners;
using System;
using System.Linq;
using UnityEngine;

namespace Apple
{
    public class AppleFertilizerContainer : HolesContainer<AppleFertilizerPlace>
    {
        public event Action OnAllPlacesFertilized;

        // Initialize fertilizer placement objects
        public void Init()
        {
            foreach (var place in _holesOnScene)
            {
                place.OnFertilized += CheckIfAllPlacesFertilized;
                place.SetupToFertilize();
            }
        }

        // Set the places ready for fertilization
        public void SetPlacesReadyToFertilize()
        {
            AppleFertilizerPlace place = _holesOnScene[0];
            place.OnFertilized += SetPlacesReadyToFertilize;
            place.SetupToFertilize();
        }

        // Check if all places are fertilized
        public void CheckIfAllPlacesFertilized()
        {
            if (_holesOnScene.All(x => x.IsFertilized))
            {
                AllPlacesFertilized();
            }
        }

        // Reduce the visibility of the holes by half
        public void HalfVisibilityHoles()
        {
            foreach (var hole in _holesOnScene)
            {
                hole.HalfVisibility();
            }
        }

        // Invoke when all places are fertilized
        private void AllPlacesFertilized()
        {
            OnAllPlacesFertilized?.Invoke();
        }

    }
}