using UnityEngine;

[RequireComponent(typeof(Player_Main))]
public class Player_StateController : MonoBehaviour
{
    #region Assignments
    //Player Main Script//
    Player_Main playerMain;

    //Movement Controllers//
    Player_Movement_Default playerMovement_Default;

    void Awake()
    {
        playerMain = GetComponent<Player_Main>();
        playerMovement_Default = GetComponent<Player_Movement_Default>();
    }
    #endregion Assignments

    void Update()
    {
        StateController_General(); //Check general state, and execute substate functions//
    }

    #region General State
    void StateController_General()
    {
        switch (playerMain.playerGeneralState)
        {
            //Main Movement State//
            case Player_Main.PlayerGeneralStates.Default:
                #region Default
                if (playerMain.canMove)
                {
                    StateController_OnAir(); //First update on air//
                    StateController_Default(); //Next do movement states//
                    StateController_Crouching(); //At the end crouching//
                }
                else
                {
                    playerMain.playerDefaultMovementState = Player_Main.PlayerDefaultMovementStates.Disabled;
                }

                #endregion Default
                return;

        }
    }
    #endregion General State

    #region Default States

    void StateController_Default()
    {
        if (playerMain.playerDefaultOnAirState == Player_Main.PlayerDefaultOnAirStates.Grounded) //When player is grounded//
        {
            //Steep slope is priority//
            if (playerMain.isOnSteepSlope && playerMain.canSlideFromSteepEdges)
            {
                playerMain.playerDefaultMovementState = Player_Main.PlayerDefaultMovementStates.SlidingFromEdge;
            }
            else if (playerMain.isCrouching && playerMain.canCrouch)
            {
                playerMain.playerDefaultMovementState = Player_Main.PlayerDefaultMovementStates.Crouching;
            }
            else if (playerMain.enteredInput_Movement && playerMain.canWalk)
            {
                if (playerMain.enteredInput_Run && playerMain.canRun)
                {
                    playerMain.playerDefaultMovementState = Player_Main.PlayerDefaultMovementStates.Running;
                }
                else
                {
                    playerMain.playerDefaultMovementState = Player_Main.PlayerDefaultMovementStates.Walking;
                }
            }
            else
            {
                playerMain.playerDefaultMovementState = Player_Main.PlayerDefaultMovementStates.Idle;
            }

        }
        else //Player is not grounded//
        {
            playerMain.playerDefaultMovementState = Player_Main.PlayerDefaultMovementStates.OnAir;
        }
    }

    void StateController_Crouching()
    {
        if (playerMain.playerDefaultMovementState == Player_Main.PlayerDefaultMovementStates.Crouching)
        {
            if (playerMain.enteredInput_Movement)
            {
                if (playerMain.enteredInput_Run && playerMain.canStandUp) //When start to run switch state//
                {
                    SwitchState_Crouch_To_Run();
                }
                else  //When not inputing run//
                {
                    playerMain.playerDefaultCrouchState = Player_Main.PlayerDefaultCrouchStates.Crouching_Walk;
                }
            }
            else
            {
                playerMain.playerDefaultCrouchState = Player_Main.PlayerDefaultCrouchStates.Crouching_Idle;
            }
        }
        else
        {
            playerMain.playerDefaultCrouchState = Player_Main.PlayerDefaultCrouchStates.NotCrouching;
        }
    }

    void StateController_OnAir()
    {
        if (playerMain.isJumping)
        {
            playerMain.playerDefaultOnAirState = Player_Main.PlayerDefaultOnAirStates.Jumping;
        }
        else if (playerMain.isGrounded)
        {
            //When player is grounded//
            playerMain.playerDefaultOnAirState = Player_Main.PlayerDefaultOnAirStates.Grounded;
        }
        else
        {
            //When player is falling//
            if (playerMovement_Default.movement_Vertical_Velocity >= 0)
            {
                playerMain.playerDefaultOnAirState = Player_Main.PlayerDefaultOnAirStates.OnAir;
            }
            else //Player is falling//
            {
                playerMain.playerDefaultOnAirState = Player_Main.PlayerDefaultOnAirStates.OnAir_Falling;
            }
        }
    }


    #endregion Default States

    #region State Switch Functions
    //Tell movement conrtoller to change variables//
    void SwitchState_Crouch_To_Run()
    {
        //Change crouhing to false//
        playerMain.isCrouching = false;

        //Start Cooldown//
        playerMovement_Default.Cooldown_Crouch();
    }

    #endregion State Switch Functions
}
