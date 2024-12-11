using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadableImage : FadableElement
{
    private Image _image;
    private RawImage _rawImage;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _rawImage = GetComponent<RawImage>();
    }

    public override void SetAlpha(float alpha)
    {
        if (_image != null)
        {
            var color = _image.color;
            color.a = alpha;
            _image.color = color;
        }
        else if (_rawImage != null)
        {
            var color = _rawImage.color;
            color.a = alpha;
            _rawImage.color = color;
        }
    }
}