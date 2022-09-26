using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory_Controller : MonoBehaviour
{
    #region Assignments
    Player_Main playerMain;
    Player_Hands_Controller handsController;
    GUI_Inventory_Controller GUIInventoryController;

    private void Awake()
    {
        playerMain = GetComponentInParent<Player_Main>();
        handsController = GameObject.FindGameObjectWithTag("Player_Hands").GetComponent<Player_Hands_Controller>();
        GUIInventoryController = GameObject.FindGameObjectWithTag("GUI_Controller").GetComponent<GUI_Inventory_Controller>();
    }
    #endregion Assignemnts

    #region Variables
    [Header("Item In Hand")]
    public int inventory_ItemInHand;

    [Header("Inventory Items")]
    public GameObject[] inventory_Items = new GameObject[9];

    [Header("Primary Item Quantity")]
    public int inventory_Items_Primary_Amount = 4;

    [Header("Object Containers")]
    public Transform inventory_Items_Container_Primary;
    public Transform inventory_Items_Container_Normal;

    [Header("Later Masks")]
    [SerializeField] LayerMask inventory_Items_Mask;
    [SerializeField] LayerMask inventory_Items_RemoveMask;

    [Header("Remove Object Position")]
    [SerializeField] Transform inventory_Items_Remove_Position;
    #endregion Variables

    #region Add Item
    public void Inventory_AddItem(GameObject itemObject, bool isPrimary)
    {
        if (isPrimary)
        {
            Inventory_AddItem_Primary(itemObject);
        }
        else
        {
            Inventory_AddItem_Normal(itemObject);
        }
    }

    void Inventory_AddItem_Primary(GameObject itemObject)
    {
        //If Item Was Added//
        bool item_WasAdded = false;
        //Count Free Slots//
        int items_Primary_FreeSlots = 0;

        for (int i = 0; i < inventory_Items_Primary_Amount; i++)
        {
            if (inventory_Items[i] == null && !item_WasAdded)
            {
                //Add item to array//
                inventory_Items[i] = itemObject;

                //Setup item to being inventory holded//
                Inventory_AddItem_Setup(itemObject, true);

                //Take new item to hand//
                Inventory_ItemInHand_Change(i);

                //Update Inventory Slot//
                GUIInventoryController.GUI_Inventory_Slot_Update(i, itemObject.GetComponent<Interactable_Item>().interactable_Item_GUI_Icon);

                //To add it once//
                item_WasAdded = true;
            }
            else if (inventory_Items[i] == null)//When is null//
            {
                //Count new free slot//
                items_Primary_FreeSlots++;
            }

        }

        //Check if inventory is full//
        if (items_Primary_FreeSlots == 0)
        {
            playerMain.inventory_Primary_IsFull = true;
        }
        else
        {
            playerMain.inventory_Primary_IsFull = false;
        }


    }

    void Inventory_AddItem_Normal(GameObject itemObject)
    {
        //If Item Was Added//
        bool item_WasAdded = false;
        //Count Free Slots//
        int items_Normal_FreeSlots = 0;

        for (int i = inventory_Items_Primary_Amount; i < inventory_Items.Length; i++)
        {
            if (inventory_Items[i] == null && !item_WasAdded)
            {
                //Add item to array//
                inventory_Items[i] = itemObject;

                //Setup item to being inventory holded//
                Inventory_AddItem_Setup(itemObject, false);

                //Take new item to hand//
                Inventory_ItemInHand_Change(i);

                //Update Inventory Slot//
                GUIInventoryController.GUI_Inventory_Slot_Update(i, itemObject.GetComponent<Interactable_Item>().interactable_Item_GUI_Icon);

                //To add it once//
                item_WasAdded = true;
            }
            else if (inventory_Items[i] == null)//When is null//
            {
                //Count new free slot//
                items_Normal_FreeSlots++;
            }

        }

        //Check if inventory is full//
        if (items_Normal_FreeSlots == 0)
        {
            playerMain.inventory_Normal_IsFull = true;
        }
        else
        {
            playerMain.inventory_Normal_IsFull = false;
        }


    }

    public void Inventory_AddItem_Setup(GameObject itemObject, bool isPrimary)
    {
        //Set Rigidbody to Kinematic//
        if (itemObject.GetComponent<Rigidbody>())
        {
            itemObject.GetComponent<Rigidbody>().isKinematic = true;
        }

        //Turn Off Colisions//
        if (itemObject.GetComponent<Collider>())
        {
            itemObject.GetComponent<Collider>().enabled = false;
        }


        //Set new Parent//
        if (isPrimary)
        {
            itemObject.transform.parent = inventory_Items_Container_Primary;
        }
        else
        {
            itemObject.transform.parent = inventory_Items_Container_Normal;
        }

        //Set layer//
        Inventory_AddItem_Setup_Layers(itemObject);

        //Set new Scale//
        itemObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        //Reset Position//
        itemObject.transform.localPosition = Vector3.zero;

        //Reset Rotation//
        itemObject.transform.localRotation = Quaternion.identity;

        //Set invisible//
        itemObject.SetActive(false);

        //Reset Interactivity//
        itemObject.GetComponent<Interactable_Item_Interactions>().interactable_Item_IsInHands = false;

    }

    void Inventory_AddItem_Setup_Layers(GameObject itemObject)
    {
        itemObject.layer = 7;

        foreach (Transform child in itemObject.transform)
        {
            child.gameObject.layer = 7;
            Inventory_AddItem_Setup_Layers(child.gameObject);
        }
    }


    #endregion Add Item

    #region Remove Item
    // Throw away item from inventory//
    public void Inventory_RemoveItem(int itemSlotNumber)
    {
        //Update Description//

        //Take Controller//
        GameObject removeObject_Item = inventory_Items[itemSlotNumber];
        Interactable_Item removeObject_Item_Controller = removeObject_Item.GetComponent<Interactable_Item>();

        //Delete from slots//
        inventory_Items[itemSlotNumber] = null;

        //Setup//
        Inventory_RemoveItem_Setup(removeObject_Item, removeObject_Item_Controller.interactable_Item_ParentTransform);

        //Count Free spaces//
        Inventory_CountFreeSpaces();
    }

    void Inventory_RemoveItem_Setup(GameObject itemObject, Transform parentObject)
    {
        //Set visible//
        itemObject.SetActive(true);

        //Set Rigidbody to Kinematic//
        if (itemObject.GetComponent<Rigidbody>())
        {
            itemObject.GetComponent<Rigidbody>().isKinematic = false;
        }

        //Turn On Colisions//
        if (itemObject.GetComponent<Collider>())
        {
            itemObject.GetComponent<Collider>().enabled = true;
        }

        //Set layer to normal//
        Inventory_RemoveItem_Setup_Layers(itemObject);

        //reset Scale//
        itemObject.transform.localScale = new Vector3(1, 1, 1);

        //Reset Parent//
        itemObject.transform.parent = parentObject;

        //Set Remove Transform Position//
        itemObject.transform.localPosition = inventory_Items_Remove_Position.position;

        //Reset Rotation//
        //itemObject.transform.localRotation = Quaternion.identity;

        //Reset Interactivity//
        itemObject.GetComponent<Interactable_Item_Interactions>().interactable_Item_IsInHands = false;

    }

    void Inventory_RemoveItem_Setup_Layers(GameObject itemObject)
    {
        itemObject.layer = 6;

        foreach (Transform child in itemObject.transform)
        {
            child.gameObject.layer = 6;
            Inventory_AddItem_Setup_Layers(child.gameObject);
        }
    }


    #endregion Remove Item

    void Inventory_CountFreeSpaces()
    {
        int freeSlots_Primary = 0;
        int freeSlots_Normal = 0;

        //Primary//
        for (int i = 0; i < inventory_Items_Primary_Amount; i++)
        {
            if (inventory_Items[i] == null)
            {
                freeSlots_Primary++;
            }
        }

        //Normal//
        for (int i = inventory_Items_Primary_Amount; i < inventory_Items.Length; i++)
        {
            if (inventory_Items[i] == null)
            {
                freeSlots_Normal++;
            }
        }

        //Update Informations//
        if (freeSlots_Primary == 0)
        {
            playerMain.inventory_Primary_IsFull = true;
        }
        else
        {
            playerMain.inventory_Primary_IsFull = false;
        }


        if (freeSlots_Normal == 0)
        {
            playerMain.inventory_Normal_IsFull = true;
        }
        else
        {
            playerMain.inventory_Normal_IsFull = false;
        }
    }


    public void Inventory_ItemInHand_Change(int item_SlotNumber)
    {
        //Update actual item in hand display//
        inventory_ItemInHand = item_SlotNumber;

        handsController.Hands_Item_Change(inventory_Items[inventory_ItemInHand]);
    }

}
