using UnityEngine;

namespace ChickenScene.Entities
{   
    // use to get and set sprite for egg
    public class EggVisual : MonoBehaviour
    {
        [field:SerializeField] public SpriteRenderer SpriteRenderer { get; set; }
        [field:SerializeField] public Sprite BrokenEgg { get; private set; }
    }
}