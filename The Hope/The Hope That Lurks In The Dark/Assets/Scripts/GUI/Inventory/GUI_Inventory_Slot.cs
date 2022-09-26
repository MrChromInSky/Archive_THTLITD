using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class GUI_Inventory_Slot : MonoBehaviour
{
    #region Assignments

    #endregion Assignments

    #region Variables

    [Header("Slot Image")]
    [SerializeField] Image GUI_Inventory_Slot_Image;

    [Header("Slot Background")]
    [SerializeField] Image GUI_Inventory_Slot_Background;

    [Header("Default Sprite")]
    [SerializeField] Sprite GUI_Inventory_Slot_DefaultSprite;


    #endregion Variables

    void Awake()
    {

    }

    public void GUI_Slot_Update(Sprite newSprite)
    {

        //Update Actual Sprite//
        if (newSprite == null)
        {
            GUI_Inventory_Slot_Image.sprite = GUI_Inventory_Slot_DefaultSprite;
        }
        else
        {
            GUI_Inventory_Slot_Image.sprite = newSprite;
        }

    }


}
