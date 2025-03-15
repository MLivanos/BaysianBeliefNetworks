using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayButtonGlitch : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private float glitchDuration = 1.0f;
    [SerializeField] private float glitchToFadeSpeedup = 2.5f;
    [SerializeField] private float stretchAmount = 2.5f;
    private Material glitchMaterial;

    private void Start()
    {
        glitchMaterial = new Material(buttonImage.material);
        buttonImage.material = glitchMaterial;
    }

    public void TriggerGlitch()
    {
        AudioManager.instance.PlayEffect("Glitch");
        StartCoroutine(GlitchRoutine());
    }

    private IEnumerator GlitchRoutine()
    {
        float elapsedTime = 0f;
        float originalWidth = buttonImage.rectTransform.sizeDelta.x;

        while (elapsedTime < glitchDuration)
        {
            float t = elapsedTime / glitchDuration;
            float newWidth = Mathf.Lerp(originalWidth, originalWidth * stretchAmount, t);
            
            buttonImage.rectTransform.sizeDelta = new Vector2(newWidth, buttonImage.rectTransform.sizeDelta.y);
            glitchMaterial.SetFloat("_GlitchIntensity", Mathf.Lerp(0, 1, Mathf.Clamp01(t*glitchToFadeSpeedup)));
            glitchMaterial.SetFloat("_GlitchOffset", Mathf.Lerp(0, 1, Mathf.Clamp01(t*glitchToFadeSpeedup)));
            glitchMaterial.SetFloat("_Spread", Mathf.Lerp(0f, 1f, t));
            glitchMaterial.SetFloat("_RGBSeparation", Mathf.Lerp(0f, 0.2f, t));

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        buttonImage.enabled = false;
    }
}
