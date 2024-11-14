using UnityEngine;

public class PinFactory
{
    private readonly GameObject _pinPrefab;

    public PinFactory(GameObject pinPrefab)
    {
        _pinPrefab = pinPrefab;
    }

    public Pin CreatePin(Vector2 position, RectTransform parent)
    {
        GameObject pinObject = Object.Instantiate(_pinPrefab, parent);
        pinObject.GetComponent<RectTransform>().anchoredPosition = position;
        return pinObject.GetComponent<Pin>();
    }
}