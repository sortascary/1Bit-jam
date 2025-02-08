using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Material ScreenTransitionMaterial;
    [SerializeField] private float TransitionSpeed = 1.0f;
    [SerializeField] private string propertyName= "_Progress";
    [SerializeField] private bool isOpening;

    public UnityEvent OnTransitionComplete;
    private void Start()
    {
        if (isOpening)
        StartCoroutine(TransitionCourutine());
        else
            ScreenTransitionMaterial.SetFloat(propertyName, 1f);
    }

    public void FadeOut()
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

    public void FadeIn()
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        OnTransitionComplete?.Invoke();
    }
}
