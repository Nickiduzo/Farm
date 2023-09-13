using DG.Tweening;
using UnityEngine;

public class BeeCameraZoom : MonoBehaviour
{
    [SerializeField] private Transform _destinationPoint;
    [SerializeField] private float _duration;
    [SerializeField] private float _orthoSize;
    [SerializeField] private FadeScreenPanel _fadeScreenPanel;
    private Camera _camera;

    // It init camera
    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    // Zooms the camera.
    public Tween ZoomCamera()
    {
        return Zooming();
    }

    // Performs the zooming sequence.
    private Tween Zooming()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(_fadeScreenPanel.FadeIn());
        sequence.Append(transform.DOMove(_destinationPoint.position, 0f));
        sequence.Join(_camera.DOOrthoSize(_orthoSize, 0f));
        sequence.Append(_fadeScreenPanel.FadeOut());
        sequence.Play();

        return sequence;
    }

}
