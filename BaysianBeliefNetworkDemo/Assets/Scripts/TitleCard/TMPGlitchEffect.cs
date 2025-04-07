using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class TMPGlitchEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text[] glitchTexts;
    [SerializeField] private float glitchIntensity = 5f;
    [SerializeField] private float glitchDuration = 0.1f;
    [SerializeField] private float alienTextMultiplier = 5f;
    [SerializeField] private Vector2 glitchInterval = new Vector2(4f,6.5f);

    [Header("Alien Text Objects (Optional)")]
    [SerializeField] private GameObject alienText;
    [SerializeField] private GameObject alienTitle;

    private Dictionary<TMP_Text, Vector3[][]> originalVerticesDict = new Dictionary<TMP_Text, Vector3[][]>();
    private bool effectOn = true;

    private void Start()
    {
        CollectOriginalVertices();
        StartCoroutine(GlitchRoutine());
    }

    private void CollectOriginalVertices()
    {
        foreach (TMP_Text text in glitchTexts)
        {
            text.ForceMeshUpdate();
            TMP_TextInfo textInfo = text.textInfo;
            Vector3[][] origVerts = new Vector3[textInfo.meshInfo.Length][];
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                origVerts[i] = (Vector3[])textInfo.meshInfo[i].vertices.Clone();
            }
            originalVerticesDict.Add(text, origVerts);
        }
    }

    private IEnumerator GlitchRoutine()
    {
        while (effectOn)
        {
            foreach (TMP_Text text in glitchTexts)
            {
                text.ForceMeshUpdate();
            }

            yield return new WaitForSeconds(GetRandomTime());
            float duration = glitchDuration;
            AudioManager.instance.PlayEffect("MiniGlitch");

            if (Random.value >= 0.5f)
            {
                yield return OffsetTextKTimes(3);
                ToggleAlienTextVisibility(true);
                duration *= alienTextMultiplier;
            }
            else
            {
                OffsetText();
            }
            yield return new WaitForSeconds(duration);
            ResetTexts();
        }
    }

    private IEnumerator OffsetTextKTimes(int k)
    {
        for (int i = 0; i < k; i++)
        {
            yield return new WaitForSeconds(glitchDuration / 3);
            OffsetText();
        }
    }

    private void OffsetText()
    {
        foreach (TMP_Text text in glitchTexts)
        {
            TMP_TextInfo textInfo = text.textInfo;
            ApplyVertexOffsets(text);
        }
    }

    private void ApplyVertexOffsets(TMP_Text text)
    {
        TMP_TextInfo textInfo = text.textInfo;
        int characterCount = textInfo.characterCount;
        if (characterCount <= 0) return; // Fixed misplaced continue

        for (int i = 0; i < characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
            if (!charInfo.isVisible) continue;

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

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
            meshInfo.mesh.vertices = meshInfo.vertices;
            text.UpdateGeometry(meshInfo.mesh, i);
        }
    }

    private void ResetTexts()
    {
        foreach (TMP_Text text in glitchTexts)
        {
            if (!originalVerticesDict.ContainsKey(text))
                continue;

            TMP_TextInfo textInfo = text.textInfo;
            Vector3[][] origVerts = originalVerticesDict[text];
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                TMP_MeshInfo meshInfo = textInfo.meshInfo[i];
                meshInfo.mesh.vertices = origVerts[i];
                text.UpdateGeometry(meshInfo.mesh, i);
            }
        }
        ToggleAlienTextVisibility(false);
    }

    private void ToggleAlienTextVisibility(bool alienTextOn)
    {
        if (alienText != null)
            alienText.SetActive(alienTextOn);
        if (alienTitle != null)
            alienTitle.SetActive(alienTextOn);
        foreach(TMP_Text text in glitchTexts)
        {
            text.gameObject.SetActive(!alienTextOn);
        }
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