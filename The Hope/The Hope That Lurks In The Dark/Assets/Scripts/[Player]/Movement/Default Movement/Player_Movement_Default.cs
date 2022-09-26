using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;

#region Require Components
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Player_Main))]
[RequireComponent(typeof(GameControls))]
#endregion Require Components
public class Player_Movement_Default : MonoBehaviour
{
    #region Assignments
    Player_Main playerMain; //Player main control script//
    GameControls gameControls; //Input controller//
    CharacterController playerController; //Character controller//

    void Awake()
    {
        playerMain = GetComponent<Player_Main>();
        gameControls = new GameControls();
        playerController = GetComponent<CharacterController>();

        #region Controls
        //Run//
        gameControls.Player.Run.started += ctx => Input_Controller_Run(true);
        gameControls.Player.Run.canceled += ctx => Input_Controller_Run(false);

        //Crouch//
        gameControls.Player.Crouch.started += ctx => Input_Controller_Crouch();

        //Jump//
        gameControls.Player.Jump.started += ctx => Input_Controller_Jump();
        #endregion Controls
    }
    #endregion Assignments

    #region Variables

    #region Velocities
    //Velocity Speed of movements//
    [Header("Horizontal Velocities", order = 0)]

    //Stop Speed//
    [Header("Stop Speed", order = 1)]
    [SerializeField] float velocity_Stop_Deceleration; //Speed of Stoping//

    [Header("One Axis Stop")]
    [SerializeField] float velocity_Stop_OneAxis_Deceleration; //Speed of stopping on one axis//

    [Header("After Landing Momentum Stop")]
    [SerializeField] float velocity_Stop_Landing_Momentum_Deceleration;
    [Space]
    [SerializeField] float velocity_Time_Landing_Momentum_Execution = 0.2f;

    //Walk Speed//
    [Header("Walk Speed")]
    [SerializeField] float velocity_Walk; //Maximal Speed//
    [SerializeField] float velocity_Walk_Acceleration; //Speed Acceleration//

    //Run Speed//
    [Header("Run Speed")]
    [SerializeField] float velocity_Run; //Maximal Speed//
    [SerializeField] float velocity_Run_Acceleration; //Speed Acceleration//

    [Header("Crouch Speed")]
    [SerializeField] float velocity_Crouch; //Maximal Speed//
    [SerializeField] float velocity_Crouch_Acceleration; //Speed Acceleration//

    [Header("On Air")]
    [SerializeField] float velocity_OnAir; //Maximal Speed//
    [SerializeField] float velocity_OnAir_Acceleration; //Speed Acceleration//
    #endregion Velocities

    #region Vertical Values
    [Space, Header("Vertical Values", order = 0)]
    [Header("Jump Strenght", order = 1)]
    [SerializeField] float velocity_Vertical_Jump_Strenght;
    [SerializeField] float velocity_Vertical_Jump_Speed;
    bool velocity_Vertical_Jump_Done;

    [Header("Grounding")]
    [SerializeField] float velocity_Vertical_OnGround_Force;

    [Header("Falling Parameters")]
    [SerializeField] float velocity_Vertical_FallSpeed_Maximal;

    [Header("Time on Air")]
    float velocity_Vertical_Time_OnAir;

    #endregion Vertical Values

    #region Cooldowns
    [Space, Header("Cooldowns", order = 0)]
    [Header("Crouch", order = 1)]
    [SerializeField] float cooldown_Crouch_Time;
    [SerializeField] bool cooldown_Crouch_Done;

    [Header("Jump")]
    [SerializeField] float cooldown_Jump_Time;
    [SerializeField] bool cooldown_Jump_Done;
    #endregion Cooldowns

     #region Modifiers
    //Dynamic modifiers that are multiplying with values//
    [Space, Header("Modifires", order = 0)]
    [Header("Movement", order = 1)]
    [SerializeField] float modifier_Movement_Speed = 1;
    [SerializeField] float modifier_Jump_Strenght = 1;

    [Header("On Air")]
    [SerializeField] float modifier_Player_Mass = 1;
    #endregion Modifires

    #region Factors
    //Static movement modifiers//
    [Space, Header("Factors", order = 0)]
    [Header("Movement", order = 1)]
    [SerializeField] float factor_Movement_Sideways = 0.8f;
    [SerializeField] float factor_Movement_Backward = 0.5f;

