using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitManager : MonoBehaviour
{
    [SerializeField] private Image bar;
    [SerializeField] private Image barBg;
    [SerializeField] private Image crosshair;

    void Start()
    {
        barBg.gameObject.SetActive(false);
    }

    public void TriggerWait(float value, float duration)
    {
        barBg.gameObject.SetActive(true);
        StartCoroutine(WaitLerp(value, duration));
    }

    IEnumerator WaitLerp(float targetValue, float duration)
    {
        barBg.gameObject.SetActive(true);
        bar.fillAmount = 0;

        float startValue = bar.fillAmount;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            bar.fillAmount = Mathf.Lerp(startValue, targetValue, elapsedTime / duration);
            yield return null;
        }

        bar.fillAmount = targetValue;

        if (bar.fillAmount == 1f)
        {
            barBg.gameObject.SetActive(false);
        }
    }

    public void CrosshairActive(bool isActive)
    {
        crosshair.enabled = isActive;
    }
}