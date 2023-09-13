using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float _alpha;
    [SerializeField] private Transform _sunActivePosition;
    [SerializeField] private Transform _sunStartPosition;
    [SerializeField] List<SpriteRenderer> sunBeams = new List<SpriteRenderer>();

    public Action CallbackAction { get; private set; }

    // Activates the sunshine 
    public void Sunshine(float duration, Action OnEndRipe = null)
    {
        CallbackAction = OnEndRipe;
        Appear();
        StartCoroutine(ShineRoutine(duration));
    }


    // Rotates the sun, shines the sun beams, and waits for the specified duration
    private IEnumerator ShineRoutine(float duration)
    {
        transform.DORotate(new Vector3(0, 0, 180), 100);
        foreach (var beam in sunBeams)
        {
            Shine(beam, duration, _alpha);
        }
        yield return new WaitForSeconds(duration);
        EndRipe();
    }

    // Hides the sun by moving it to its start position and fading out the sun beams.
    private void Hide()
    {
        transform.DOMove(_sunStartPosition.position, 1f);
        foreach (var beam in sunBeams)
        {
            Shine(beam, 1f, 0);
        }
    }

    // Moves the sun to its active position.
    private void Appear() => transform.DOMove(_sunActivePosition.position, 1f);

    // Shines a sun beam by adjusting its alpha value over a certain duration.
    private void Shine(SpriteRenderer beam, float duration, float alpha)
    {
        var color = beam.color;
        beam.DOColor(new Color(color.r, color.g, color.b, alpha), duration).SetLink(beam.gameObject);
    }

    // Ends the ripe effect by hiding the sun and invoking the OnEndRipe action.
    private void EndRipe()
    {
        Hide();
        CallbackAction?.Invoke();
    }
}
