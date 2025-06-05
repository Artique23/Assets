using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public Image slotImage;
    public bool isWinningSlot = false;
    public string expectedSignName = ""; // The name of the sign this slot expects (set by WinningInventory)
    
    private void Awake()
    {
        if (slotImage == null)
            slotImage = GetComponent<Image>();
            
        if (slotImage != null)
            slotImage.raycastTarget = true;
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;
        
        DraggableItem draggableItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        if (draggableItem == null) return;
        
        // If this is a winning slot, check if the sign matches what's expected
        if (isWinningSlot && !string.IsNullOrEmpty(expectedSignName))
        {
            string signName = draggableItem.gameObject.name;
            if (signName.EndsWith("(Clone)"))
                signName = signName.Substring(0, signName.Length - 7);
                
            // If wrong sign, don't accept it
            if (signName != expectedSignName)
            {
                Debug.Log($"Wrong sign! Expected {expectedSignName}, got {signName}");
                return;
            }
        }
        
        // Accept the item
        draggableItem.parentAfterDrag = transform;
    }
}