    [Header("OnAir")]
    [SerializeField] float factor_Movement_OnAir_Momentum = 0.8f; //
    #endregion Factors

    #region Inputs
    //Player inputs//

    [Space, Header("Inputs", order = 0)]
    [Header("Movement", order = 1)]
    [SerializeField] Vector2 input_Movement; //Input from movement input//
    bool input_movesBackward; //If player is moving backward//

    #endregion Inputs

    #region Round
    [Space, Header("Round Values", order = 0)]

    [Header("Horizontal Vector", order = 1)]
    [SerializeField] float rounding_HorizontalMovement = 0.02f;

    [Header("Jumping")]
    [SerializeField] float rounding_Vertical_Jump = 0.02f;

    [Header("After Landing")]
    [SerializeField] float rounding_Horizontal_AfterLanding = 0.05f;

    #endregion Round

    #region Movement Values
    //Movement Calculation Values//
    [Space, Header("Movement Values", order = 0)]

    //Horizontal Movement//
    [Header("Horizontal", order = 1)]
    [SerializeField] Vector2 movement_Horizontal_Target;
    Vector2 movement_Horizontal_Calculation;
    [SerializeField] Vector2 movement_Horizontal_FinalVector;

    [Header("Horizontal - On Air")]
    [SerializeField] Vector2 movement_Horizontal_OnAir_Momentum_Saved_Vector;
    [SerializeField] Vector2 movement_Horizontal_OnAir_Momentum_Additional_Vector;
    public Vector2 movement_Horizontal_OnAir_Momentum_Additional_Vector_Target;
    [SerializeField] bool movement_Horizontal_OnAir_Momentum_Saved;
    [Space]
    [SerializeField] Vector2 movement_Horizontal_OnAir_Momentum_AfterLanding;
    Vector2 movement_Horizontal_OnAir_Momentum_AfterLanding_Calculation;

    //Vertical Movement//
    [Header("Vertical")]
    public float movement_Vertical_Velocity;
    float movement_Vertical_Calculation;

    //Complete Move Vector//
    [Header("Movement Vector")]
    [SerializeField] Vector3 movement_Vector;

    #endregion Movement Values

    #endregion Variables

    void Update()
    {
        if (playerMain.playerGeneralState == Player_Main.PlayerGeneralStates.Default)
        {
            //Update inputs//
            Input_Controller();

            //Special functions, that need continues updates//
            Special_Functions();

            //Calculate Movement Vector//
            Movement_Calculation();

            //Execute Movement//
            Movement_Execution();
        }
        else
        {
            //Here Turn off function//

        }

    }

    #region Movement Calculation
    void Movement_Calculation()
    {
        //Horizontal Calculation//
        Movement_Calculation_Horizontal();

        //Vertical Calculation//
        Movement_Calculation_Vertical();
    }

    #region Horizontal Calculation
    //Calculate horizontal movement values//
    void Movement_Calculation_Horizontal()
    {
        switch (playerMain.playerDefaultMovementState)
        {
            case Player_Main.PlayerDefaultMovementStates.Idle:
                Movement_Calculation_Horizontal_Stop();
                return;

            case Player_Main.PlayerDefaultMovementStates.Walking:
                Movement_Calculation_Horizontal_Default(velocity_Walk, velocity_Walk_Acceleration);
                return;

            case Player_Main.PlayerDefaultMovementStates.Running:
                Movement_Calculation_Horizontal_Default(velocity_Run, velocity_Run_Acceleration);
                return;

            case Player_Main.PlayerDefaultMovementStates.Crouching:
                Movement_Calculation_Horizontal_Crouch();
                return;

            case Player_Main.PlayerDefaultMovementStates.OnAir:
                Movement_Calculation_Horizontal_OnAir();
                return;

            case Player_Main.PlayerDefaultMovementStates.Disabled:

                return;
        }

    }

    void Movement_Calculation_Horizontal_Crouch()
    {
        switch (playerMain.playerDefaultCrouchState)
        {
            case Player_Main.PlayerDefaultCrouchStates.Crouching_Idle:
                Movement_Calculation_Horizontal_Stop();
                return;


            case Player_Main.PlayerDefaultCrouchStates.Crouching_Walk:
                Movement_Calculation_Horizontal_Default(velocity_Crouch, velocity_Crouch_Acceleration);
                return;
        }

    }

