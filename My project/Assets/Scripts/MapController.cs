using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject pinPrefab;
    [SerializeField] private RectTransform mapRectTransform; // Ссылка на RectTransform карты
    private PinFactory _pinFactory;

    private List<Vector2> pinPositions = new List<Vector2>();

    private void Awake()
    {
        _pinFactory = new PinFactory(pinPrefab);
    }

    private void Start()
    {
        // Загружаем данные при старте
        GameData gameData = GameData.LoadData();
        if (gameData != null && gameData.PinPositions.Count > 0)
        {
            StartCoroutine(AnimatePins(gameData.PinPositions));
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clickPosition = GetClickPosition();
            if (clickPosition != Vector2.zero)
            {
                AddPin(clickPosition, false);
                pinPositions.Add(clickPosition);
                SavePinsPosition(pinPositions);
            }
        }
    }

    private IEnumerator AnimatePins(List<Vector2> positions)
    {
        foreach (var position in positions)
        {
            AddPin(position, true);
            yield return new WaitForSeconds(0.5f); // задержка между анимациями
        }
    }

    private Vector2 GetClickPosition()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(mapRectTransform, Input.mousePosition, null, out Vector2 localPoint);
        if (RectTransformUtility.RectangleContainsScreenPoint(mapRectTransform, Input.mousePosition))
        {
            return localPoint;
        }
        return Vector2.zero;
    }

    private void AddPin(Vector2 position, bool isFromLoad)
    {
        Pin pin = _pinFactory.CreatePin(position, mapRectTransform);
        pin.Show(isFromLoad);
    }

    private void SavePinsPosition(List<Vector2> positions)
    {
        GameData data = GameData.LoadData();
        data.PinPositions.AddRange(positions);
        GameData.SaveData(data);
    }
}
