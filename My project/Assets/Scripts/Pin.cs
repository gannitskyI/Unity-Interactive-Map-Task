using UnityEngine;
using DG.Tweening;

public class Pin : MonoBehaviour
{
    private const float AnimationDuration = 0.5f;
    private const float StartScale = 0.1f;

    public void Show(bool isFromLoad)
    {
        if (isFromLoad)
        {
            transform.localScale = Vector3.one * StartScale;
            transform.DOScale(Vector3.one, AnimationDuration).SetEase(Ease.OutBounce);
        }
        else
        {
            transform.localScale = Vector3.one * StartScale;
            transform.DOScale(Vector3.one, AnimationDuration).SetEase(Ease.OutBounce);
        }
    }
}
