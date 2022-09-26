using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Player_Interaction_Controller : MonoBehaviour
{
    #region Assignemnts
    Player_Main playerMain;
    GameControls gameControls; //Game Controls//

    void Awake()
    {
        playerMain = GetComponent<Player_Main>();
        gameControls = new GameControls();

        #region Controls
        gameControls.Player.Interact.started += ctx => Interaction_Input_Started_Controller();
        gameControls.Player.Interact.canceled += ctx => interaction_Input_Started = false;
        #endregion Controls

        SetStartValues();
    }
    #endregion Assignments

    #region Variables

    #region Ray Parameters
    [Header("Interaction Ray Parameters")]
    [SerializeField] float interaction_Ray_Lenght;
    [Space]
    [SerializeField] LayerMask interaction_Ray_Layers;
    RaycastHit interaction_Ray_Hit;
    #endregion Ray Parameters

    #region Input
    [Header("Interaction Started?")]
    [SerializeField] bool interaction_Input_Started = false;
    #endregion Input

    #region Cooldown
    [Header("Cooldown Parameters")]
    [SerializeField] bool interaction_Cooldown_Done = true;
    [SerializeField] float interaction_Cooldown_Time; //How long cooldown reset takes//
    #endregion Cooldown

    #region Interaction Objects
    [Header("Interactable Objects")]
    public GameObject interaction_InteractiveObject_Actual; //Actual interaction object//
    public GameObject interaction_InteractiveObject_LastFrame; //Interaction Object Last Frame//

    #endregion Interaction Objects

    #region Player Elements
    [Header("Player Head")]
    [SerializeField] Transform playerHead;

    #endregion

    #endregion Variables


    void SetStartValues()
    {
        //Head//
        if (playerHead != null)
        {
            playerHead = GameObject.FindGameObjectWithTag("Player_Head_Pivot").transform;
        }
    }

    void Update()
    {
        //Start interaction//
        Interaction_Raycast();
    }

    #region Interaction Functions
    void Interaction_Raycast()
    {
        if (Physics.Raycast(playerHead.position, playerHead.transform.forward, out interaction_Ray_Hit, interaction_Ray_Lenght, interaction_Ray_Layers)) //When hit something
        {
            Interaction_Controller();
        }
        else //Else reset holding, and gameobjects
        {
            //Reset Progress
            Interaction_Reset_HoldProgress();

            //Reset Objects//
            Interaction_Reset_InteractiveObjects();
        }
    }

    void Interaction_Controller()
    {
        //Hitted GameObject//
        GameObject interactionHitted_GameObject = interaction_Ray_Hit.collider.gameObject;

        //Input Start//
        if (interaction_Input_Started)
        {

            if (interactionHitted_GameObject.GetComponent<Interactable_Main>()) //When object is interactive//
            {
                //Update Actual Gameobject//
                interaction_InteractiveObject_Actual = interaction_Ray_Hit.collider.gameObject;


                if (interaction_InteractiveObject_Actual == interaction_InteractiveObject_LastFrame || interaction_InteractiveObject_LastFrame == null) //Where they are the same object do not do nothing//
                {
                    Interaction_Controller_InteractableType(interactionHitted_GameObject);
                }
                else //Else//
                {
                    //Reset Progress//
                    Interaction_Reset_HoldProgress();

                    //Reset Objects//
                    Interaction_Reset_InteractiveObjects();
                }


                //Update Last frame object//
                interaction_InteractiveObject_LastFrame = interaction_InteractiveObject_Actual;
            }
            else //Else reset progress//
            {
                //Reset Progress
                Interaction_Reset_HoldProgress();

                //Reset Objects//
                Interaction_Reset_InteractiveObjects();
            }
        }
        else
        {
            //Reset Progress
            Interaction_Reset_HoldProgress();

            //Reset Objects//
            Interaction_Reset_InteractiveObjects();
        }
    }


    //Here I can manage situations when player cannot do something, before interaction is done//
    void Interaction_Controller_InteractableType(GameObject interactionGameObject)
    {
        #region Items
        if (interactionGameObject.GetComponent<Interactable_Item>()) //When it's an item//
        {
            Interaction_Controller_Item(interactionGameObject);
        }
        #endregion Items

        #region Objects
        else if (interactionGameObject.GetComponent<Interactable_Object>()) //When it's an object//
        {
            Interaction_Controller_Object(interactionGameObject);
        }
        #endregion Objects

    }

    //When Interacting with item//
    void Interaction_Controller_Item(GameObject interactionItem)
    {
        Interactable_Item itemController = interactionItem.GetComponent<Interactable_Item>();

        if (itemController.interactable_Item_IsPrimary)
        {
            if (!playerMain.inventory_Primary_IsFull) //When inventory have free space//
            {
                Interaction_Execution_Controller();
            }
            else //When inventory is full, not possible with primary//
            {
                //GUI Interaction//
                Debug.Log("Primary is full");

                //Reset Progress
                Interaction_Reset_HoldProgress();

                //Reset Objects//
                Interaction_Reset_InteractiveObjects();

                return;
            }

        }
        else //When its normal
        {
            if (!playerMain.inventory_Normal_IsFull) //When inventory have free space//
            {
                Interaction_Execution_Controller();
            }
            else //When inventory is full//
            {
                //GUI Interaction//
                Debug.Log("Normal is full");

                //Reset Progress
                Interaction_Reset_HoldProgress();

                //Reset Objects//
                Interaction_Reset_InteractiveObjects();

                return;
            }
        }
    }

    //When Interactig with Objects//
    void Interaction_Controller_Object(GameObject interactionObject)
    {
        Interactable_Object objectController = interactionObject.GetComponent<Interactable_Object>();

        if (objectController.interactable_Object_Cooldown_Done) //When cooldown is done//
        {
            Interaction_Execution_Controller();
        }
        else //When Object is still on cooldown//
        {
            Debug.Log("Still on cooldown");

            //Reset Progress
            Interaction_Reset_HoldProgress();

            //Reset Objects//
            Interaction_Reset_InteractiveObjects();

            return;
        }

    }


    //Execute Interaction//
    void Interaction_Execution_Controller()
    {
        Interactable_Main interacive = interaction_InteractiveObject_Actual.GetComponent<Interactable_Main>();

        interacive.Interaction_Handler();
    }

    #endregion Interaction Functions

    #region Reset Functions
    void Interaction_Reset_InteractiveObjects()
    {
        //Reset Input//
        interaction_Input_Started = false;

        if (interaction_InteractiveObject_Actual != null)
        {
            interaction_InteractiveObject_Actual = null;
        }

        if (interaction_InteractiveObject_LastFrame != null)
        {
            interaction_InteractiveObject_LastFrame = null;
        }
    }

    void Interaction_Reset_HoldProgress()
    {
        if (interaction_InteractiveObject_Actual == null)
        {
            if (interaction_InteractiveObject_LastFrame != null)
            {
                interaction_InteractiveObject_LastFrame.GetComponent<Interactable_Main>().Interaction_Hold_Progress_Reset();
            }
        }
        else
        {
            if (interaction_InteractiveObject_LastFrame != null)
            {
                if (interaction_InteractiveObject_Actual == interaction_InteractiveObject_LastFrame)
                {
                    interaction_InteractiveObject_Actual.GetComponent<Interactable_Main>().Interaction_Hold_Progress_Reset();
                }
                else
                {
                    interaction_InteractiveObject_LastFrame.GetComponent<Interactable_Main>().Interaction_Hold_Progress_Reset();
                }

            }
            else
            {
                interaction_InteractiveObject_Actual.GetComponent<Interactable_Main>().Interaction_Hold_Progress_Reset();
            }
        }
    }

    #endregion Reset Functions

    #region Input Start Function
    void Interaction_Input_Started_Controller()
    {
        if (interaction_Cooldown_Done && playerMain.canInteract)
        {
            interaction_Input_Started = true;
        }
    }

    #endregion

    #region Cooldown
    public void Interaction_Cooldown()
    {

        //Interaction blocked//
        interaction_Cooldown_Done = false;

        //Input Reset//
        interaction_Input_Started = false;

        //Start Cooldown Count//
        StartCoroutine(Interaction_Cooldown_Reset());
    }

    IEnumerator Interaction_Cooldown_Reset()
    {
        //Wait time//
        yield return new WaitForSeconds(interaction_Cooldown_Time);

        //Reset Cooldown//
        interaction_Cooldown_Done = true;
    }
    #endregion

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
