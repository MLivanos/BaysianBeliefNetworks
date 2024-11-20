using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadableTextMeshPro : MonoBehaviour, IFadable
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    public void SetAlpha(float alpha)
    {
        if (_text != null)
        {
            var color = _text.color;
            color.a = alpha;
            _text.color = color;
        }
    }
}