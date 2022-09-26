using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player_Main))]
[RequireComponent(typeof(CharacterController))]
public class Player_Movement_Default_Crouching : MonoBehaviour
{
    #region Assignments
    Player_Main playerMain; //Player Main Contoller//
    CharacterController playerController; //Character Controller//

    void Awake()
    {
        playerMain = GetComponent<Player_Main>();
        playerController = GetComponent<CharacterController>();

        SetStartValues(); //Set up on start standing values//
    }
    #endregion Assignments

    #region Variables

    #region Player Height
    [Header("Height Parameters")]
    public float playerHeight_Stand; //player size on stand//
    [SerializeField] float playerHeight_Stand_Speed; //How quick player reach stand height//
    float playerController_Center_Stand_Y;
    [Space]
    [SerializeField] float playerHeight_Crouch; //player size when crouching//
    [SerializeField] float playerHeight_Crouch_Speed; //How quick player will crouch//
    float playerController_Center_Crouch_Y;
    float playerHeight_CalculationValue; //Value for smooth damp function//
    #endregion Player Height

    #region Camera Position
    [Header("Camera Parameters")]
    [SerializeField] GameObject playerHead; //Gamo object of player head//
    [Space]
    [SerializeField] float playerHead_Position_Y_Stand;
    [SerializeField] float playerHead_Position_Y_Stand_Speed;
    [Space]
    [SerializeField] float playerHead_Position_Y_Crouch;
    [SerializeField] float playerHead_Position_Y_Crouch_Speed;
    float playerHead_CalculationValue;
    #endregion Camera Position

    #region Rounding Value
    [Header("Rounding Value")]
    [SerializeField] float roundingValue;
    #endregion Rounding Value

    #endregion Variables

    void Update()
    {
        //Check if player is in default movement
        if (playerMain.playerGeneralState == Player_Main.PlayerGeneralStates.Default && playerMain.playerMovementState == Player_Main.PlayerMovementStates.Default)
        {
            CrouchController(); //Controls Crouching
            SetCharacterControllerCenter(); //Controls center of character controller - local transforms problem fix//
        }
    }

    void CrouchController()
    {
        switch (playerMain.playerDefaultCrouchState)
        {
            case Player_Main.PlayerDefaultCrouchStates.Crouching_Idle:
            case Player_Main.PlayerDefaultCrouchStates.Crouching_Walk:
                CrouchController_ToCrouch();
                return;

            case Player_Main.PlayerDefaultCrouchStates.NotCrouching:
                CrouchController_ToStand();
                return;
        }

    }

    void SetCharacterControllerCenter()
    {
        //Sets new center for character controler - always half of height//
        if(playerController.center.y != playerController.height/2)
        {
            Vector3 newCenter = new Vector3(playerController.center.x, playerController.height / 2, playerController.center.z);
            playerController.center = newCenter;
        }
    }

    #region Crouching
    //Crouching functions//
    void CrouchController_ToCrouch()
    {
        CrouchController_ToCrouch_Camera();
        CrouchController_ToCrouch_Height();
    }

    void CrouchController_ToCrouch_Camera()
    {
        float playerHead_Position_Y_actual = playerHead.transform.localPosition.y;

        if (playerHead_Position_Y_actual == playerHead_Position_Y_Crouch || (playerHead_Position_Y_actual < playerHead_Position_Y_Crouch && playerHead_Position_Y_actual > playerHead_Position_Y_Crouch - roundingValue) || (playerHead_Position_Y_actual > playerHead_Position_Y_Crouch && playerHead_Position_Y_actual < playerHead_Position_Y_Crouch + roundingValue))
        {
            playerHead_Position_Y_actual = playerHead_Position_Y_Crouch;
        }
        else
        {
            playerHead_Position_Y_actual = Mathf.SmoothDamp(playerHead_Position_Y_actual, playerHead_Position_Y_Crouch, ref playerHead_CalculationValue, playerHead_Position_Y_Crouch_Speed);
        }

        //Create new vector for camera movement//
        Vector3 playerHead_newVector = new Vector3(playerHead.transform.localPosition.x, playerHead_Position_Y_actual, playerHead.transform.localPosition.z);

        //Update Camera Position//
        playerHead.transform.localPosition = playerHead_newVector;
    }