    void Movement_Calculation_Horizontal_Stop()
    {
        //Set Target to 0//
        movement_Horizontal_Target = Vector2.zero;

        //Deceleration//
        movement_Horizontal_FinalVector = Vector2.SmoothDamp(movement_Horizontal_FinalVector, Vector2.zero, ref movement_Horizontal_Calculation, velocity_Stop_Deceleration);

        Rounding_Movement_Calculation_Horizontal();
    }

    void Movement_Calculation_Horizontal_Default(float moveVelocity, float accelerationSpeed)
    {
        #region Calculate Target

        #region Y-Axis
        // Y-Axis Target Calculation //
        if (input_movesBackward)
        {
            movement_Horizontal_Target.y = input_Movement.y * moveVelocity * factor_Movement_Backward;
        }
        else
        {
            movement_Horizontal_Target.y = input_Movement.y * moveVelocity;
        }
        #endregion Y-Axis

        #region X-Axis
        // X-Axis Target Calculation //
        movement_Horizontal_Target.x = input_Movement.x * moveVelocity * factor_Movement_Sideways;
        #endregion X-Axis

        #region Apply Modifiers

        //Apply modifiers//
        movement_Horizontal_Target *= modifier_Movement_Speed;

        #endregion Apply Modifiers

        #endregion Calculate Target

        #region Smooth Accelerate Speeds

        if (input_Movement.y != 0)
        {
            //Smooth Transition for Y-Axis//
            movement_Horizontal_FinalVector.y = Mathf.SmoothDamp(movement_Horizontal_FinalVector.y, movement_Horizontal_Target.y, ref movement_Horizontal_Calculation.y, accelerationSpeed);
        }

        if (input_Movement.x != 0)
        {
            //Smooth Transition for X-Axis//
            movement_Horizontal_FinalVector.x = Mathf.SmoothDamp(movement_Horizontal_FinalVector.x, movement_Horizontal_Target.x, ref movement_Horizontal_Calculation.x, accelerationSpeed);
        }

        #endregion Smooth Accelerate Speeds

        Movement_Calculation_Horizontal_OneAxis_Stop();

        Rounding_Movement_Calculation_Horizontal();
    }

    void Movement_Calculation_Horizontal_OnAir()
    {
        //do adjustments only when player want it//
        if (playerMain.enteredInput_Movement)
        {
            #region Calculate Target

            // Y-Axis Target Calculation //
            Vector2 RawTarget = new Vector2(input_Movement.x * velocity_OnAir, input_Movement.y * velocity_OnAir);

            //Target To Vector3 + Rotation//
            Vector3 movement_Horizontal_OnAir_Momentum_Additional_Vector_Target_Rotated = transform.rotation * new Vector3(RawTarget.x, 0, RawTarget.y);

            //Convert Back to Vector2//
            movement_Horizontal_OnAir_Momentum_Additional_Vector_Target = new Vector2(movement_Horizontal_OnAir_Momentum_Additional_Vector_Target_Rotated.x, movement_Horizontal_OnAir_Momentum_Additional_Vector_Target_Rotated.z);

            #endregion Calculate Target

            movement_Horizontal_OnAir_Momentum_Additional_Vector = Vector2.SmoothDamp(movement_Horizontal_OnAir_Momentum_Additional_Vector, movement_Horizontal_OnAir_Momentum_Additional_Vector_Target, ref movement_Horizontal_Calculation, velocity_OnAir_Acceleration);
        }
    }

    void Movement_Calculation_Horizontal_OneAxis_Stop()
    {
        if (playerMain.enteredInput_Movement)
        {
            #region Axis-Y

            if (input_Movement.y == 0 && movement_Horizontal_FinalVector.y != 0)
            {
                movement_Horizontal_FinalVector.y = Mathf.SmoothDamp(movement_Horizontal_FinalVector.y, 0, ref movement_Horizontal_Calculation.y, velocity_Stop_OneAxis_Deceleration);
            }

            #endregion Axis-Y


            #region Axis-X

            if (input_Movement.x == 0 && movement_Horizontal_FinalVector.x != 0)
            {
                movement_Horizontal_FinalVector.x = Mathf.SmoothDamp(movement_Horizontal_FinalVector.x, 0, ref movement_Horizontal_Calculation.x, velocity_Stop_OneAxis_Deceleration);
            }

            #endregion Axis-X

        }


    }

