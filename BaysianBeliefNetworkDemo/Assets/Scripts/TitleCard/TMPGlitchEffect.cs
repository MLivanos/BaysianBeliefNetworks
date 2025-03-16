using UnityEngine;
using TMPro;
using System.Collections;

public class TMPGlitchEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpText;
    [SerializeField] private GameObject alienText;
    [SerializeField] private GameObject alienTitle;
    [SerializeField] private GameObject humanTitle;
    [SerializeField] private float glitchIntensity = 5f;
    [SerializeField] private float glitchDuration = 0.1f;
    [SerializeField] private float alienTextMultiplier = 5f;
    [SerializeField] private Vector2 glitchInterval = new Vector2(4f,6.5f);
    private bool effectOn = true;

    void Awake()
    {
        if (tmpText == null) tmpText = GetComponent<TMP_Text>();
    }

    void Start()
    {
        StartCoroutine(GlitchRoutine());
    }

    IEnumerator GlitchRoutine()
    {
        TMP_TextInfo textInfo = tmpText.textInfo;
        tmpText.ForceMeshUpdate();
        Vector3[][] originalVertices = new Vector3[textInfo.meshInfo.Length][];
        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            originalVertices[i] = (Vector3[])textInfo.meshInfo[i].vertices.Clone();
        }

        while (effectOn)
        {
            tmpText.ForceMeshUpdate();
            yield return new WaitForSeconds(GetRandomTime());
            float duration = glitchDuration;
            AudioManager.instance.PlayEffect("MiniGlitch");

            if (Random.value >= 0.5f)
            {
                yield return OffsetTextKTimes(3);
                ShowAlienText();
                duration *= alienTextMultiplier;
            }
            else OffsetText();
            yield return new WaitForSeconds(duration);
            ResetText(originalVertices);
        }
    }

    private void ShowAlienText()
    {
        tmpText.gameObject.SetActive(false);
        humanTitle.SetActive(false);
        alienText.SetActive(true);
        alienTitle.SetActive(true);
    }

    private IEnumerator OffsetTextKTimes(int k)
    {
        for(int i=0; i<k; i++)
        {
            yield return new WaitForSeconds(glitchDuration/3);
            OffsetText();
        }
    }

    private void OffsetText()
    {
        TMP_TextInfo textInfo = tmpText.textInfo;

        // Apply the glitch offsets to each visible character.
        int characterCount = textInfo.characterCount;
        if (characterCount > 0)
        {
            for (int i = 0; i < characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

                if (!charInfo.isVisible)
                    continue;

                int vertexIndex = charInfo.vertexIndex;
                int materialIndex = charInfo.materialReferenceIndex;
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                Vector3 offset = new Vector3(
                    Random.Range(-glitchIntensity, glitchIntensity),
                    Random.Range(-glitchIntensity, glitchIntensity),
                    0f
                );

                vertices[vertexIndex + 0] += offset;
                vertices[vertexIndex + 1] += offset;
                vertices[vertexIndex + 2] += offset;
                vertices[vertexIndex + 3] += offset;
            }

            // Update geometry with glitched vertices.
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = meshInfo.vertices;
                tmpText.UpdateGeometry(meshInfo.mesh, i);
            }
        }
    }

    private void ResetText(Vector3[][] originalVertices)
    {
        tmpText.gameObject.SetActive(true);
        humanTitle.SetActive(true);
        for (int i = 0; i < tmpText.textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo = tmpText.textInfo.meshInfo[i];
            meshInfo.mesh.vertices = originalVertices[i];
            tmpText.UpdateGeometry(meshInfo.mesh, i);
        }
        alienText.SetActive(false);
        alienTitle.SetActive(false);
    }

    private float GetRandomTime()
    {
        return Random.Range(glitchInterval.x, glitchInterval.y);
    }

    public void TurnOff()
    {
        effectOn = false;
    }
}
