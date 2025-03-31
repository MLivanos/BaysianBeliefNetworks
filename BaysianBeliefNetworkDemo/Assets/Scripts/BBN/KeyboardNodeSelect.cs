using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardNodeSelect : MonoBehaviour, IXRaycastTargetScript
{
    [SerializeField] private Toggle queryToggle;
    [SerializeField] private Toggle evidenceToggle;

    private Coroutine keyWatcher;

    private void Awake()
    {
        if (transform.parent != null)
        {
            Transform qChild = transform.parent.Find("QueryCheckbox");
            Transform eChild = transform.parent.Find("EvidenceCheckbox");

            if (qChild != null && queryToggle == null) queryToggle = qChild.GetComponent<Toggle>();
            if (eChild != null && evidenceToggle == null) evidenceToggle = eChild.GetComponent<Toggle>();
        }
    }

    public void OnDeepPointerEnter()
    {
        if (keyWatcher != null) StopCoroutine(keyWatcher);
        keyWatcher = StartCoroutine(WatchKeys());
    }

    public void OnDeepPointerExit()
    {
        if (keyWatcher != null)
        {
            StopCoroutine(keyWatcher);
            keyWatcher = null;
        }
    }

    private IEnumerator WatchKeys()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Q) && queryToggle != null)
            {
                queryToggle.isOn = !queryToggle.isOn;
            }

            if (Input.GetKeyDown(KeyCode.E) && evidenceToggle != null)
            {
                evidenceToggle.isOn = !evidenceToggle.isOn;
            }

            yield return null;
        }
    }
}