using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [SerializeField] private RectTransform cursorTransform;
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image cursorImage;
    [SerializeField] private Sprite defaultCursor;
    [SerializeField] private Vector2 offset = new Vector2(10, -10);
    [SerializeField] private bool lockToGameView = true;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (cursorImage != null && defaultCursor != null)
        {
            cursorImage.sprite = defaultCursor;
        }

        Cursor.visible = false;
    }

    private void Update()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out pos
        );

        cursorTransform.anchoredPosition = pos + offset;
    }
}