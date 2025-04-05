using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGame : MonoBehaviour
{
    [SerializeField] private string[] keysToReset;

    public void GoToMainMenu()
    {
        ResetPlayerPrefs();
        DestroyAllDDOL();
        SceneManager.LoadScene("TitleScreen");
    }

    private void ResetPlayerPrefs()
    {
        foreach (string key in keysToReset)
        {
            PlayerPrefs.DeleteKey(key);
        }
        PlayerPrefs.Save();
    }

    private void DestroyAllDDOL()
    {
        GameObject[] rootObjects = FindObjectsOfType<GameObject>(true);
        foreach (GameObject obj in rootObjects)
        {
            if (obj.scene.buildIndex == -1) Destroy(obj);
        }
        Debug.Log("Deleted DDOL");
    }
}