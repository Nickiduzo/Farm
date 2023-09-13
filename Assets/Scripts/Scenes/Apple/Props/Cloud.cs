using DG.Tweening;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    [SerializeField] private Transform _endPosition;
    [SerializeField] private float _moveDuration = 25f;
    [SerializeField] private float _moveDelay = 25f;

    private Vector3 _startPosition;

    // Store the initial position of the object and initiate the object's movement
    private void Awake()
    {
        _startPosition = this.transform.position;
        Move();
    }

    //Move the clouds
    private void Move()
    {
        transform.position = _startPosition;
        transform.DOMoveX(_endPosition.position.x, _moveDuration)
            .SetDelay(Random.Range(1f, _moveDelay))
            .OnComplete(Move);
    }
}
