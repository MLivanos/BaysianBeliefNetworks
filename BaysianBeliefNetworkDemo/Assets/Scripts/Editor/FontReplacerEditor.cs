using UnityEngine;
using UnityEditor;
using TMPro;

public class FontReplacerEditor : EditorWindow
{
    private TMP_FontAsset newTMPFont;
    private Font newUnityFont;

    [MenuItem("Tools/Font Replacer")]
    public static void ShowWindow()
    {
        GetWindow<FontReplacerEditor>("Font Replacer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Replace Fonts in Scene", EditorStyles.boldLabel);

        newTMPFont = (TMP_FontAsset)EditorGUILayout.ObjectField("New TMP Font", newTMPFont, typeof(TMP_FontAsset), false);
        newUnityFont = (Font)EditorGUILayout.ObjectField("New Unity Font", newUnityFont, typeof(Font), false);

        if (GUILayout.Button("Replace Fonts"))
        {
            ReplaceFontsInScene();
        }
    }

    private void ReplaceFontsInScene()
    {
        if (newTMPFont == null && newUnityFont == null)
        {
            Debug.LogWarning("Please assign a font to replace.");
            return;
        }

        Undo.RegisterCompleteObjectUndo(FindObjectsOfType<TMP_Text>(), "Replace TMP Fonts");
        Undo.RegisterCompleteObjectUndo(FindObjectsOfType<UnityEngine.UI.Text>(), "Replace Unity Text Fonts");

        // Replace TextMeshPro fonts
        TMP_Text[] textMeshProComponents = FindObjectsOfType<TMP_Text>();
        foreach (TMP_Text textComponent in textMeshProComponents)
        {
            if (newTMPFont != null)
            {
                textComponent.font = newTMPFont;
                EditorUtility.SetDirty(textComponent); // Marks the object as dirty so changes are saved
            }
        }

        // Replace Unity Text fonts
        UnityEngine.UI.Text[] unityTextComponents = FindObjectsOfType<UnityEngine.UI.Text>();
        foreach (UnityEngine.UI.Text textComponent in unityTextComponents)
        {
            if (newUnityFont != null)
            {
                textComponent.font = newUnityFont;
                EditorUtility.SetDirty(textComponent); // Marks the object as dirty so changes are saved
            }
        }

        Debug.Log("Font replacement complete!");
    }
}