    #endregion Horizontal Calculation

    #region Vertical Calculation
    void Movement_Calculation_Vertical()
    {
        switch (playerMain.playerDefaultOnAirState)
        {
            case Player_Main.PlayerDefaultOnAirStates.Grounded:
                Special_OnGround();
                Movement_Calculation_Vertical_Grounded();
                return;

            case Player_Main.PlayerDefaultOnAirStates.Jumping:
                Special_OnAir();
                Movement_Calculation_Vertical_Jump();
                return;

            case Player_Main.PlayerDefaultOnAirStates.OnAir:
                Special_OnAir();
                Movement_Calculation_Vertical_OnAir();
                return;

            case Player_Main.PlayerDefaultOnAirStates.OnAir_Falling:
                Special_OnAir();
                Movement_Calculation_Vertical_OnAir_Falling();
                return;
        }
    }

    void Movement_Calculation_Vertical_Jump()
    {
        if (movement_Vertical_Velocity == velocity_Vertical_Jump_Strenght)
        {
            //When jump strenght is reached, reset state//
            playerMain.isJumping = false;
            velocity_Vertical_Jump_Done = false;
        }
        else
        {
            //Reset Vertical velocity//
            if (!velocity_Vertical_Jump_Done)
            {
                //Reset Vertical Velocity before transiton//
                movement_Vertical_Velocity = 0;
                //Save that reset was done//
                velocity_Vertical_Jump_Done = true;
            }

            //
            if (movement_Vertical_Velocity != velocity_Vertical_Jump_Strenght)
            {
                movement_Vertical_Velocity = Mathf.SmoothDamp(movement_Vertical_Velocity, velocity_Vertical_Jump_Strenght * modifier_Jump_Strenght, ref movement_Vertical_Calculation, velocity_Vertical_Jump_Speed);
            }


            //Try to round value//
            Rounding_Movement_Vertical_Jump();
        }
    }

    void Movement_Calculation_Vertical_OnAir()
    {
        //Just Apply gravity//
        movement_Vertical_Velocity += Physics.gravity.y * modifier_Player_Mass * Time.deltaTime;
    }

    void Movement_Calculation_Vertical_OnAir_Falling()
    {
        if (movement_Vertical_Velocity != velocity_Vertical_FallSpeed_Maximal)
        {
            if (movement_Vertical_Velocity > velocity_Vertical_FallSpeed_Maximal)
            {
                //Just Apply gravity//
                movement_Vertical_Velocity += Physics.gravity.y * modifier_Player_Mass * Time.deltaTime;
            }
            else
            {
                movement_Vertical_Velocity = velocity_Vertical_FallSpeed_Maximal;
            }
        }
    }

    void Movement_Calculation_Vertical_Grounded()
    {
        if (movement_Vertical_Velocity != velocity_Vertical_OnGround_Force)
        {
            movement_Vertical_Velocity = velocity_Vertical_OnGround_Force;
        }
    }

    #endregion Vertical Calculation

    #endregion Movement Calculation

    #region Movement Execution
    void Movement_Execution()
    {
        if (playerMain.playerDefaultMovementState == Player_Main.PlayerDefaultMovementStates.SlidingFromEdge)
        {
            Debug.Log("Sliiiiiiiiiiiide");
        }
        else if (playerMain.playerDefaultOnAirState == Player_Main.PlayerDefaultOnAirStates.Grounded)
        {
            if (playerMain.movement_Momentum_Decelerate_AfterLanding)
            {
                Movement_Execution_Default_AfterLanding();
            }
            else
            {
                Movement_Execution_Default();
            }
        }
        else
        {
            Movement_Execution_OnAir();
        }
    }

    //Default Movement Execution//
    void Movement_Execution_Default()
    {
        #region Assembly Final Vector
        movement_Vector = new Vector3(movement_Horizontal_FinalVector.x, movement_Vertical_Velocity, movement_Horizontal_FinalVector.y);
        #endregion Assemby Final Vector

        playerController.Move(transform.rotation * movement_Vector * Time.deltaTime);
    }

