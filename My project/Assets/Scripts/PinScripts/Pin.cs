using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Pin : MonoBehaviour
{
    [SerializeField] private GameObject nestedPanel;
    [SerializeField] private TextMeshProUGUI nestedText;
    [SerializeField] private Button detailsButton;
    [SerializeField] private Button deleteButton;

    private const float AnimationDuration = 0.5f;
    private const float StartScale = 0.1f;

    public PinInfo Info { get; private set; }

    private void Awake()
    {
        ConfigureButton(detailsButton, OnDetailsButtonClicked);
        ConfigureButton(deleteButton, OnDeleteButtonClicked);
        ConfigureButton(GetComponent<Button>(), OnMainPanelClicked);
    }

    public void Initialize(PinInfo info)
    {
        Info = info;
        UpdateUI();
    }

    public void Show(bool isFromLoad)
    {
        transform.localScale = Vector3.one * StartScale;
        transform.DOScale(Vector3.one, AnimationDuration).SetEase(Ease.OutBounce);
    }

    private void UpdateUI()
    {
        if (nestedText != null)
        {
            nestedText.text = Info.Title;
        }
    }

    private void OnMainPanelClicked()
    {
        bool isPanelActive = nestedPanel.activeSelf;
        nestedPanel.SetActive(!isPanelActive);

        if (isPanelActive)
        {
            MapController.Instance.ClearActivePin(this);
        }
        else
        {
            UpdateUI();  
            MapController.Instance.ShowNestedPanel(this);
        }
    }

    private void OnDetailsButtonClicked()
    {
        nestedPanel.SetActive(false);
        MapController.Instance.ShowPinInfoPanel(this);
    }

    private void OnDeleteButtonClicked()
    {
        MapController.Instance.DeletePin(this);
    }

    public void HideNestedPanel()
    {
        SetNestedPanelState(false);
    }

    public void ShowNestedPanel()
    {
        UpdateUI();  
        SetNestedPanelState(true);
    }

    public bool IsNestedPanelActive()
    {
        return nestedPanel.activeSelf;
    }

    private void ConfigureButton(Button button, UnityEngine.Events.UnityAction action)
    {
        button?.onClick.AddListener(action);
    }

    private void SetNestedPanelState(bool isActive)
    {
        nestedPanel.SetActive(isActive);
    }
}
