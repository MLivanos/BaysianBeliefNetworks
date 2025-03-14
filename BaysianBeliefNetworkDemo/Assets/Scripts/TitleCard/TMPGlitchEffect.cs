using UnityEngine;
using TMPro;
using System.Collections;

public class TMPGlitchEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text tmpText;
    [SerializeField] private float glitchIntensity = 5f;  // How far vertices are offset.
    [SerializeField] private float glitchDuration = 0.1f;   // How long the glitch effect is visible.
    [SerializeField] private float glitchInterval = 5f;     // How long to wait between glitch bursts.
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
        while (effectOn)
        {
            tmpText.ForceMeshUpdate();
            TMP_TextInfo textInfo = tmpText.textInfo;

            // Create a copy of the original vertices for each mesh so we can restore them later.
            Vector3[][] originalVertices = new Vector3[textInfo.meshInfo.Length][];
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                originalVertices[i] = (Vector3[])textInfo.meshInfo[i].vertices.Clone();
            }

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

            yield return new WaitForSeconds(glitchDuration);

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = originalVertices[i];
                tmpText.UpdateGeometry(meshInfo.mesh, i);
            }

            yield return new WaitForSeconds(glitchInterval);
        }
    }

    public void TurnOff()
    {
        effectOn = false;
    }
}