    void Movement_Execution_Default_AfterLanding()
    {
        #region Assembly Final Vector
        //Assembly main Vector//
        movement_Vector = new Vector3(movement_Horizontal_FinalVector.x, movement_Vertical_Velocity, movement_Horizontal_FinalVector.y);

        //Decelerate//
        movement_Horizontal_OnAir_Momentum_AfterLanding = Vector2.SmoothDamp(movement_Horizontal_OnAir_Momentum_AfterLanding, Vector2.zero, ref movement_Horizontal_OnAir_Momentum_AfterLanding_Calculation, velocity_Stop_Landing_Momentum_Deceleration);

        //Try to round value//
        Rounding_Movement_Execution_AfterLanding();

        //After Rounding Check of its 0, when it is, then reset case//
        if (movement_Horizontal_OnAir_Momentum_AfterLanding == Vector2.zero)
        {
            playerMain.movement_Momentum_Decelerate_AfterLanding = false;
        }

        Vector3 movement_Horizontal_OnAir_Momentum_AfterLanding_Vector3 = new Vector3(movement_Horizontal_OnAir_Momentum_AfterLanding.x, 0, movement_Horizontal_OnAir_Momentum_AfterLanding.y);

        #endregion Assemby Final Vector

        //Execute move//
        playerController.Move(((transform.rotation * movement_Vector) + movement_Horizontal_OnAir_Momentum_AfterLanding_Vector3) * Time.deltaTime);
    }

    void Movement_Execution_OnAir()
    {
        #region Assembly Final Vector
        movement_Vector = new Vector3(movement_Horizontal_OnAir_Momentum_Saved_Vector.x + movement_Horizontal_OnAir_Momentum_Additional_Vector.x, movement_Vertical_Velocity, movement_Horizontal_OnAir_Momentum_Saved_Vector.y + movement_Horizontal_OnAir_Momentum_Additional_Vector.y);
        #endregion Assembly Final Vector

        playerController.Move(movement_Vector * Time.deltaTime);
    }



    #endregion Movement Execution

    #region Cooldowns

    #region Crouch
    public void Cooldown_Crouch()
    {
        //Start Cooldown//
        cooldown_Crouch_Done = false;

        //Start Restart//
        StartCoroutine(Cooldown_Crouch_Reset());
    }

    IEnumerator Cooldown_Crouch_Reset()
    {
        //Wait until reset//
        yield return new WaitForSeconds(cooldown_Crouch_Time);

        //Reset Cooldown//
        cooldown_Crouch_Done = true;
    }

    #endregion Crouch

    #region Jump

    public void Cooldown_Jump()
    {
        //Start Cooldown//
        cooldown_Jump_Done = false;
    }

    IEnumerator Cooldown_Jump_Reset()
    {
        //Wait until reset//
        yield return new WaitForSeconds(cooldown_Jump_Time);

        //Reset Cooldown//
        cooldown_Jump_Done = true;
    }

    #endregion Jump

    #endregion Cooldowns

    #region Special Functions
    void Special_Functions()
    {
        Special_Moving_Check(); //Updates if player is moving//
    }

    void Special_OnGround()
    {
        Special_Landing_Grounding(); //Checks if landing will be executed//

        Special_Movement_OnAir_Momentum_Reset(); //Reset On Air momentum//

        Special_Time_OnAir_Reset(); //Reset time on air//
    }

    void Special_OnAir()
    {
        Special_Landing_OnAir(); //Reset landing state//

        Special_Movement_After_Landing_Momentum_Check(); //Checks if momentum will be executed after landing//

        Special_Movement_OnAir_Momentum_Save(); //save momentum onair//

        Special_Time_OnAir_Count(); //Count time on air//
    }

    #region Landing
    void Special_Landing_Grounding()
    {
        //When player not landed on the ground//
        if (!playerMain.playerLanded)
        {
            //Do it once, while landing//
            Special_Movement_After_Landing_Momentum_Reset(); //Update momentum vector, andresets case//

            //Set that player landed//
            playerMain.playerLanded = true;
        }
    }

    //When player is on air
    void Special_Landing_OnAir()
    {
        if (playerMain.playerLanded)
        {
            //Reset That player landed on the ground//
            playerMain.playerLanded = false;
        }
    }
    #endregion Landing

