using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Interactable_Item : Interactable_Main
{
    #region Assignments
    Player_Inventory_Controller inventoryController;

    #endregion Assignments

    #region Variables

    #region Item Parameters

    public enum interactable_Item_Types { Default, Big };
    [Header("Item Parameters", order = 0)]
    [Header("Item Type", order = 1)]
    public interactable_Item_Types interactable_Item_Type;

    [Header("Item Name")]
    public string interactable_Item_Name;

    [Header("Item ID")]
    public int interactable_Item_ID = 0;

    [Header("Is It Primary Item?")]
    public bool interactable_Item_IsPrimary = false;

    [Header("Item Icon")]
    public Sprite interactable_Item_GUI_Icon = null;

    [Header("Position in Hands")]
    public int interactable_Item_InHands_Position = 0;

    [Header("Parent Transform")]
    public Transform interactable_Item_ParentTransform;

    #endregion Item Parameters


    #endregion Variables

    public override void Interact()
    {
        Inventory_AddItem();
    }

    void Inventory_AddItem()
    {
        if (interactable_Item_ParentTransform == null)
        {
            interactable_Item_ParentTransform = transform.parent;
        }

        if (inventoryController == null)
        {
            inventoryController = GameObject.FindGameObjectWithTag("Player_Inventory").GetComponent<Player_Inventory_Controller>();
        }

        inventoryController.Inventory_AddItem(gameObject, interactable_Item_IsPrimary);
    }


}

public abstract class Interactable_Item_Interactions : Interactable_Item
{
    #region Assignments
    GameControls gameControls;

    void Awake()
    {
        gameControls = new GameControls();

        gameControls.Player_Items.Item_Interaction_Primary.started += ctx => Item_Interaction_Primary();
        gameControls.Player_Items.Item_Interaction_Secondary.started += ctx => Item_Interaction_Secondary();


    }
    #endregion Assignments

    #region Variables
    [Header("Item Is In Hands")]
    public bool interactable_Item_IsInHands;
    #endregion Variables

    #region Abstract Functions
    //Primary Interaction
    public abstract void Item_Interaction_Primary();

    //Secondary Interaction//
    public abstract void Item_Interaction_Secondary();
    #endregion Abstarct Functions

    #region OnEnable, OnDisable
    void OnEnable()
    {
        gameControls.Enable();
    }

    void OnDisable()
    {
        gameControls.Disable();
    }

    #endregion
}

