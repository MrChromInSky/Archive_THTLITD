using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public abstract class Interactable_Main : MonoBehaviour
{
    #region Assignments
    Player_Main playerMain;

    //Interaction Controller On Player//
    Player_Interaction_Controller player_Interaction_Controller;

    void Start()
    {
        playerMain = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Main>();
        player_Interaction_Controller = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Interaction_Controller>();
    }
    #endregion Assignments

    #region Variables

    #region Interaction Type
    public enum interactable_InteractionTypes { Hold, Click };
    [Header("Interaction Type")]
    public interactable_InteractionTypes interactable_InteractionType;
    #endregion Interaction Type

    #region Is Single Use?
    [Header("Is Single Use?")]
    //Not for use in Items//
    [SerializeField] bool interactable_SingleUse = false;
    bool interactable_SingleUse_Used = false; //If it was used once before//
    #endregion Is Single Use?

    #region Hold Interaction Variables
    //All hold variables//

    #region Hold Reset

    #region Hold Reset Behaviour
    //Reset Behaviour//
    public enum interactable_Interaction_Hold_ResetBehaviours { InTime, Instant };
    [Header("Hold Reset Behaviour")]
    public interactable_Interaction_Hold_ResetBehaviours interactable_Interaction_Hold_ResetBehaviour;

    #endregion Hold Reset Behaviour

    #region Hold Reset Time
    //Time to full reset//
    [Header("Hold Reset Time")]
    [SerializeField] float interactable_Interaction_Hold_ResetTime = 0.4f;
    float interactable_Interaction_Hold_Reset_Calculation; //For SmootDamp//
    #endregion Hold Reset Time

    #region Hold Reset Started
    [Header("Reset Started")]
    public bool interactable_Interaction_Hold_Reset_Started = false;

    #endregion

    #endregion Hold Reset

    #region Hold Progress Target
    //Target That object must overcome//
    [Header("Hold Interaction Progress Target")]
    public float interactable_Interaction_HoldProgress_Target = 1f;

    #endregion Hold Progress Target

    #region Hold Interaction Progress
    //Actual Progress//
    [Header("Hold Interaction Progress")]
    public float interactable_Interaction_HoldProgress = 0;

    #endregion Hold Interaction Progress

    #endregion Hold Interaction Variables

    #endregion Variables

    #region Interaction Functions
    public void Interaction_Handler()
    {
        #region Single Use Controller
        //check if it was used before, when yes then returns function//
        if (interactable_SingleUse)
        {
            if (interactable_SingleUse_Used)
            {
                return;
            }
            else
            {
                //UI Says That "You cannot do that again"//
            }
        }

        #endregion Single Use Controller

        #region Stop Reseting
        //When Hold Reset Function is on Reset//
        if (interactable_Interaction_Hold_Reset_Started)
        {
            interactable_Interaction_Hold_Reset_Started = false;
        }

        #endregion Stop Reseting

        #region Interaction
        switch (interactable_InteractionType)
        {
            case interactable_InteractionTypes.Click:
                Interaction_Handler_Click();
                return;

            case interactable_InteractionTypes.Hold:
                Interaction_Handler_Hold();
                return;
        }
        #endregion Interaction
    }

    //Well, Just Interact//
    void Interaction_Handler_Click()
    {
        //Interact//
        Interact();

        //Set Cooldown//
        Interactable_Player_SetCooldown();
    }

    void Interaction_Handler_Hold()
    {
        //Add Progress//
        interactable_Interaction_HoldProgress += Time.deltaTime;

        //Checks if target is reached//
        if (interactable_Interaction_HoldProgress >= interactable_Interaction_HoldProgress_Target)
        {
            //Interact//
            Interact();

            //Set Cooldown//
            Interactable_Player_SetCooldown();

            //Reset Hold Progress//
            interactable_Interaction_HoldProgress = 0;
        }

    }


    //Final Custom Function//
    public abstract void Interact();

    #endregion Functions

    void Update()
    {
        if (interactable_InteractionType == interactable_InteractionTypes.Hold)
        {
            if (interactable_Interaction_Hold_Reset_Started)
            {
                Interaction_Hold_Progress_Reset();
            }
        }
    }

    #region Hold Reset Function

    public void Interaction_Hold_Progress_Reset()
    {

        switch (interactable_Interaction_Hold_ResetBehaviour)
        {
            case interactable_Interaction_Hold_ResetBehaviours.Instant:
                Interaction_Hold_Progress_Reset_Instant();
                return;


            case interactable_Interaction_Hold_ResetBehaviours.InTime:
                Interaction_Hold_Progress_Reset_InTime();
                return;

        }

    }

    //Instant//
    void Interaction_Hold_Progress_Reset_Instant()
    {
        //Reset Progress//
        interactable_Interaction_HoldProgress = 0;
        //Reset Reseting function//
        interactable_Interaction_Hold_Reset_Started = false;
    }

    //In Time//
    void Interaction_Hold_Progress_Reset_InTime()
    {
        //Start Reset, When its not started//
        if (!interactable_Interaction_Hold_Reset_Started)
        {
            interactable_Interaction_Hold_Reset_Started = true;
        }

        if (interactable_Interaction_HoldProgress >= 0 && interactable_Interaction_HoldProgress < 0.05f)
        {
            //Round To zero//
            interactable_Interaction_HoldProgress = 0;
            //Reset Reseting function//
            interactable_Interaction_Hold_Reset_Started = false;
        }
        else
        {
            interactable_Interaction_HoldProgress = Mathf.SmoothDamp(interactable_Interaction_HoldProgress, 0, ref interactable_Interaction_Hold_Reset_Calculation, interactable_Interaction_Hold_ResetTime);
        }

    }

    #endregion Hold Reset

    #region Set Player Cooldown

    void Interactable_Player_SetCooldown()
    {
        //When Interaction Is for sure ended, then mark tis as used//
        if (interactable_SingleUse)
        {
            interactable_SingleUse_Used = true;
        }

        //Start Cooldown Function In Player Interaction Controller//
        player_Interaction_Controller.Interaction_Cooldown();
    }

    #endregion
}
