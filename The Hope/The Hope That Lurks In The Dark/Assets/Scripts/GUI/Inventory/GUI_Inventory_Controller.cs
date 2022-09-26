using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class GUI_Inventory_Controller : MonoBehaviour
{
    #region Assignments
    Player_Main playerMain;
    GameControls gameControls;
    Player_Inventory_Controller playerInventory;

    void Awake()
    {
        playerMain = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Main>();
        gameControls = new GameControls();
        playerInventory = GameObject.FindGameObjectWithTag("Player_Inventory").GetComponent<Player_Inventory_Controller>();


        #region Controls
        //Inventory//
        gameControls.Player_UI.Inventory_Open.started += ctx => GUI_Inventory_Input(true);
        gameControls.Player_UI.Inventory_Open.canceled += ctx => GUI_Inventory_Input(false);


        #endregion Controls
    }
    #endregion Assignments

    #region Variables
    [Header("UI Controls", order = 0)]
    [Header("Mouse Position", order = 1)]
    [SerializeField] Vector2 GUI_Inventory_Input_CursorPosition;
    Vector2 screenSize_Half;

    [Header("Wheel Sizes, in percent")]
    [SerializeField] float GUI_Inventory_Wheel_Size_Percent;
    float GUI_Inventory_Wheel_Size;
    [SerializeField] float GUI_Inventory_Description_Size_Percent;
    float GUI_Inventory_Description_Size;

    [Header("Angle to cursor position")]
    [SerializeField] float GUI_Inventory_Cursor_Angle;

    [Header("Selected Slot")]
    [SerializeField] bool GUI_Inventory_Slots_Selected;
    [SerializeField] int GUI_Inventory_Slots_Selected_Number;

    [Header("Selected Slot Highlight Settings")]
    [SerializeField] Color GUI_Inventory_Slots_Selected_Highlight_Color;

    [Header("Inventory Objects", order = 0)]
    [Header("General", order = 1)]
    [SerializeField] GameObject GUI_Inventory_Object;

    [Header("Description")]
    [SerializeField] GameObject GUI_Inventory_Description_Object;
    [SerializeField] TextMeshProUGUI GUI_Inventory_Description_Text;
    [SerializeField] Image GUI_Inventory_Description_Image;
    [SerializeField] Sprite GUI_Inventory_Description_Image_DefaultSprite;

    [Header("Slots")]
    [SerializeField] GameObject[] GUI_Inventory_Slots;


    public float test_float;

    #endregion Variables

    void Update()
    {
        //When Inventory is open, control inventory//
        if (playerMain.playerGUIState == Player_Main.PlayerGUIStates.Inventory)
        {
            //Control slot wheel//
            GUI_Inventory_Wheel_Controller();


        }
    }

    #region Wheel & Slot

    void GUI_Inventory_Wheel_Controller()
    {
        //Update Cursor Position//
        GUI_Inventory_Input_Cursor();


        if (Vector2.Distance(Vector2.zero, GUI_Inventory_Input_CursorPosition) > GUI_Inventory_Description_Size && Vector2.Distance(Vector2.zero, GUI_Inventory_Input_CursorPosition) < GUI_Inventory_Wheel_Size)
        {
            //When is selected start //
            GUI_Inventory_Slots_Selected = true;

            GUI_Inventory_Design_Slots_Controller();
        }
        else
        {
            if (GUI_Inventory_Slots_Selected)
            {
                GUI_Inventory_Slot_Update(1, true);

                //Reset Description//
                GUI_Inventory_Description_Update(null, null);
            }

            GUI_Inventory_Slots_Selected = false;
        }
    }

    //Calculate degrees, and choose whitch slot is selected//
    void GUI_Inventory_Design_Slots_Controller()
    {
        //Slot last frame//
        int actualSlot_LastFrame = 10;

        //Calculate angle//
        GUI_Inventory_Cursor_Angle = Mathf.Atan2(GUI_Inventory_Input_CursorPosition.y, -GUI_Inventory_Input_CursorPosition.x) * Mathf.Rad2Deg;

        //Slot controller//
        if (GUI_Inventory_Cursor_Angle >= 0)
        {
            //Primary Slots//
            if (GUI_Inventory_Cursor_Angle > 0 && GUI_Inventory_Cursor_Angle < 45)
            {
                //Give info whitch one is selected currently//
                GUI_Inventory_Slots_Selected_Number = 0;
            }
            else if (GUI_Inventory_Cursor_Angle > 45 && GUI_Inventory_Cursor_Angle < 90)
            {
                //Give info whitch one is selected currently//
                GUI_Inventory_Slots_Selected_Number = 1;
            }
            else if (GUI_Inventory_Cursor_Angle > 90 && GUI_Inventory_Cursor_Angle < 135)
            {
                //Give info whitch one is selected currently//
                GUI_Inventory_Slots_Selected_Number = 2;
            }
            else if (GUI_Inventory_Cursor_Angle > 135 && GUI_Inventory_Cursor_Angle < 180)
            {
                //Give info whitch one is selected currently//
                GUI_Inventory_Slots_Selected_Number = 3;
            }

        }
        else //When Normal slots//
        {
            if (GUI_Inventory_Cursor_Angle < 0 && GUI_Inventory_Cursor_Angle > -36)
            {
                //Give info whitch one is selected currently//
                GUI_Inventory_Slots_Selected_Number = 4;
            }
            else if (GUI_Inventory_Cursor_Angle < 36 && GUI_Inventory_Cursor_Angle > -72)
            {
                //Give info whitch one is selected currently//
                GUI_Inventory_Slots_Selected_Number = 5;
            }
            else if (GUI_Inventory_Cursor_Angle < 72 && GUI_Inventory_Cursor_Angle > -108)
            {
                //Give info whitch one is selected currently//
                GUI_Inventory_Slots_Selected_Number = 6;
            }
            else if (GUI_Inventory_Cursor_Angle < 108 && GUI_Inventory_Cursor_Angle > -144)
            {
                //Give info whitch one is selected currently//
                GUI_Inventory_Slots_Selected_Number = 7;
            }
            else if (GUI_Inventory_Cursor_Angle < 144 && GUI_Inventory_Cursor_Angle > -180)
            {
                //Give info whitch one is selected currently//
                GUI_Inventory_Slots_Selected_Number = 8;
            }
        }

        if (actualSlot_LastFrame != GUI_Inventory_Slots_Selected_Number)
        {
            //Update Slot//
            GUI_Inventory_Slot_Update(GUI_Inventory_Slots_Selected_Number, false);


            //Update Description//
            if (playerInventory.inventory_Items[GUI_Inventory_Slots_Selected_Number] != null)
            {
                Interactable_Item itemController = playerInventory.inventory_Items[GUI_Inventory_Slots_Selected_Number].GetComponent<Interactable_Item>();
                GUI_Inventory_Description_Update(itemController.interactable_Item_GUI_Icon, itemController.interactable_Item_Name);
            }
            else
            {
                GUI_Inventory_Description_Update(null, null);
            }
        }
    }

    //Update Screen size values//
    void GUI_Inventory_Size_Update()
    {
        //Update Resolution//
        Vector2 ScreenResolution = new Vector2(Screen.width, Screen.height);

        //Half Resolution//
        screenSize_Half = ScreenResolution / 2;

        //Update UI Distances//
        GUI_Inventory_Description_Size = (GUI_Inventory_Description_Size_Percent * 0.01f) * ScreenResolution.y / 2;
        GUI_Inventory_Wheel_Size = (GUI_Inventory_Wheel_Size_Percent * 0.01f) * ScreenResolution.y / 2;
    }

    #region Update Slot Design

    void GUI_Inventory_Slot_Update(int actualSlotNumber, bool fullReset)
    {
        if (!fullReset)
        {
            for (int i = 0; i < 9; i++)
            {
                if (i == actualSlotNumber) //When its actual slot, then activate it//
                {
                    GUI_Inventory_Wheel_Slot_Highlight(GUI_Inventory_Slots[i]);
                }
                else //Else turn back to normal//
                {
                    GUI_Inventory_Wheel_Slot_Reset(GUI_Inventory_Slots[i]);
                }
            }
        }
        else
        {
            for (int i = 0; i < 9; i++)
            {
                GUI_Inventory_Wheel_Slot_Reset(GUI_Inventory_Slots[i]);
            }
        }


    }

    void GUI_Inventory_Wheel_Slot_Highlight(GameObject slotObject)
    {
        //Set new color//
        slotObject.GetComponentInChildren<Image>().color = GUI_Inventory_Slots_Selected_Highlight_Color;
    }

    void GUI_Inventory_Wheel_Slot_Reset(GameObject slotObject)
    {
        //Reset color to base white//
        slotObject.GetComponentInChildren<Image>().color = Color.white;

    }

    #endregion

    #endregion Wheel & Slot

    #region Update Slots & Description
    public void GUI_Inventory_Slot_Update(int slotNumber, Sprite itemSprite)
    {
        //Get Slot controller//
        GUI_Inventory_Slot GUI_Slot_Controler = GUI_Inventory_Slots[slotNumber].GetComponent<GUI_Inventory_Slot>();

        //Update Slot//
        GUI_Slot_Controler.GUI_Slot_Update(itemSprite);
    }

    void GUI_Inventory_Description_Update(Sprite slotSprite, string itemName)
    {
        if (slotSprite != null)
        {
            GUI_Inventory_Description_Image.sprite = slotSprite;
            GUI_Inventory_Description_Text.SetText(itemName);
        }
        else
        {
            GUI_Inventory_Description_Image.sprite = GUI_Inventory_Description_Image_DefaultSprite;
            GUI_Inventory_Description_Text.SetText(" ");
        }
    }


    #endregion Update Slots & Description

    //Update item in hand//
    void GUI_Inventory_Item_Update()
    {
        if (GUI_Inventory_Slots_Selected)
        {
            playerInventory.Inventory_ItemInHand_Change(GUI_Inventory_Slots_Selected_Number);
        }
    }

    #region Input Controllers
    void GUI_Inventory_Input(bool isEntered)
    {
        if (isEntered)
        {
            //Check if player can open an inventory//
            if (playerMain.canOpenInventory)
            {
                //if can//
                //Update Main Script//
                playerMain.playerGUIState = Player_Main.PlayerGUIStates.Inventory;
                playerMain.inventory_IsOpen = true;

                //Turn on Inventory//
                GUI_Inventory_Object.SetActive(true);

                //Update screen size//
                GUI_Inventory_Size_Update();
            }
        }
        else //When player cancel input//
        {
            //Update Main Script//
            playerMain.playerGUIState = Player_Main.PlayerGUIStates.Default;
            playerMain.inventory_IsOpen = false;

            //Here Update Item Function//
            GUI_Inventory_Item_Update();

            //Turn on Inventory//
            GUI_Inventory_Object.SetActive(false);
        }

    }

    void GUI_Inventory_Input_Cursor()
    {
        //Raw position//
        Vector2 GUI_Inventory_Input_CursorPosition_Raw = gameControls.Player_UI.Inventory_Cursor_Position.ReadValue<Vector2>();

        //Update Cursor position//
        GUI_Inventory_Input_CursorPosition = new Vector2(GUI_Inventory_Input_CursorPosition_Raw.x - screenSize_Half.x, GUI_Inventory_Input_CursorPosition_Raw.y - screenSize_Half.y);
    }
    #endregion Input Controllers

    #region OnEnable, OnDisable
    void OnEnable()
    {
        gameControls.Enable();
    }

    void OnDisable()
    {
        gameControls.Disable();
    }
    #endregion OnEnable, OnDisable
}
