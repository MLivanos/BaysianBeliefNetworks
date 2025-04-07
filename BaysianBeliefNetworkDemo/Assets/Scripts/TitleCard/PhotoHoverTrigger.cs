using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

public class PhotoHoverTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private VideoClip clip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (VideoClipManager.Instance != null)
        {
            VideoClipManager.Instance.PlayClipAt(transform, clip);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer Exiting");
        if (VideoClipManager.Instance != null)
        {
            //VideoClipManager.Instance.StopClip();
        }
    }
}
