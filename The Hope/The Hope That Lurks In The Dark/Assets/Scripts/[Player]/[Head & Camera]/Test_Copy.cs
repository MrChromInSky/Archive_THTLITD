using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Copy : MonoBehaviour
{
    #region Assingments
    Player_Main playerMain;

    void Awake()
    {
        playerMain = GetComponentInParent<Player_Main>();
    }
    #endregion Assignments

    #region Variables

    #region Working Parameters
    [Space, Header("Stablisation Parameters", order = 0)]
    [Header("Maximal Angles", order = 1)]
    [SerializeField] float stabilisation_Angle_Maximal_Up = 75;
    [SerializeField] float stabilisation_Angle_Maximal_Down = 60;

    [Header("Minimal Distance")]
    [SerializeField] float stabilisation_Distance_Maximum = 80;
    [SerializeField] float stabilisation_Distance_Minimum = 10;
    #endregion Working Parameters

    #region Raycast

    [Space, Header("Raycast Parameters")]
    [SerializeField] float stabilisation_Ray_Lenght;
    [SerializeField] LayerMask stabilisation_Ray_Layers;
    RaycastHit stabilisation_Ray_Hit;

    #endregion Raycast

    #region Transition

    [Space, Header("Focus Point Tansition Parameters")]
    [SerializeField] float stabilisation_FocusPoint_Transition_Speed;
    Vector3 stabilisation_FocusPoint_Transition_Calculation;
    public float stabilisation_Reset_Time;

    #endregion Transition

    #region Rounding

    [Space, Header("Rounding")]
    [SerializeField] float rounding_FocusPoint_Position;

    #endregion Rounding

    #region Focus Points

    [Space, Header("Focus Point")]
    [SerializeField] Vector3 stabilisation_FocusPoint_Position;
    [Space]
    [SerializeField] Vector3 stabilisation_FocusPoint_Target;


    #endregion Focus Points

    #region Transforms

    [Space, Header("Head Transforms")]
    [SerializeField] Transform transform_Head_Base; //Base of the head//
    [SerializeField] Transform transform_Head_Pivot; //Rotation part//
    [SerializeField] Transform transform_CameraHolder; //Cameras//

    #endregion Transforms

    #region Debug
    [Space, Header("Debug")]
    [SerializeField] bool debug_Stabilisation;
    #endregion Debug

    #endregion Variables

    void Update()
    {
        if (playerMain.head_Stabilisation_Active)
        {
            Stabilisation_Controller();
            Stabilisation_Transition();
            Stabilisation_Execution();
        }
    }

    void Stabilisation_Controller()
    {
        //Actual head angle//
        float headAngle_X = transform_Head_Pivot.localRotation.eulerAngles.x;

        if (headAngle_X <= stabilisation_Angle_Maximal_Down || headAngle_X >= 360 - stabilisation_Angle_Maximal_Up)
        {
            Stabilisation_TargetPoint_Update();
        }
        else
        {
            Stabilisation_TargetPoint_Reset();
        }
    }

    #region Set Target
    void Stabilisation_TargetPoint_Update()
    {
        #region Raycast
        if (Physics.Raycast(transform_Head_Base.position, transform_Head_Pivot.forward, out stabilisation_Ray_Hit, stabilisation_Ray_Lenght, stabilisation_Ray_Layers))
        {
            if (stabilisation_Ray_Hit.distance >= stabilisation_Distance_Minimum && stabilisation_Ray_Hit.distance <= stabilisation_Distance_Maximum)
            {
                //Update Target//
                stabilisation_FocusPoint_Target = stabilisation_Ray_Hit.point;

            }
            else
            {
                Stabilisation_TargetPoint_Reset();
            }
        }
        else
        {
            Stabilisation_TargetPoint_Reset();
        }
        #endregion Raycast


        if (stabilisation_Reset_Time != 0)
        {
            stabilisation_Reset_Time = 0;
        }

    }

    void Stabilisation_TargetPoint_Reset()
    {
        stabilisation_FocusPoint_Position = transform_Head_Pivot.position + transform_Head_Pivot.forward * 30;

        if (transform_CameraHolder.localRotation != Quaternion.identity)
        {
            Debug.Log("Reset");

            transform_CameraHolder.localRotation = Quaternion.Slerp(transform_CameraHolder.localRotation, Quaternion.identity, stabilisation_Reset_Time);

            if (stabilisation_Reset_Time < 1)
            {
                stabilisation_Reset_Time += Time.deltaTime;
            }
            else
            {
                transform_CameraHolder.localRotation = Quaternion.identity;
            }
        }
    }

    #endregion Set Target

    void Stabilisation_Transition()
    {
        if (stabilisation_FocusPoint_Position != stabilisation_FocusPoint_Target)
        {
            //Transition between target and actual//
            stabilisation_FocusPoint_Position = Vector3.SmoothDamp(stabilisation_FocusPoint_Position, stabilisation_FocusPoint_Target, ref stabilisation_FocusPoint_Transition_Calculation, stabilisation_FocusPoint_Transition_Speed);

            //Try to round//
            if ((stabilisation_FocusPoint_Position.magnitude > stabilisation_FocusPoint_Target.magnitude && stabilisation_FocusPoint_Position.magnitude < stabilisation_FocusPoint_Target.magnitude + rounding_FocusPoint_Position) || (stabilisation_FocusPoint_Position.magnitude < stabilisation_FocusPoint_Target.magnitude && stabilisation_FocusPoint_Position.magnitude > stabilisation_FocusPoint_Target.magnitude - rounding_FocusPoint_Position))
            {
                stabilisation_FocusPoint_Position = stabilisation_FocusPoint_Target;
            }

        }
    }

    void Stabilisation_Execution()
    {
        //Look at//
        transform_CameraHolder.LookAt(stabilisation_FocusPoint_Position);
    }

    #region Debug
    void OnDrawGizmos()
    {
        if (debug_Stabilisation)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform_Head_Pivot.position, stabilisation_FocusPoint_Position);
        }
    }
    #endregion Debug
}