    #region OnAir - Save Momentum
    void Special_Movement_OnAir_Momentum_Save()
    {
        //When momentum is not saved, save//
        if (!movement_Horizontal_OnAir_Momentum_Saved)
        {
            //Calculate velocity//
            Vector3 momentumVelocityWithRotation = transform.rotation * new Vector3(movement_Horizontal_FinalVector.x, 0, movement_Horizontal_FinalVector.y) * factor_Movement_OnAir_Momentum;

            //Convert back to Vector2//
            movement_Horizontal_OnAir_Momentum_Saved_Vector = new Vector2(momentumVelocityWithRotation.x, momentumVelocityWithRotation.z);

            //When on air reset saved After landing momentum// 
            movement_Horizontal_OnAir_Momentum_AfterLanding = Vector2.zero;

            //Tell that is saved//
            movement_Horizontal_OnAir_Momentum_Saved = true;

            //When on air, reset case//
            playerMain.movement_Momentum_Decelerate_AfterLanding = false;
        }
    }

    void Special_Movement_OnAir_Momentum_Reset()
    {
        if (movement_Horizontal_OnAir_Momentum_Saved)
        {
            //Reset starting momentum//
            movement_Horizontal_OnAir_Momentum_Saved_Vector = Vector2.zero;

            //Reset additional force//
            movement_Horizontal_OnAir_Momentum_Additional_Vector = Vector2.zero;
            movement_Horizontal_OnAir_Momentum_Additional_Vector_Target = Vector2.zero;

            movement_Horizontal_OnAir_Momentum_Saved = false;
        }
    }

    #endregion OnAir - Save Momentum

    #region OnAir - Time

    void Special_Time_OnAir_Count()
    {
        velocity_Vertical_Time_OnAir += Time.deltaTime;
    }

    void Special_Time_OnAir_Reset()
    {
        if (velocity_Vertical_Time_OnAir != 0)
        {
            velocity_Vertical_Time_OnAir = 0;
        }
    }


    #endregion OnAir - Time

    #region Decelerate momentum controller
    void Special_Movement_After_Landing_Momentum_Check()
    {
        if (!playerMain.movement_Momentum_Decelerate_AfterLanding)
        {
            if (velocity_Vertical_Time_OnAir >= velocity_Time_Landing_Momentum_Execution)
            {
                playerMain.movement_Momentum_Decelerate_AfterLanding = true;
            }
        }
    }

    void Special_Movement_After_Landing_Momentum_Reset()
    {
        velocity_Vertical_Time_OnAir = 0;

        if (playerMain.movement_Momentum_Decelerate_AfterLanding)
        {
            //When cas is active, reset speeds//
            movement_Horizontal_FinalVector = Vector2.zero;

            //Reset remaining vector//
            movement_Vector = new Vector3(0, movement_Vertical_Velocity, 0);

            //Update After landing momentum//
            movement_Horizontal_OnAir_Momentum_AfterLanding = new Vector2(movement_Horizontal_OnAir_Momentum_Saved_Vector.x + movement_Horizontal_OnAir_Momentum_Additional_Vector.x, movement_Horizontal_OnAir_Momentum_Saved_Vector.y + movement_Horizontal_OnAir_Momentum_Additional_Vector.y);
        }
    }
    #endregion

    void Special_Moving_Check()
    {
        if (playerMain.isChangingPosition && playerMain.enteredInput_Movement)
        {
            playerMain.isMoving = true;
        }
        else
        {
            playerMain.isMoving = false;
        }
    }

    #endregion Special Functions

    #region External Functions
    #endregion External Functions

    #region Rounding
    void Rounding_Movement_Calculation_Horizontal()
    {
        //Round Horizontal Movement Values to full target value//
        #region Y-Axis
        if (movement_Horizontal_FinalVector.y != movement_Horizontal_Target.y)
        {
            if ((movement_Horizontal_FinalVector.y < movement_Horizontal_Target.y && movement_Horizontal_FinalVector.y > movement_Horizontal_Target.y - rounding_HorizontalMovement) || (movement_Horizontal_FinalVector.y > movement_Horizontal_Target.y && movement_Horizontal_FinalVector.y < movement_Horizontal_Target.y + rounding_HorizontalMovement))
            {
                movement_Horizontal_FinalVector.y = movement_Horizontal_Target.y;
            }
        }
        #endregion Y-Axis

        #region X-Axis
        if (movement_Horizontal_FinalVector.x != movement_Horizontal_Target.x)
        {
            if ((movement_Horizontal_FinalVector.x < movement_Horizontal_Target.x && movement_Horizontal_FinalVector.x > movement_Horizontal_Target.x - rounding_HorizontalMovement) || (movement_Horizontal_FinalVector.x > movement_Horizontal_Target.x && movement_Horizontal_FinalVector.x < movement_Horizontal_Target.x + rounding_HorizontalMovement))
            {
                movement_Horizontal_FinalVector.x = movement_Horizontal_Target.x;
            }
        }
        #endregion X-Axis
    }

