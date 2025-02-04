using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Material ScreenTransitionMaterial;
    [SerializeField] private float TransitionSpeed = 1.0f;
    [SerializeField] private string propertyName= "_Progress";

    public UnityEvent OnTransitionComplete;
    private void Start()
    {
        StartCoroutine(TransitionCourutine());
    }

    private IEnumerator TransitionCourutine()
    {
        float currentTime = 0;
        while (currentTime < TransitionSpeed)
        {
            currentTime += Time.deltaTime * TransitionSpeed;
            ScreenTransitionMaterial.SetFloat(propertyName, Mathf.Clamp01(currentTime/TransitionSpeed));
            yield return null;
        }
        OnTransitionComplete?.Invoke();
    }

    public void ReverseTransition()
    {
        StartCoroutine(TransitionCourutineReverse());
    }

    private IEnumerator TransitionCourutineReverse()
    {
        float currentTime = TransitionSpeed;
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime * TransitionSpeed;
            ScreenTransitionMaterial.SetFloat(propertyName, Mathf.Clamp01(currentTime / TransitionSpeed));
            yield return null;
        }
        OnTransitionComplete?.Invoke();
    }
}
