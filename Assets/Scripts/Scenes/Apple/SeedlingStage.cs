using System;
using UnityEngine;

[Serializable]
public class SeedlingStage
{
    [SerializeField] private Sprite _stage;
    [SerializeField] private Vector3 _scale;
    [SerializeField] private Vector3 _position;

    public bool StagePassed { get; set; }
    public Sprite Stage => _stage;
    public Vector3 Scale => _scale;
    public Vector3 Position => _position;
}