    void CrouchController_ToCrouch_Height()
    {
        //Changes size of CharacterController//

        if (playerController.height == playerHeight_Crouch || (playerController.height < playerHeight_Crouch && playerController.height > playerHeight_Crouch - roundingValue) || (playerController.height > playerHeight_Crouch && playerController.height < playerHeight_Crouch + roundingValue)
)
        {
            playerController.height = playerHeight_Crouch;
        }
        else
        {
            playerController.height = Mathf.SmoothDamp(playerController.height, playerHeight_Crouch, ref playerHeight_CalculationValue, playerHeight_Crouch_Speed);
        }
    }

    #endregion Crouching

    #region Standing
    //Standing functions//
    void CrouchController_ToStand()
    {
        CrouchController_ToStand_Height();
        CrouchController_ToStand_Camera();
    }

    void CrouchController_ToStand_Camera()
    {
        float playerHead_Position_Y_actual = playerHead.transform.localPosition.y;

        if (playerHead_Position_Y_actual == playerHead_Position_Y_Stand || (playerHead_Position_Y_actual < playerHead_Position_Y_Stand && playerHead_Position_Y_actual > playerHead_Position_Y_Stand - roundingValue) || (playerHead_Position_Y_actual > playerHead_Position_Y_Stand && playerHead_Position_Y_actual < playerHead_Position_Y_Stand + roundingValue))
        {
            playerHead_Position_Y_actual = playerHead_Position_Y_Stand;
        }
        else
        {
            playerHead_Position_Y_actual = Mathf.SmoothDamp(playerHead_Position_Y_actual, playerHead_Position_Y_Stand, ref playerHead_CalculationValue, playerHead_Position_Y_Stand_Speed);
        }

        //Create new vector for camera movement//
        Vector3 playerHead_newVector = new Vector3(playerHead.transform.localPosition.x, playerHead_Position_Y_actual, playerHead.transform.localPosition.z);

        //Update Camera Position//
        playerHead.transform.localPosition = playerHead_newVector;
    }

    void CrouchController_ToStand_Height()
    {
        //Changes size of CharacterController//

        if (playerController.height == playerHeight_Stand || (playerController.height < playerHeight_Stand && playerController.height > playerHeight_Stand - roundingValue) || (playerController.height > playerHeight_Stand && playerController.height < playerHeight_Stand + roundingValue))
        {
            playerController.height = playerHeight_Stand;
        }
        else
        {
            float lastHeight = playerController.height; //Save last position for fixing jiter//

            playerController.height = Mathf.SmoothDamp(playerController.height, playerHeight_Stand, ref playerHeight_CalculationValue, playerHeight_Stand_Speed);

            CrouchController_ToStand_Height_FixPosition(lastHeight); //Function that fixes jitter on standing//
        }
    }

    void CrouchController_ToStand_Height_FixPosition(float LastHeight) //Fixing jitter when standing
    {
        float fixPositionY = (playerController.height - LastHeight) / 2; //Value that ned to be added to y axis to fix jitter.//

        playerController.enabled = false; //Trun of Character controller to overwrite transform//
        transform.position = new Vector3(transform.position.x, transform.position.y + fixPositionY, transform.position.z);
        playerController.enabled = true; //Turn it on after fixing Vertical position//
    }

    #endregion Standing

    void SetStartValues() //Set up on start standing values//
    {
        if(playerHead == null)
        {
            playerHead = playerMain.playerHead_Base;
        }

        //Set Stand Height value on start//
        playerHeight_Stand = playerController.height;

        //Set Stand Position for Y-Axis//
        playerHead_Position_Y_Stand = playerHead.transform.localPosition.y;

        //Set Centers for character controller//
        playerController_Center_Stand_Y = playerController.center.y;
        playerController_Center_Crouch_Y = playerHeight_Crouch * 0.5f;
    }
}
