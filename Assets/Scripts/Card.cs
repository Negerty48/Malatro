using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool IsSelected { get; private set; }
    private float selectionOffset = 40f;
    [SerializeField] private RectTransform faceTransform;
    private RectTransform rect;
    private LayoutElement layoutElement;
    private CanvasGroup canvasGroup;
    private Transform originalParent;

    void Awake()
    {        
        rect = GetComponent<RectTransform>();
        layoutElement = GetComponent<LayoutElement>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalParent = transform.parent;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        var manager = FindObjectOfType<CardsManager>();

        if (!IsSelected)
        {
            if (manager.SelectedCards.Count >= 5) return;
            IsSelected = true;
            manager.SelectedCards.Add(this.gameObject);
            faceTransform.DOAnchorPosY(selectionOffset, 0.2f).SetEase(Ease.OutBack);
        }
        else
        {
            Deselect();
        }
    }

    public void Deselect()
    {
        IsSelected = false;
        var manager = FindObjectOfType<CardsManager>();
        manager.SelectedCards.Remove(this.gameObject);
        faceTransform.DOAnchorPos(new Vector2(0f, 0f), 0.2f).SetEase(Ease.OutBack);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = false;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        Transform parent = transform.parent;
        int newIndex = parent.childCount;

        for (int i = 0; i < parent.childCount; i++)
        {
            Transform sibling = parent.GetChild(i);
            if (sibling == transform) continue;

            float siblingMidX = sibling.position.x + sibling.GetComponent<RectTransform>().rect.width * 0.5f;

            if (transform.position.x < siblingMidX)
            {
                newIndex = i;
                break;
            }
        }

        transform.SetSiblingIndex(newIndex);

        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)parent);
    }
}