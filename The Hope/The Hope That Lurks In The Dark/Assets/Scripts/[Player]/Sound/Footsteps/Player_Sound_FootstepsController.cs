using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class Player_Sound_FootstepsController : MonoBehaviour
{
    #region Assignments
    Player_Main playerMain;
    Head_HeadBobbing_Default camera_HeadBobbing;

    void Awake()
    {
        playerMain = GetComponentInParent<Player_Main>();
        camera_HeadBobbing = playerMain.playerHead_Base.GetComponent<Head_HeadBobbing_Default>();
    }
    #endregion Assignments

    #region Variables

    #region Sounds
    [Header("General Sound Controller", order = 0)]
    [Header("Default Sounds", order = 1)]

    [Header("Walk - Footsteps", order = 2)]
    [SerializeField] EventReference[] sound_Footstep_Default_Walk;

    [Header("Run - Footsteps")]
    [SerializeField] EventReference[] sound_Footstep_Default_Run;

    [Header("Crouch Walk - Footsteps")]
    [SerializeField] EventReference[] sound_Footstep_Default_Crouch;

    [Header("Landing")]
    [SerializeField] EventReference[] sound_Footstep_Default_Landing;
    #endregion Sounds

    #region Ground Type

    enum Footsteps_GroundTypes { Default, Special };
    [Header("Ground Type")]
    [SerializeField] Footsteps_GroundTypes footsteps_GroundType;

    #endregion Ground Type

    //Sound Position//
    [Header("Footsteps Sound Position")]
    [SerializeField] Transform player_FootstepSound_Position;

    //Fmod Event//
    FMOD.Studio.EventInstance sound_Footstep_Event;
    #endregion Variables

    #region Footsteps
    public void Footsteps_Play()
    {
        Footsteps_Play_Controller();
    }

    void Footsteps_Play_Controller()
    {
        if (playerMain.isMoving) //Playe only when player is moving/
        {
            switch (playerMain.playerDefaultMovementState)
            {
                //TestCase//
                case Player_Main.PlayerDefaultMovementStates.Idle:
                    Footsteps_Play_Walk_Controller();
                    return;

                case Player_Main.PlayerDefaultMovementStates.Walking:
                    Footsteps_Play_Walk_Controller();
                    return;

                case Player_Main.PlayerDefaultMovementStates.Running:
                    Footsteps_Play_Run_Controller();
                    return;

                case Player_Main.PlayerDefaultMovementStates.Crouching:
                    Footsteps_Play_Crouch_Controller();
                    return;
            }
        }
        else
        {


        }
    }

    #region Walk Controllers
    void Footsteps_Play_Walk_Controller()
    {
        switch (footsteps_GroundType)
        {
            case Footsteps_GroundTypes.Default:
                Footsteps_Play_Walk_Default();
                return;
        }
    }

    void Footsteps_Play_Walk_Default()
    {
        //Choose random sample//
        int sound_Random = Random.Range(0, sound_Footstep_Default_Walk.Length - 1);

        //Create sound instantion//
        sound_Footstep_Event = RuntimeManager.CreateInstance(sound_Footstep_Default_Walk[sound_Random]);

        //Attach it to foot gameobject//
        RuntimeManager.AttachInstanceToGameObject(sound_Footstep_Event, player_FootstepSound_Position);

        //Play Sound//
        sound_Footstep_Event.start();
    }

    #endregion

    #region Run Controllers
    void Footsteps_Play_Run_Controller()
    {
        switch (footsteps_GroundType)
        {
            case Footsteps_GroundTypes.Default:
                Footsteps_Play_Run_Default();
                return;
        }
    }

    void Footsteps_Play_Run_Default()
    {
        //Choose random sample//
        int sound_Random = Random.Range(0, sound_Footstep_Default_Run.Length - 1);

        //Create sound instantion//
        sound_Footstep_Event = FMODUnity.RuntimeManager.CreateInstance(sound_Footstep_Default_Run[sound_Random]);

        //Attach it to foot gameobject//
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(sound_Footstep_Event, player_FootstepSound_Position);

        //Play Sound//
        sound_Footstep_Event.start();
    }

    #endregion

    #region Walk Controllers
    void Footsteps_Play_Crouch_Controller()
    {
        switch (footsteps_GroundType)
        {
            case Footsteps_GroundTypes.Default:
                Footsteps_Play_Crouch_Default();
                return;

        }

    }

    void Footsteps_Play_Crouch_Default()
    {
        //Choose random sample//
        int sound_Random = Random.Range(0, sound_Footstep_Default_Crouch.Length - 1);

        //Create sound instantion//
        sound_Footstep_Event = FMODUnity.RuntimeManager.CreateInstance(sound_Footstep_Default_Crouch[sound_Random]);

        //Attach it to foot gameobject//
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(sound_Footstep_Event, player_FootstepSound_Position);

        //Play Sound//
        sound_Footstep_Event.start();
    }

    #endregion

    #endregion Footsteps

    #region Landing
    public void Footsteps_Landing()
    {
        switch (footsteps_GroundType)
        {
            case Footsteps_GroundTypes.Default:
                Footsteps_Landing_Default();
                return;
        }
    }

    void Footsteps_Landing_Default()
    {
        //Choose random sample//
        int sound_Random = Random.Range(0, sound_Footstep_Default_Landing.Length - 1);

        //Create sound instantion//
        sound_Footstep_Event = FMODUnity.RuntimeManager.CreateInstance(sound_Footstep_Default_Landing[sound_Random]);

        //Attach it to foot gameobject//
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(sound_Footstep_Event, player_FootstepSound_Position);

        //Play Sound//
        sound_Footstep_Event.start();
    }

    #endregion Landing
}
