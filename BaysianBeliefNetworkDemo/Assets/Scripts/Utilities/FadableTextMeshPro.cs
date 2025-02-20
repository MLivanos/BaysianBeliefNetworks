using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadableTextMeshPro : FadableElement
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    public override void SetAlpha(float alpha)
    {
        if (_text != null)
        {
            var color = _text.color;
            color.a = alpha;
            _text.color = color;
        }
    }

    public void SetText(string newText)
    {
        _text.text = newText;
    }
}