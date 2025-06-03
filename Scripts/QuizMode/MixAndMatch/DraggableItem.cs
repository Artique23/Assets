using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image; 
    [HideInInspector] public Transform parentAfterDrag;
    
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
            
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();
            
        if (image == null)
            image = GetComponent<Image>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {

        parentAfterDrag = transform.parent;
        
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
        
        Debug.Log("Dragging started for: " + gameObject.name);
    }

    public void OnDrag(PointerEventData eventData)
    {

        if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            rectTransform.position = eventData.position;
        }
        else
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        transform.SetParent(parentAfterDrag);
        
        Debug.Log("Dragging ended for: " + gameObject.name);
    }
}