    void Rounding_Movement_Vertical_Jump()
    {
        if ((movement_Vertical_Velocity < velocity_Vertical_Jump_Strenght && movement_Vertical_Velocity > velocity_Vertical_Jump_Strenght - rounding_Vertical_Jump) || (movement_Vertical_Velocity > velocity_Vertical_Jump_Strenght && movement_Vertical_Velocity < velocity_Vertical_Jump_Strenght + rounding_Vertical_Jump))
        {
            movement_Vertical_Velocity = velocity_Vertical_Jump_Strenght;
        }
    }

    void Rounding_Movement_Execution_AfterLanding()
    {
        if (movement_Horizontal_OnAir_Momentum_AfterLanding.y != 0)
        {
            if ((movement_Horizontal_OnAir_Momentum_AfterLanding.y > 0 && movement_Horizontal_OnAir_Momentum_AfterLanding.y < rounding_Horizontal_AfterLanding) || (movement_Horizontal_OnAir_Momentum_AfterLanding.y < 0 && movement_Horizontal_OnAir_Momentum_AfterLanding.y > -rounding_Horizontal_AfterLanding))
            {
                movement_Horizontal_OnAir_Momentum_AfterLanding.y = 0;
            }
        }

        if (movement_Horizontal_OnAir_Momentum_AfterLanding.x != 0)
        {
            if ((movement_Horizontal_OnAir_Momentum_AfterLanding.x > 0 && movement_Horizontal_OnAir_Momentum_AfterLanding.x < rounding_Horizontal_AfterLanding) || (movement_Horizontal_OnAir_Momentum_AfterLanding.x < 0 && movement_Horizontal_OnAir_Momentum_AfterLanding.x > -rounding_Horizontal_AfterLanding))
            {
                movement_Horizontal_OnAir_Momentum_AfterLanding.x = 0;
            }
        }

    }

    #endregion Rounding

    #region Input Functions
    void Input_Controller()
    {
        //Update movememnt//
        Input_Controller_Movement();
    }

    void Input_Controller_Movement()
    {
        //Update Movement Value//
        input_Movement = gameControls.Player.Movement.ReadValue<Vector2>();

        //Checks if entered input//
        if (input_Movement != Vector2.zero)
        {
            //Update input info//
            playerMain.enteredInput_Movement = true;

            //Check if player is moving back//
            if (input_Movement.y < 0)
            {
                input_movesBackward = true;
            }
            else
            {
                input_movesBackward = false;
            }
        }
        else
        {
            //Upsdate input info//
            playerMain.enteredInput_Movement = false;

            //Reset Back moving//
            if (input_movesBackward)
            {
                input_movesBackward = false;
            }
        }
    }

    void Input_Controller_Run(bool enteredInput)
    {
        if (enteredInput)
        {
            if (playerMain.canRun)
            {
                playerMain.enteredInput_Run = true;
            }
        }
        else
        {
            playerMain.enteredInput_Run = false;
        }
    }

    void Input_Controller_Crouch()
    {
        if (playerMain.canCrouch)
        {
            if (cooldown_Crouch_Done)
            {
                //Start Cooldown//
                Cooldown_Crouch();

                //Change State//
                playerMain.isCrouching = !playerMain.isCrouching;
            }
        }
        else
        {
            playerMain.isCrouching = false;
        }
    }

    void Input_Controller_Jump()
    {
        if (playerMain.canJump && cooldown_Jump_Done)
        {
            if (playerMain.playerDefaultOnAirState == Player_Main.PlayerDefaultOnAirStates.Grounded)
            {
                //start cooldown//
                Cooldown_Jump();

                playerMain.isJumping = true;

                //Delete after landing implementation//
                StartCoroutine(Cooldown_Jump_Reset());
            }
        }
    }

    #endregion Input Functions

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
