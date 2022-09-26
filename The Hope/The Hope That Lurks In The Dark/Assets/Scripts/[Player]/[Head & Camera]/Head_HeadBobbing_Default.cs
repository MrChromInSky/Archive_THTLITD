using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_HeadBobbing_Default : MonoBehaviour
{
    #region Assignments
    Player_Main playerMain;
    Player_Sound_FootstepsController FootstepsController;

    void Awake()
    {
        playerMain = GetComponentInParent<Player_Main>();
        FootstepsController = GameObject.FindGameObjectWithTag("Player_SoundController").GetComponent<Player_Sound_FootstepsController>();
    }
    #endregion Assignemnts

    #region Variables

    #region Frequencies
    [Header("Frequencies", order = 0)]
    [Header("Idle", order = 1)]
    [SerializeField] float headbobbing_Frequency_Idle;
    [Header("Walk")]
    [SerializeField] float headbobbing_Frequency_Walk;
    [Header("Run")]
    [SerializeField] float headbobbing_Frequency_Run;
    [Header("Crouch - Idle")]
    [SerializeField] float headbobbing_Frequency_Crouch_Idle;
    [Header("Crouch - Walk")]
    [SerializeField] float headbobbing_Frequency_Crouch_Walk;
    #endregion Frequencies

    #region Ranges
    [Space, Header("Ranges", order = 0)]
    [Header("Idle", order = 1)]
    [SerializeField] float headbobbing_Range_Horizontal_Idle;
    [SerializeField] float headbobbing_Range_Vertical_Idle;
    [Header("Walk")]
    [SerializeField] float headbobbing_Range_Horizontal_Walk;
    [SerializeField] float headbobbing_Range_Vertical_Walk;
    [Header("Run")]
    [SerializeField] float headbobbing_Range_Horizontal_Run;
    [SerializeField] float headbobbing_Range_Vertical_Run;
    [Header("Crouch - Idle")]
    [SerializeField] float headbobbing_Range_Horizontal_Crouch_Idle;
    [SerializeField] float headbobbing_Range_Vertical_Crouch_Idle;
    [Header("Crouch - Walk")]
    [SerializeField] float headbobbing_Range_Horizontal_Crouch_Walk;
    [SerializeField] float headbobbing_Range_Vertical_Crouch_Walk;
    #endregion Ranges

    #region Transitions
    [Space, Header("Range Value Transition Speed")]
    [SerializeField] float headbobbing_Range_Transition_Speed;

    [Header("Frequency Value Transition Speed")]
    [SerializeField] float headbobbing_Frequency_Transition_Speed;
    float headbobbing_Frequency_Transition_Calculation;

    [Header("Camera Reset Transition")]
    [SerializeField] float headbobbing_Position_Reset_Speed;
    Vector3 headbobbing_Position_Reset_Calculation;

    [Header("Head movement transition smoothing")]
    [SerializeField] float headbobbing_Head_Position_Smoothing_Speed;
    Vector3 headbobbing_Head_Position_Smoothing_Calculation;

    #endregion Transitions

    #region Rounding 
    [Space, Header("Rounding", order = 0)]
    [Header("General", order = 1)]
    [SerializeField] float headbobbing_Round_General;
    #endregion Rounding

    #region Calculation Values
    [Space, Header("Calculation Values", order = 0)]
    [Header("Sinus Timers", order = 1)]
    [SerializeField] float headbobbing_Sinus_Timer;
    [SerializeField] float footsteps_Sinus_Timer;
    [SerializeField] float headbobbing_Frequency;
    float headbobbing_Frequency_Calculation;

    [Space, Header("Position - Range - Target")]
    [SerializeField] float headbobbing_Range_Horizontal_Target;
    [SerializeField] float headbobbing_Range_Vertical_Target;
    Vector2 headbobbing_Range_Calculation;

    [Header("Position - Range")]
    [SerializeField] float headbobbing_Range_Horizontal;
    [SerializeField] float headbobbing_Range_Vertical;

    [Header("Execute Position")]
    [SerializeField] Vector3 headbobbing_Head_Position_New;


    #endregion Calculation Values

    #region Game Objects
    [Space, Header("Player Objects", order = 0)]
    [Header("Player Head", order = 1)]
    [SerializeField] GameObject headbobbing_Object_Player_Head;
    [SerializeField] Vector3 headbobbing_Object_Player_Head_StartPosition;
    #endregion Objects

    #endregion Variables

    void Start()
    {
        //Set start position of camera//
        headbobbing_Object_Player_Head_StartPosition = headbobbing_Object_Player_Head.transform.localPosition;
    }

    void Update()
    {
        if (playerMain.headBobbing_Active)
        {
            if (playerMain.playerGeneralState == Player_Main.PlayerGeneralStates.Default)
            {
                Headbobbing_Controller();
                Footsteps_Controller();
                Headbobbing_Execution();
            }
            else
            {
                Headbobbing_Calculation_Timer_Reset();
                Headbobbing_Reset();
            }
        }
        else
        {
            Headbobbing_Calculation_Timer_Reset();
            Headbobbing_Reset();
        }
    }

    void Headbobbing_Controller()
    {
        Headbobbing_Calculation_Controller();
    }

    #region Calculations

    #region State Controller
    void Headbobbing_Calculation_Controller()
    {
        switch (playerMain.playerDefaultMovementState)
        {
            case Player_Main.PlayerDefaultMovementStates.Idle:
                Headbobbing_Calculation_Timer(headbobbing_Frequency_Idle);
                Headbobbing_Calculation_Range(headbobbing_Range_Vertical_Idle, headbobbing_Range_Horizontal_Idle);
                return;

            case Player_Main.PlayerDefaultMovementStates.Walking:
                Headbobbing_Calculation_Timer(headbobbing_Frequency_Walk);
                Headbobbing_Calculation_Range(headbobbing_Range_Vertical_Walk, headbobbing_Range_Horizontal_Walk);

                return;

            case Player_Main.PlayerDefaultMovementStates.Running:
                Headbobbing_Calculation_Timer(headbobbing_Frequency_Run);
                Headbobbing_Calculation_Range(headbobbing_Range_Vertical_Run, headbobbing_Range_Horizontal_Run);

                return;

            case Player_Main.PlayerDefaultMovementStates.Crouching:
                Headbobbing_Calculation_Controller_Crouch();
                return;

            default:

                return;
        }
    }

    void Headbobbing_Calculation_Controller_Crouch()
    {
        switch (playerMain.playerDefaultCrouchState)
        {
            case Player_Main.PlayerDefaultCrouchStates.Crouching_Idle:
                Headbobbing_Calculation_Timer(headbobbing_Frequency_Crouch_Idle);
                Headbobbing_Calculation_Range(headbobbing_Range_Vertical_Crouch_Idle, headbobbing_Range_Horizontal_Crouch_Idle);
                return;

            case Player_Main.PlayerDefaultCrouchStates.Crouching_Walk:
                Headbobbing_Calculation_Timer(headbobbing_Frequency_Crouch_Walk);
                Headbobbing_Calculation_Range(headbobbing_Range_Vertical_Crouch_Walk, headbobbing_Range_Horizontal_Crouch_Walk);
                return;
        }
    }
    #endregion State Controller

    #region Timers

    void Headbobbing_Calculation_Timer(float frequency)
    {
        float headbobbing_Rest = 0;

        #region Frequency Smooting

        if (headbobbing_Frequency != frequency)
        {
            //Smooh transition//
            headbobbing_Frequency = Mathf.SmoothDamp(headbobbing_Frequency, frequency, ref headbobbing_Frequency_Calculation, headbobbing_Frequency_Transition_Speed);

            //Rounding//
            if ((headbobbing_Frequency > frequency && headbobbing_Frequency < frequency + 0.02f) || (headbobbing_Frequency < frequency && headbobbing_Frequency > frequency - 0.02f))
            {
                headbobbing_Frequency = frequency;
            }
        }
        #endregion Frequency smoothing

        #region Add Time
        //Headbobbing//
        headbobbing_Sinus_Timer += Time.deltaTime * headbobbing_Frequency;

        #endregion Add Time

        #region Check if loop is done
        //Headbobbing//
        if (headbobbing_Sinus_Timer >= (2 * Mathf.PI))
        {
            //Keep the rest for sync//
            headbobbing_Rest = headbobbing_Sinus_Timer - (2 * Mathf.PI);
            headbobbing_Sinus_Timer = headbobbing_Rest;

        }


        #endregion Check if loop is done
    }

    //Resets timers in some cases//
    void Headbobbing_Calculation_Timer_Reset()
    {
        if (headbobbing_Sinus_Timer != 0 || footsteps_Sinus_Timer != 0)
        {
            //Reset Timers//
            headbobbing_Sinus_Timer = 0;
            footsteps_Sinus_Timer = 0;
        }
    }

    #endregion Timers

    #region Range

    void Headbobbing_Calculation_Range(float rangeVertical, float rangeHorizontal)
    {
        #region Set Target
        //Vertical//
        if (rangeVertical != headbobbing_Range_Vertical_Target)
        {
            headbobbing_Range_Vertical_Target = rangeVertical;
        }
        //Horizontal//
        if (rangeHorizontal != headbobbing_Range_Horizontal_Target)
        {
            headbobbing_Range_Horizontal_Target = rangeHorizontal;
        }

        #endregion Set Target

        #region Smooth Transition
        if (headbobbing_Range_Vertical_Target != headbobbing_Range_Vertical)
        {
            //Smooth damp//
            headbobbing_Range_Vertical = Mathf.SmoothDamp(headbobbing_Range_Vertical, headbobbing_Range_Vertical_Target, ref headbobbing_Range_Calculation.y, headbobbing_Range_Transition_Speed);

            //Rounding//
            Round_Headbobbing_Range();
        }

        if (headbobbing_Range_Horizontal_Target != headbobbing_Range_Horizontal)
        {
            //Smooth damp//
            headbobbing_Range_Horizontal = Mathf.SmoothDamp(headbobbing_Range_Horizontal, headbobbing_Range_Horizontal_Target, ref headbobbing_Range_Calculation.x, headbobbing_Range_Transition_Speed);

            //Rounding//
            Round_Headbobbing_Range();
        }

        #endregion Smooth Transition 
    }


    #endregion Range

    #endregion Calculations

    #region Execution

    void Headbobbing_Execution()
    {
        #region Calculate Final Sinuses
        //Vertical//
        float headbobbing_Sinus_Vertical = Mathf.Abs(Mathf.Sin(headbobbing_Sinus_Timer) * headbobbing_Range_Vertical);
        //Horizontal//
        float headbobbing_Sinus_Horizontal = Mathf.Sin(headbobbing_Sinus_Timer) * headbobbing_Range_Horizontal;
        //Complee Vector//
        headbobbing_Head_Position_New = new Vector3(headbobbing_Sinus_Horizontal, headbobbing_Sinus_Vertical, 0);
        #endregion Calculate Final Sinuses

        #region Set New Position

        Vector3 headbobbing_Head_Position_New_Final = headbobbing_Object_Player_Head_StartPosition + headbobbing_Head_Position_New;

        #endregion Set New Position

        #region Change position
        if (headbobbing_Object_Player_Head.transform.localPosition != headbobbing_Head_Position_New_Final)
        {
            //Smooth dump to target//
            headbobbing_Object_Player_Head.transform.localPosition = Vector3.SmoothDamp(headbobbing_Object_Player_Head.transform.localPosition, headbobbing_Head_Position_New_Final, ref headbobbing_Head_Position_Smoothing_Calculation, headbobbing_Head_Position_Smoothing_Speed);
        }
        #endregion Change position
    }

    void Headbobbing_Reset()
    {
        //When want to reset, check if head is not already in position//
        if (headbobbing_Object_Player_Head.transform.localPosition != headbobbing_Object_Player_Head_StartPosition)
        {
            headbobbing_Object_Player_Head.transform.localPosition = Vector3.SmoothDamp(headbobbing_Object_Player_Head.transform.localPosition, headbobbing_Object_Player_Head_StartPosition, ref headbobbing_Position_Reset_Calculation, headbobbing_Position_Reset_Speed);


            //Rounding Function//
            if ((headbobbing_Object_Player_Head.transform.localPosition.magnitude > headbobbing_Object_Player_Head_StartPosition.magnitude && headbobbing_Object_Player_Head.transform.localPosition.magnitude < headbobbing_Object_Player_Head_StartPosition.magnitude + 0.02f) || (headbobbing_Object_Player_Head.transform.localPosition.magnitude < headbobbing_Object_Player_Head_StartPosition.magnitude && headbobbing_Object_Player_Head.transform.localPosition.magnitude > headbobbing_Object_Player_Head_StartPosition.magnitude - 0.02f))
            {
                headbobbing_Object_Player_Head.transform.localPosition = headbobbing_Object_Player_Head_StartPosition;
            }
        }
    }

    #endregion Execution

    #region Rounding
    void Round_Headbobbing_Range()
    {
        #region Horizontal
        if (headbobbing_Range_Horizontal != headbobbing_Range_Horizontal_Target)
        {
            if ((headbobbing_Range_Horizontal > headbobbing_Range_Horizontal_Target && headbobbing_Range_Horizontal < headbobbing_Range_Horizontal_Target - headbobbing_Round_General) || (headbobbing_Range_Horizontal < headbobbing_Range_Horizontal_Target && headbobbing_Range_Horizontal > headbobbing_Range_Horizontal_Target + headbobbing_Round_General))
            {
                headbobbing_Range_Horizontal = headbobbing_Range_Horizontal_Target;
            }
        }
        #endregion Horizontal

        #region Vertical
        if (headbobbing_Range_Vertical != headbobbing_Range_Vertical_Target)
        {
            if ((headbobbing_Range_Vertical > headbobbing_Range_Vertical_Target && headbobbing_Range_Vertical < headbobbing_Range_Vertical_Target - headbobbing_Round_General) || (headbobbing_Range_Vertical < headbobbing_Range_Vertical_Target && headbobbing_Range_Vertical > headbobbing_Range_Vertical_Target + headbobbing_Round_General))
            {
                headbobbing_Range_Vertical = headbobbing_Range_Vertical_Target;
            }
        }
        #endregion Vertical
    }

    #endregion Rounding

    #region Footsteps Sound
    void Footsteps_Controller()
    {
        //Time Rest after loop//
        float footsteps_Rest = 0;

        //Add Time to timer//
        footsteps_Sinus_Timer += Time.deltaTime * headbobbing_Frequency;

        //Footsteps//
        if (footsteps_Sinus_Timer >= Mathf.PI)
        {
            #region reset
            //Keep the rest for sync//
            footsteps_Rest = footsteps_Sinus_Timer - Mathf.PI;
            footsteps_Sinus_Timer = footsteps_Rest;
            #endregion reset

            //Execute footsteps//
            Footsteps_Controller_Execute();
        }
    }

    void Footsteps_Controller_Execute()
    {
        FootstepsController.Footsteps_Play();
    }

    #endregion Footsteps Sound

}
