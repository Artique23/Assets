using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image slotImage;
    public bool isWinningSlot = false;  // Flag to indicate if this is a winning slot
    
    private WinningInventory winningInventory;
    
    private void Awake()
    {
        if (slotImage == null)
            slotImage = GetComponent<Image>();
            
        if (slotImage != null)
            slotImage.raycastTarget = true;
            
        // Find the winning inventory manager
        winningInventory = FindObjectOfType<WinningInventory>();
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && transform.childCount == 0)
        {
            DraggableItem draggableItem = eventData.pointerDrag.GetComponent<DraggableItem>();
            
            if (draggableItem != null)
            {
                // If this is a winning slot, check if the sign is correct
                if (isWinningSlot && winningInventory != null)
                {
                    int slotIndex = winningInventory.GetSlotIndex(this);
                    
                    // If the sign doesn't match what's required for this slot, return it to its original parent
                    if (!winningInventory.CanPlaceSignInSlot(draggableItem.gameObject, slotIndex))
                    {
                        // Wrong sign for this slot - return it
                        Debug.Log("Wrong sign for slot: " + gameObject.name);
                        return;
                    }
                }
                
                // Correct sign or regular inventory slot - accept the item
                draggableItem.parentAfterDrag = transform;
                
                RectTransform itemRect = draggableItem.GetComponent<RectTransform>();
                if (itemRect != null)
                {
                    itemRect.anchoredPosition = Vector2.zero;
                }
                
                Debug.Log("Item dropped into slot: " + gameObject.name);
            }
        }
    }
}