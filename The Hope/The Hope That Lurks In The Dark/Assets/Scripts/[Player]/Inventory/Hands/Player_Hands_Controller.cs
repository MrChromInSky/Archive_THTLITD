using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Animations;
using UnityEngine;

public class Player_Hands_Controller : MonoBehaviour
{
    #region Assignments
    Player_Main playerMain;
    Player_Inventory_Controller player_Inventory_Controller;
    GameControls gameControls;
    Animator hands_Animator; //Animator//

    void Awake()
    {
        playerMain = GetComponentInParent<Player_Main>();
        player_Inventory_Controller = GameObject.FindGameObjectWithTag("Player_Inventory").GetComponent<Player_Inventory_Controller>();
        gameControls = new GameControls();
        hands_Animator = GetComponent<Animator>();

        #region Controls
        // Hide, Pull out controller//
        gameControls.Player_Items.Hands_HidePullOut.started += ctx => Hands_Input_HidePullOut_Controller();

        #endregion Controls
    }
    #endregion Assignments

    #region Variables
    [Header("Item In Hands")]
    public GameObject hands_Item_InHand;
    public GameObject hands_Item_InHand_New;

    [Header("Item Positions In Hands")]
    [SerializeField] Transform[] hands_Item_Positions;

    [Header("Hide & PullOut Parameters")]
    [SerializeField] float hands_HidePullOut_Input_HoldTime;
    float hands_HidePullOut_Input_HoldTime_Actual = 0;
    bool hands_ItemChanging_Hide_Started = false;

    [Header("Item Forward Rotation Speed")]
    [SerializeField] float hands_Item_RotationSpeed;

    [Header("Input")]
    bool hands_HidePullOut_Started = false;

    [Header("Animations")]
    public bool hands_Animation_IsAnimated;
    #endregion Variables

    public void Hands_Item_Change(GameObject newItem)
    {
        //Set new update//
        hands_Item_InHand_New = newItem;

        if (hands_Item_InHand_New == null)
        {
            //Only hide current item//

        }
        else
        {
            if (hands_Item_InHand == null)
            {
                //Only pull out new item//
                Hands_Item_PullOut_Controller(true);
            }
            else
            {
                //Hide old one, next pull out current//

            }
        }
    }

    void Update()
    {
        //Controls manual hiding, and pulling out//
        Hands_HidePullOut_Controller();


    }

    #region Pull Out Item
    void Hands_Item_PullOut_Controller(bool isSwitchingItem)
    {
        if (isSwitchingItem)
        {
            //When Changing items//
            //Update Actual Item//
            hands_Item_InHand = hands_Item_InHand_New;

            if (hands_Item_InHand != null)
            {
                Hands_Item_Setup();

                //Play animation//
                Hands_Item_PullOut_Animation();
            }
        }
        else
        {
            //When manualy hiding//



        }
    }

    void Hands_Item_PullOut_Animation()
    {
        int itemPosition = hands_Item_InHand.GetComponent<Interactable_Item>().interactable_Item_InHands_Position;

        //Update State//
        playerMain.hands_InFront = true;

        switch (itemPosition)
        {
            case 0:
                hands_Animator.Play("Hands_PullOut_RightDown");
                return;
        }
    }

    void Hands_Item_Setup()
    {
        int itemPosition = hands_Item_InHand.GetComponent<Interactable_Item>().interactable_Item_InHands_Position;

        //Set new parent//
        hands_Item_InHand.transform.parent = hands_Item_Positions[itemPosition];

        //Reset Positions//
        hands_Item_InHand.transform.localPosition = Vector3.zero;
        hands_Item_InHand.transform.localRotation = Quaternion.identity;

        //Set Visiblitity//
        hands_Item_InHand.SetActive(true);

        //Set interactivity//
        hands_Item_InHand.GetComponent<Interactable_Item_Interactions>().interactable_Item_IsInHands = true;
    }
    #endregion Pull Out Item

    #region Hide & PullOut
    void Hands_HidePullOut_Controller()
    {
        if (hands_HidePullOut_Started)
        {
            //Check if still pressed//
            if (gameControls.Player_Items.Hands_HidePullOut.IsPressed())
            {
                //Update progress//
                hands_HidePullOut_Input_HoldTime_Actual += Time.deltaTime;

                if (hands_HidePullOut_Input_HoldTime >= hands_HidePullOut_Input_HoldTime_Actual)
                {
                    //Execute//
                    Hands_HidePullOut_Controller_Execution();

                    //Reset Entered Input//
                    hands_HidePullOut_Started = false;

                    //Reset Progress//
                    hands_HidePullOut_Input_HoldTime_Actual = 0;
                }
            }
            else
            {
                //Reset without execution//
                //Reset Entered Input//
                hands_HidePullOut_Started = false;

                //Reset Progress//
                hands_HidePullOut_Input_HoldTime_Actual = 0;
            }

        }
    }

    void Hands_HidePullOut_Controller_Execution()
    {
        //Switch actual state//
        playerMain.hands_InFront = !playerMain.hands_InFront;

        //Play animation//
        if (playerMain.hands_InFront)
        {
            //Pull out//
        }
        else
        {
            //Hide//
        }

    }

    #endregion 

    #region Input
    void Hands_Input_HidePullOut_Controller()
    {
        if (playerMain.hands_InFront)
        {
            if (playerMain.hands_CanHide)
            {
                hands_HidePullOut_Started = true;
            }
        }
        else
        {
            if (playerMain.hands_CanPullOut)
            {
                hands_HidePullOut_Started = true;
            }
        }
    }
    #endregion Input

    #region OnDisable, OnEnable
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
