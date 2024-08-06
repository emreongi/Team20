using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ElectromagneticExplosion : MonoBehaviour
{
    [SerializeField] private List<Transform> explosionElements = new List<Transform>();
    private List<Vector3> explosionElementsOriginalDimensions = new List<Vector3>();
    [SerializeField] private List<Material> materials = new List<Material>();
    [SerializeField] private Color startColor;
    [SerializeField] private Color andColor;
    [SerializeField] private float lightPower = 100f;
    [SerializeField] private Light pointLight;
    [SerializeField] private float destroyTime = 3f;
    private CinemachineImpulseSource cinemachineImpulseSource;

    void Start()
    {
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();

        for (int i = 0; i < explosionElements.Count; i++)
        {
            explosionElementsOriginalDimensions.Add(explosionElements[i].localScale);
        }

        foreach (Material material in materials)
        {
            material.SetColor("_TintColor", startColor);
        }

        pointLight.intensity = 0;

        StartCoroutine(ExplosionProcess());

        Destroy(this.gameObject, destroyTime);
    }

    IEnumerator ExplosionProcess()
    {
        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(FadeOutLight(0.5f));

        yield return StartCoroutine(Saturator(new Vector3(0.1f, 0.1f, 0.1f), 1f));

        yield return new WaitForSeconds(0.1f);

        yield return StartCoroutine(Saturator(new Vector3(5f, 5f, 5f), 0.3f));

        yield return StartCoroutine(FadeOutLight(0.1f));

        yield return StartCoroutine(FadeOutMaterials(0.25f));
    }

    IEnumerator Saturator(Vector3 targetScaleMultiplier, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            foreach (Transform item in explosionElements)
            {
                int index = explosionElements.IndexOf(item);
                Vector3 originalScale = explosionElementsOriginalDimensions[index];
                Vector3 targetScale = Vector3.Scale(originalScale, targetScaleMultiplier);
                item.localScale = Vector3.Lerp(item.localScale, targetScale, time / duration);
            }

            time += Time.deltaTime;
            yield return null;
        }

        foreach (Transform item in explosionElements)
        {
            int index = explosionElements.IndexOf(item);
            Vector3 originalScale = explosionElementsOriginalDimensions[index];
            item.localScale = Vector3.Scale(originalScale, targetScaleMultiplier);
        }
    }

    IEnumerator FadeOutMaterials(float duration)
    {
        float time = 0;

        List<Color> originalColors = new List<Color>();
        foreach (Material material in materials)
        {
            originalColors.Add(material.GetColor("_TintColor"));
        }

        while (time < duration)
        {
            foreach (Material material in materials)
            {
                int index = materials.IndexOf(material);
                Color originalColor = originalColors[index];
                Color newColor = Color.Lerp(originalColor, andColor, time / duration);
                material.SetColor("_TintColor", newColor);
            }

            time += Time.deltaTime;
            yield return null;
        }

        foreach (Material material in materials)
        {
            material.SetColor("_TintColor", andColor);
        }
    }

    IEnumerator FadeOutLight(float duration)
    {
        float time = 0;

        while (time < duration)
        {
            pointLight.intensity = Mathf.Lerp(pointLight.intensity, lightPower, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        pointLight.intensity = lightPower;
    }

    void OnTriggerEnter(Collider collider)
    {
        ExplosionNoise();
        if (collider.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Düşman vurulduuuuuu");
            collider.gameObject.GetComponent<Enemy>().TakeDamage(0.3f);
        }

    }

    void ExplosionNoise()
    {
        cinemachineImpulseSource.GenerateImpulse();
    }
}
