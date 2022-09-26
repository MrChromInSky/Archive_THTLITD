using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_Camera_Stabilisation : MonoBehaviour
{
    #region Assingments
    Player_Main playerMain;

    void Awake()
    {
        playerMain = GetComponentInParent<Player_Main>();
    }
    #endregion Assignments

    #region Variables

    #region Stabilisation Parameters

    [Space, Header("Stabilisation Parameters", order = 0)]
    [Header("Maximal Angles", order = 1)]
    [SerializeField] float stabilisation_Angle_Maximal_Up;
    [SerializeField] float stabilisation_Angle_Maximal_Down;

    [Header("Distances")]
    [SerializeField] float stabilisation_Distance_Minimal;
    [SerializeField] float stabilisation_Distance_Maximal;

    #endregion Stabilisation Parameters

    #region Transition Settings

    [Space, Header("Transition Parameters")]
    [SerializeField] float stabilisation_Camera_Rotation_FocusPoint_Speed;
    [SerializeField] float stabilisation_Camera_Rotation_Reset;

    #endregion Transition Settings

    #region Raycast Settings

    [Space, Header("Raycast Parameters")]
    [SerializeField] float stabilisation_Ray_Lenght = 100;
    [SerializeField] LayerMask stabilisation_Ray_Layers;
    RaycastHit stabilisation_Ray_Hit;

    #endregion Raycast Settings

    #region Focus Point

    [Space, Header("Focus Point")]
    [SerializeField] Vector3 stabilisation_FocusPoint_Position;

    #endregion Focus Point

    #region Transforms

    [Space, Header("Transforms")]
    [SerializeField] Transform transform_CameraHolder;
    [SerializeField] Transform transform_Head_Base;
    [SerializeField] Transform transform_Head_Pivot;

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
        }
        else
        {
            Stabilisation_Reset();
        }
    }

    void Stabilisation_Controller()
    {
        //Actual Rotation//
        float headRotation_X = transform_Head_Pivot.localRotation.eulerAngles.x;


        if (headRotation_X <= stabilisation_Angle_Maximal_Down || headRotation_X >= (360 - stabilisation_Angle_Maximal_Up))
        {
            Stabilisation_Raycast();
        }
        else
        {
            Stabilisation_Reset();
        }
    }

    void Stabilisation_Raycast()
    {
        if (Physics.Raycast(transform_Head_Base.position, transform_Head_Pivot.forward, out stabilisation_Ray_Hit, stabilisation_Ray_Lenght, stabilisation_Ray_Layers))
        {
            //Check if it is in correct distance//
            if (stabilisation_Ray_Hit.distance >= stabilisation_Distance_Minimal && stabilisation_Ray_Hit.distance <= stabilisation_Distance_Maximal)
            {
                //Update focus point//
                stabilisation_FocusPoint_Position = stabilisation_Ray_Hit.point;

                //Look at target//
                Stabilisation_FocusPoint_LookAt();
            }
            else
            {
                Stabilisation_Reset();
            }

        }
        else //Reset//
        {
            Stabilisation_Reset();
        }
    }

    void Stabilisation_FocusPoint_LookAt()
    {
        //Calculate Direction//
        Vector3 Camera_Focus_Direction = stabilisation_FocusPoint_Position - transform_CameraHolder.position;

        //From Direction determine quaterion//
        Quaternion Camera_Focus_Rotation = Quaternion.LookRotation(Camera_Focus_Direction);

        //Rotate on target, in time//
        transform_CameraHolder.rotation = Quaternion.RotateTowards(transform_CameraHolder.rotation, Camera_Focus_Rotation, Time.deltaTime * stabilisation_Camera_Rotation_FocusPoint_Speed);
    }

    void Stabilisation_Reset()
    {
        //Reset Camera rotation//
        if (transform_CameraHolder.localRotation != Quaternion.identity)
        {
            transform_CameraHolder.localRotation = Quaternion.RotateTowards(transform_CameraHolder.localRotation, Quaternion.identity, Time.deltaTime * stabilisation_Camera_Rotation_Reset);
        }
    }

    #region Debug
    private void OnDrawGizmos()
    {
        if (debug_Stabilisation)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(transform_CameraHolder.position, stabilisation_FocusPoint_Position);
        }
    }
    #endregion Debug

}
