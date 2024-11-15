using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject pinPrefab;
    [SerializeField] private RectTransform mapRectTransform;
    [SerializeField] private PinInfoPanel pinInfoPanel;

    private PinFactory _pinFactory;
    private List<Pin> _pins = new List<Pin>();
    private Pin _activePin;
    private Pin _draggedPin;
    private float _dragStartTime;
    private bool _isDragging;

    private const float DragThreshold = 0.3f;

    public static MapController Instance { get; private set; }

    private void Awake()
    {
        _pinFactory = new PinFactory(pinPrefab);
        Instance = this;
    }

    private void Start()
    {
        var gameData = GameData.LoadData();
        if (gameData?.Pins.Count > 0)
        {
            StartCoroutine(AnimatePins(gameData.Pins));
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsPanelActive() && _draggedPin == null)
        {
            Vector2 clickPosition = GetClickPosition();
            if (clickPosition != Vector2.zero && !IsPointerOverPin())
            {
                var pinInfo = new PinInfo(clickPosition, "New Pin", "Description");
                AddPin(pinInfo, false);
                pinInfoPanel.Show(_pins[^1]);
            }
        }

        HandlePinDragging();
    }

    private void HandlePinDragging()
    {
        switch (_draggedPin)
        {
            case null when Input.GetMouseButtonDown(0):
                _draggedPin = _pins.FirstOrDefault(pin =>
                    RectTransformUtility.RectangleContainsScreenPoint(pin.GetComponent<RectTransform>(), Input.mousePosition));
                if (_draggedPin != null)
                {
                    _dragStartTime = Time.time;
                    _isDragging = false;
                }
                break;

            case { } draggedPin when Input.GetMouseButton(0):
                if (!_isDragging && Time.time - _dragStartTime >= DragThreshold)
                {
                    _isDragging = true;
                }

                if (_isDragging)
                {
                    Vector2 dragPosition = GetClickPosition();
                    if (dragPosition != Vector2.zero)
                    {
                        draggedPin.transform.localPosition = dragPosition;
                    }
                }
                break;

            case { } draggedPin when Input.GetMouseButtonUp(0):
                if (_isDragging)
                {
                    Vector2 finalPosition = GetClickPosition();
                    if (finalPosition != Vector2.zero)
                    {
                        draggedPin.Info.Position = finalPosition;
                        SavePinData();
                    }
                }

                _draggedPin = null;
                break;
        }
    }

    private void SavePinData()
    {
        var gameData = GameData.LoadData();
        gameData.Pins = _pins.Select(pin => pin.Info).ToList();
        GameData.SaveChanges(); 
    }

    private bool IsPanelActive()
    {
        return pinInfoPanel.IsActive() || (_activePin != null && _activePin.IsNestedPanelActive());
    }

    private IEnumerator AnimatePins(List<PinInfo> pinInfos)
    {
        foreach (var info in pinInfos)
        {
            AddPin(info, true);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private Vector2 GetClickPosition()
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mapRectTransform, Input.mousePosition, null, out var localPoint) &&
            RectTransformUtility.RectangleContainsScreenPoint(mapRectTransform, Input.mousePosition))
        {
            return localPoint;
        }

        return Vector2.zero;
    }

    private void AddPin(PinInfo info, bool isFromLoad)
    {
        var pin = _pinFactory.CreatePin(info.Position, mapRectTransform);
        pin.Initialize(info);
        _pins.Add(pin);
        pin.Show(isFromLoad);

        if (!isFromLoad)
        {
            SavePinData();
        }
    }

    private bool IsPointerOverPin()
    {
        return _pins.Any(pin =>
            RectTransformUtility.RectangleContainsScreenPoint(pin.GetComponent<RectTransform>(), Input.mousePosition));
    }

    public void ShowNestedPanel(Pin pin)
    {
        if (_activePin != null && _activePin != pin)
        {
            _activePin.HideNestedPanel();
        }

        _activePin = pin;
        _activePin.ShowNestedPanel();
    }

    public void ShowPinInfoPanel(Pin pin)
    {
        pin.HideNestedPanel();
        pinInfoPanel.Show(pin);
    }

    public GameData GetGameData()
    {
        return new GameData
        {
            Pins = _pins.Select(pin => pin.Info).ToList()
        };
    }

    public void DeletePin(Pin pin)
    {
        if (_pins.Contains(pin))
        {
            _pins.Remove(pin);
        }

        var gameData = GameData.LoadData();
        var pinInfoToRemove = gameData?.Pins.FirstOrDefault(p => p.Id == pin.Info.Id);
        if (pinInfoToRemove != null)
        {
            gameData.Pins.Remove(pinInfoToRemove);
            GameData.SaveChanges();
        }

        Destroy(pin.gameObject);

        if (_activePin == pin)
        {
            _activePin = null;
        }
    }

    public void ClearActivePin(Pin pin)
    {
        if (_activePin == pin)
        {
            _activePin = null;
        }
    }
}
