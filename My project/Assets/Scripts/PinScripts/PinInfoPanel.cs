using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SFB;

public class PinInfoPanel : MonoBehaviour
{
    [SerializeField] private TMP_InputField titleInput;
    [SerializeField] private TMP_InputField descriptionInput;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button hideButton;
    [SerializeField] private Button uploadPhotoButton;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image photoPreview;

    private string _photoPath;
    private Pin _currentPin;

    private void Awake()
    {
        ConfigureButton(saveButton, OnSaveButtonClicked);
        ConfigureButton(hideButton, Hide);
        ConfigureButton(uploadPhotoButton, OnUploadPhotoButtonClicked);
    }

    public void Show(Pin pin)
    {
        _currentPin = pin;
        titleInput.text = pin.Info.Title;
        descriptionInput.text = pin.Info.Description;
        _photoPath = pin.Info.PhotoPath;
        UpdatePhotoPreview();
        SetCanvasGroupState(true);
    }

    public void Hide()
    {
        SetCanvasGroupState(false);
    }

    private void OnSaveButtonClicked()
    {
        if (_currentPin == null) return;

        _currentPin.Info.Title = titleInput.text;
        _currentPin.Info.Description = descriptionInput.text;
        _currentPin.Info.PhotoPath = _photoPath;
        GameData.SaveData(MapController.Instance.GetGameData());
        Hide();
    }

    private void OnUploadPhotoButtonClicked()
    {
        _photoPath = OpenFilePicker();
        if (!string.IsNullOrEmpty(_photoPath))
        {
            UpdatePhotoPreview();
            Debug.Log($"Selected photo path: {_photoPath}");
        }
    }

    private string OpenFilePicker()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Select an image", "", "png", false);
        return paths.Length > 0 ? paths[0] : null;
    }

    private void UpdatePhotoPreview()
    {
        if (string.IsNullOrEmpty(_photoPath)) return;

        var texture = LoadTextureFromFile(_photoPath);
        if (texture == null) return;

        photoPreview.sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );
    }

    private Texture2D LoadTextureFromFile(string path)
    {
        if (!System.IO.File.Exists(path))
        {
            Debug.LogError($"File not found: {path}");
            return null;
        }

        var fileData = System.IO.File.ReadAllBytes(path);
        var texture = new Texture2D(2, 2);
        return texture.LoadImage(fileData) ? texture : null;
    }

    public bool IsActive()
    {
        return canvasGroup.alpha > 0 && canvasGroup.interactable;
    }

    private void ConfigureButton(Button button, UnityEngine.Events.UnityAction action)
    {
        if (button != null)
        {
            button.onClick.AddListener(action);
        }
    }

    private void SetCanvasGroupState(bool isActive)
    {
        canvasGroup.alpha = isActive ? 1 : 0;
        canvasGroup.interactable = isActive;
        canvasGroup.blocksRaycasts = isActive;
    }
}
