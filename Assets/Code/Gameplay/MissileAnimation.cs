using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MissileAnimation : MonoBehaviour
{
    [Header("Light Animation")]
    [SerializeField] private float initialLightIntensity = 5f;
    [SerializeField] private float midLightIntensity = 25f;
    private const float finalLightIntensity = 0f;
    private const float initialFade = 1f;
    private const float midFade = 0.75f;
    private const float finalFade = 0f;
    private const float initialBorderThickness = 0.2f;
    private const float finalBorderThickness = 1f;

    [Header("Global Light Dimming")]
    [SerializeField] private bool globalLightDimmingEnabled = false;
    [SerializeField] private float globalLightStartIntensity = 1f;
    [SerializeField] private float globalLightDimIntensity = 0.3f;
    [SerializeField] private float globalLightDimDuration = 0.2f;
    [SerializeField] private float globalLightStayDimmedTime = 0.3f;

    private Renderer materialRenderer;
    private Light2D light2D;
    private Light2D globalLight;
    private MissileExplosion missileExplosion;


    private void Start()
    {
        materialRenderer = GetComponent<Renderer>();
        light2D = GetComponent<Light2D>();
        missileExplosion = GetComponent<MissileExplosion>();
        globalLight = GameManager.Instance.GetGlobalLight();

        StartCoroutine(AnimateDissolveEffect());
    }


    private IEnumerator AnimateDissolveEffect()
    {
        float halfDuration = missileExplosion.duration / 2;

        if (globalLightDimmingEnabled)
        {
            StartCoroutine(DimGlobalLight(globalLightStartIntensity, globalLightDimIntensity, globalLightDimDuration, globalLightStayDimmedTime));
        }

        yield return StartCoroutine(AnimateLightAndFade(initialFade, midFade, initialLightIntensity, midLightIntensity, halfDuration));
        yield return StartCoroutine(AnimateLightAndFade(midFade, finalFade, midLightIntensity, finalLightIntensity, halfDuration * 0.5f));

        Destroy(gameObject);
    }


    private IEnumerator DimGlobalLight(float start, float end, float duration, float stayDimmedTime)
    {
        float timer = 0;
        while (timer < duration)
        {
            float progress = timer / duration;
            globalLight.intensity = Mathf.Lerp(start, end, progress);
            timer += Time.deltaTime;
            yield return null;
        }

        globalLight.intensity = end;

        yield return new WaitForSeconds(stayDimmedTime);

        timer = 0;
        while (timer < duration)
        {
            float progress = timer / duration;
            globalLight.intensity = Mathf.Lerp(end, start, progress);
            timer += Time.deltaTime;
            yield return null;
        }

        globalLight.intensity = start;
    }


    private IEnumerator AnimateLightAndFade(float startFade, float endFade, float startIntensity, float endIntensity, float duration)
    {
        float timer = 0;
        while (timer < duration)
        {
            float progress = timer / duration;
            materialRenderer.material.SetFloat("_Fade", Mathf.Lerp(startFade, endFade, progress));
            materialRenderer.material.SetFloat("_BorderThickness", Mathf.Lerp(initialBorderThickness, finalBorderThickness, progress));
            light2D.intensity = Mathf.Lerp(startIntensity, endIntensity, progress);
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
