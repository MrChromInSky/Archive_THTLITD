using UnityEngine.InputSystem;
using UnityEngine;


[RequireComponent(typeof(Player_Main))]
public class Player_Looking : MonoBehaviour
{
    #region Assignments
    GameControls gameControls; //Game Controls//
    Player_Main playerMain; //Player main controller//


    void Awake()
    {
        gameControls = new GameControls();
        playerMain = GetComponent<Player_Main>();

        //Setup Head//
        playerHead = playerMain.playerHead_Pivot;

        //Cursor debug//
       // Cursor.lockState = CursorLockMode.Locked;
    }
    #endregion Assignments

    #region Variables
    [Header("Look Input")]
    Vector2 lookDeltaInput_Raw; //Raw looking input//
    Vector2 lookDeltaInput; //Fully calculated input//

    [Header("Default Look Settings")]
    [Space]
    [Header("Smooth Time")]
    [SerializeField] float lookSmoothTime_Default; //Smoothing time//

    [Header("Max Angles")]
    [SerializeField] float defaultMaxLookAngle_up;
    [SerializeField] float defaultMaxLookAngle_down;

    [Header("Smoothing Values")] //Values for smoothing functions
    public Vector2 currentLookDelta_Default;
    Vector2 currentLookDeltaVelocity_Default;

    [Header("Execution Values")]
    public float verticalLookValue_Default;
    public float horizontalLookValue_Default;
    [Space]

    [Header("Player's Head")]
    GameObject playerHead;

    [Header("Debug Values")]
    public float mouseSensitivity;



    #endregion Variables

    void Update()
    {
        if (playerMain.inventory_IsOpen)
        {
            LookExecution_InInventory();
        }
        else if (playerMain.canLook)
        {
            LookState_Controller();
        }
    }

    void LookExecution_InInventory()
    {
        //Set cursor//
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void LookState_Controller()
    {
        switch (playerMain.playerMovementState)
        {
            case Player_Main.PlayerMovementStates.Default:
                LookExecution_Default();
                return;

            case Player_Main.PlayerMovementStates.Special:

                return;
        }
    }

    void LookExecution_Default()
    {
        #region Set Cursor
     /*   if(Cursor.visible != false)
        {
            Cursor.visible = false;
        }

        if(Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }*/
        #endregion Set cursor

        #region Input
        //Update Delta Input//
        lookDeltaInput_Raw = gameControls.Player.Look.ReadValue<Vector2>(); //Raw Input//
        lookDeltaInput = new Vector2(lookDeltaInput_Raw.x * mouseSensitivity, lookDeltaInput_Raw.y * mouseSensitivity); //Full input Value//

        //Smooth Input//
        currentLookDelta_Default = Vector2.SmoothDamp(currentLookDelta_Default, lookDeltaInput, ref currentLookDeltaVelocity_Default, lookSmoothTime_Default);
        #endregion Input

        //Execute Looking//
        #region Vertical Execution
        //Calculation//
        verticalLookValue_Default -= currentLookDelta_Default.y;
        verticalLookValue_Default = Mathf.Clamp(verticalLookValue_Default, -defaultMaxLookAngle_down, defaultMaxLookAngle_up);

        //Execution//
        playerHead.transform.localRotation = Quaternion.Euler(Vector3.right * verticalLookValue_Default);

        #endregion Vertical Execution

        #region Horizontal Execution
        if (playerMain.canRotate)
        {
            //Calculation//
            horizontalLookValue_Default = currentLookDelta_Default.x;

            //Execution//
            transform.Rotate(Vector3.up * horizontalLookValue_Default);
        }
        #endregion Horizontal Execution
    }

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
