using UnityEngine;

public class Player_Main : MonoBehaviour
{
    #region Variables

    #region States
    //GUI States//
    public enum PlayerGUIStates { Default, Inventory }
    [Header("States", order = 0)]
    [Header("GUI State", order = 1)]
    public PlayerGUIStates playerGUIState;

    //General States//
    public enum PlayerGeneralStates { Default }; //Default state is movement//
    [Header("General State")]
    public PlayerGeneralStates playerGeneralState;

    //Movement Modes//
    public enum PlayerMovementStates { Disabled, Default, Special };
    [Header("Movement State")]
    public PlayerMovementStates playerMovementState;

    #region Default Movement State

    //Movement State - Default//
    public enum PlayerDefaultMovementStates { Disabled, Idle, Walking, Crouching, Running, OnAir, SlidingFromEdge };
    [Header("Default Movement State")]
    public PlayerDefaultMovementStates playerDefaultMovementState;

    //Default State - Crouching//
    public enum PlayerDefaultCrouchStates { NotCrouching, Crouching_Idle, Crouching_Walk };
    [Header("Crouch State")]
    public PlayerDefaultCrouchStates playerDefaultCrouchState;

    //Default State - OnAir//
    public enum PlayerDefaultOnAirStates { Grounded, Jumping, OnAir, OnAir_Falling };
    [Header("On Air State")]
    public PlayerDefaultOnAirStates playerDefaultOnAirState;

    #endregion Default Movement State



    #endregion States

    #region Cases
    [Header("Cases", order = 0)]

    [Header("General Cases", order = 1)]
    public bool isChangingPosition; //When player is changing his actual position//

    [Header("Input Cases")]
    public bool enteredInput_Look;  //If look inputs are entered//
    public bool enteredInput_Movement; //If movement inputs are entered/
    public bool enteredInput_Run; //If player entered run input//
    public bool enteredInput_Jump; //If playeres entered jump this frame//
    [Space]

    [Header("Inventory Cases")]
    public bool inventory_IsOpen = false;
    [Space]
    public bool inventory_Primary_IsFull = false;
    public bool inventory_Normal_IsFull = false;
    [Space]

    [Header("Hands")]
    public bool hands_InFront = true; //When hands are not hide//
    public bool hands_CanPullOut = true;
    public bool hands_CanHide = true;

    #region Default State
    //Default movement cases//
    [Space, Header("Default State Cases")]
    public bool isGrounded; //If player stays on the ground//
    [Space]
    public bool isOnSteepSlope; //If player stays on slope that is to steep//
    [Space]
    public bool isJumping; //When player start to jump//
    public bool playerJumped; //When player is on air after player jumped//
    [Space]
    public bool hittedCelling; //When player hits celling//
    [Space]
    public bool isCrouching; //If player is crouching//
    [Space]
    public bool isRunning; //If player is actualy running//
    [Space]
    public bool playerLanded; //If player landed on the ground//
    [Space]
    public bool isMoving; //Chenging position + input//
    [Space]
    public bool movement_Momentum_Decelerate_AfterLanding;

    #endregion Default State

    #endregion Cases

    #region Possibilities
    [Header("Possibilities", order = 0)]
    [Header("UI Possiblilities", order = 1)]
    public bool canOpenInventory = true;

    [Header("General Possibilities")]
    public bool canMove = true; //If player can change position//
    public bool canInteract = true;
    public bool canRotate = true;
    [Space]

    #region Default State
    [Header("Default State Possiblities")]
    public bool canLook = true;
    [Space]
    public bool canWalk = true;
    public bool canRun = true;
    public bool canCrouch = true;
    public bool canStandUp = true;
    public bool canJump = true;
    [Space]
    public bool canSlideFromSteepEdges = true;

    #endregion Default State

    #endregion Possibilities

    #region Elements
    [Header("Player Elements")]
    public GameObject playerHead_Base; //Player Head Game Object//
    public GameObject playerHead_Pivot; //For rotation
    public GameObject playerHead_Height; //For Headbobbing

    #endregion Elements

    [Header("Debug Setting")]
    public bool headBobbing_Active = true;
    public bool head_Stabilisation_Active = true;

    #endregion Variables
}
