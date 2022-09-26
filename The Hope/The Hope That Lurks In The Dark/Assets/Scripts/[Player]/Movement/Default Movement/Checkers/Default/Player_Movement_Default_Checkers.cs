using UnityEngine;

public class Player_Movement_Default_Checkers : MonoBehaviour
{
    #region Assignments
    Player_Main playerMain;
    CharacterController playerController;
    Player_Movement_Default playerDefaultMovement;
    Player_Movement_Default_Crouching playerCrouchController;

    void Awake()
    {
        playerMain = GetComponentInParent<Player_Main>(); //Take PlayerMain from player main object//
        playerController = GetComponentInParent<CharacterController>();
        playerDefaultMovement = GetComponentInParent<Player_Movement_Default>(); //Take Default player movement controller//
        playerCrouchController = GetComponentInParent<Player_Movement_Default_Crouching>(); //Take Player crouch controller, for celling check//

        playerCheckPosition = this.gameObject.transform; //Set Check Transform to object position//

        SetStartValues(); //Set values on start//
    }

    #endregion Assignments

    #region Variables
    [Header("General Options")]
    [SerializeField] Transform playerCheckPosition; //Position from all checkers starts//
    [Space]
    [SerializeField] LayerMask groundLayers; //Layers that counts as ground//
    [SerializeField] LayerMask cellingLayers; //Layers that counts as celling//

    #region Ground Check
    [Header("Ground Check Parameters")]
    bool playerIsGrounded = false; //When player is grounded//
    [SerializeField] float groundCheck_Lenght; //Lenght of Sphere cast offset//
    [SerializeField] float groundCheck_SphereRadius; //Sphere cast radius//
    [Space]
    RaycastHit groundCheck_Hit; //Return what was hitted by raycast//

    #endregion Ground Check

    #region Celling Check
    [Header("Celling Check", order = 0)]
    [Header("Standing", order = 1)]
    [SerializeField] float cellingCheck_Stand_Lenght; //Lenght of sphere cast//
    [SerializeField] float cellingCheck_Stand_SphereRadius; //Sphere cast radius//
    [Header("Crouch")]
    [SerializeField] float cellingCheck_Crouch_Lenght; //Lenght of sphere cast//
    [SerializeField] float cellingCheck_Crouch_SphereRadius; //Sphere cast radius//

    RaycastHit cellingCheck_Hit;

    #endregion Celling Check

    #region Slope Check
    [Header("Slope Check Parameters")]
    [SerializeField] bool slopeCheck_IsOnSlope_Started;
    [SerializeField] int slopeCheck_MaxSlope; //Maximal slope//
    [Space]
    [SerializeField] float slopeCheck_Lenght; //Lenght of raycast//
    [Space]
    [SerializeField] float slopeCheck_Sphere_Lenght;
    [SerializeField] float slopeCheck_Sphere_Radius;

    RaycastHit slopeCheck_Hit;
    RaycastHit slopeCheck_Sphere_Hit;

    #endregion Slope Check

    #region Debug

    [Header("Debug")]
    //Ground Check
    [SerializeField] bool DEBUG_groundCheck; //Draws ground Check//
    float DEBUG_groundCheck_Lenght;
    [SerializeField] bool DEBUG_cellingCheck; //Draws celling check//
    float DEBUG_cellingCheck_Crouch_Lenght;
    float DEBUG_cellingCheck_Stand_Lenght;
    [SerializeField] bool DEBUG_slopeCheck;
    float DEBUG_slopeCheck_Lenght;
    float DEBUG_slopeCheck_Sphere_Lenght;
    float DEBUG_slopeCheck_Support_Lenght;

    #endregion Debug

    #endregion Variables

    void SetStartValues()
    {
        //Set maximal slope//
        slopeCheck_MaxSlope = (int)playerController.slopeLimit;
    }

    void Update()
    {
        if (playerMain.playerGeneralState == Player_Main.PlayerGeneralStates.Default && playerMain.playerMovementState == Player_Main.PlayerMovementStates.Default)
        {
            GroundCheck(); //Grounding//        
            SlopeCheck(); //Slope//
            CellingCheck(); //Celling Check//

        }

    }


    #region Check Functions
    void GroundCheck() //Checks if player is grounded//
    {
        //Sphere cast check function//
        if (Physics.SphereCast(playerCheckPosition.position, groundCheck_SphereRadius, Vector3.down, out groundCheck_Hit, groundCheck_Lenght, groundLayers, QueryTriggerInteraction.Ignore))
        {
            playerIsGrounded = true;

            #region Debug Gizmo

            //Debug Gizmo Draw//
            if (DEBUG_groundCheck)
            {
                DEBUG_groundCheck_Lenght = groundCheck_Hit.distance;
            }

            #endregion Debug Gizmo
        }
        else
        {
            playerIsGrounded = false;

            #region Debug

            //Debug Gizmo Draw//
            if (DEBUG_groundCheck)
            {
                DEBUG_groundCheck_Lenght = groundCheck_Lenght;
            }

            #endregion Debug
        }

        playerMain.isGrounded = playerIsGrounded; //Update in main script//
    }


    void CellingCheck() //Checking if above player is space to stand up from crouch//
    {
        bool playerCanStandUp = false; //if player can stand up//

        #region Celling Check

        if (playerMain.playerDefaultOnAirState != Player_Main.PlayerDefaultOnAirStates.Grounded)
        {
            if (Physics.SphereCast(playerCheckPosition.position, cellingCheck_Stand_SphereRadius, Vector3.up, out cellingCheck_Hit, cellingCheck_Stand_Lenght, cellingLayers, QueryTriggerInteraction.Ignore))
            {
                if (!playerMain.hittedCelling)
                {
                    playerMain.hittedCelling = true;
                }

                #region Debug
                //Debug Gizmo Thingy//
                if (DEBUG_cellingCheck)
                {
                    DEBUG_cellingCheck_Stand_Lenght = cellingCheck_Hit.distance;
                }
                #endregion Debug
            }
            else
            {

                #region Debug
                //Debug Gizmo Thingy//
                if (DEBUG_cellingCheck)
                {
                    DEBUG_cellingCheck_Stand_Lenght = cellingCheck_Stand_Lenght;
                }
                #endregion Debug
            }
        }
        else if(playerMain.playerDefaultMovementState == Player_Main.PlayerDefaultMovementStates.Crouching) //When crouching check if can stand up//
        {
            if (Physics.SphereCast(playerCheckPosition.position, cellingCheck_Crouch_SphereRadius, Vector3.up, out cellingCheck_Hit, cellingCheck_Crouch_Lenght, cellingLayers, QueryTriggerInteraction.Ignore))
            {
                playerCanStandUp = false; //When player hit something above him, then he cannot stand up//

                #region Debug
                //Debug Gizmo Thingy//
                if (DEBUG_cellingCheck)
                {
                    DEBUG_cellingCheck_Crouch_Lenght = cellingCheck_Hit.distance;
                }
                #endregion Debug
            }
            else
            {
                playerCanStandUp = true; //When nothing is abowe player, then he may stand up//

                #region Debug
                //Debug Gizmo Thingy//
                if (DEBUG_cellingCheck)
                {
                    DEBUG_cellingCheck_Crouch_Lenght = cellingCheck_Crouch_Lenght;
                }
                #endregion Debug
            }
        }
        #endregion Celling Check

        #region Update

        //Update in script//
        playerMain.canStandUp = playerCanStandUp; //Update in main controller//

        #endregion Update

    }

    void SlopeCheck()
    {
        Vector3 slopeNormals = Vector3.zero;

        bool Raycast_Hits_Ray = false;
        bool Raycast_Hits_Sphere = false;

        //If Raycast hits//
        //    if (Physics.Raycast(playerCheckPosition.position, Vector3.down, out slopeCheck_Hit, slopeCheck_Lenght, groundLayers))
        //  {
        //     if (Physics.SphereCast(playerCheckPosition.position, slopeCheck_SupportSphere_Radius, Vector3.down, out slopeCheck_SupportSphere_Hit, slopeCheck_SupportSphere_Lenght, groundLayers))
        //    {

        //Cast Rays//
        if (Physics.Raycast(playerCheckPosition.position, Vector3.down, out slopeCheck_Hit, slopeCheck_Lenght, groundLayers))
        {
            Raycast_Hits_Ray = true;

            //Cast Support//
            if (Physics.SphereCast(playerCheckPosition.position, slopeCheck_Sphere_Radius, Vector3.down, out slopeCheck_Sphere_Hit, slopeCheck_Sphere_Lenght, groundLayers))
            {
                Raycast_Hits_Sphere = true;

                if (DEBUG_slopeCheck)
                {
                    DEBUG_slopeCheck_Sphere_Lenght = slopeCheck_Sphere_Hit.distance;
                }
            }
            else
            {
                Raycast_Hits_Sphere = false;

                if (DEBUG_slopeCheck)
                {
                    DEBUG_slopeCheck_Sphere_Lenght = slopeCheck_Sphere_Lenght;
                }
            }

            if (DEBUG_slopeCheck)
            {
                DEBUG_slopeCheck_Lenght = slopeCheck_Hit.distance;
            }
        }
        else
        {
            Raycast_Hits_Ray = false;

            if (DEBUG_slopeCheck)
            {
                DEBUG_slopeCheck_Lenght = slopeCheck_Lenght;
            }
        }

        //When slopiing not started//
        if (!slopeCheck_IsOnSlope_Started)
        {
            //If both shoots//
            if (Raycast_Hits_Ray && Raycast_Hits_Sphere)
            {
                if (Vector3.Angle(slopeCheck_Hit.normal, Vector3.up) > slopeCheck_MaxSlope && Vector3.Angle(slopeCheck_Sphere_Hit.normal, Vector3.up) > slopeCheck_MaxSlope)
                {
                    slopeCheck_IsOnSlope_Started = true;

                    slopeNormals = slopeCheck_Sphere_Hit.normal;
                }
            }
        }
        else //When started//
        {
            //Stop Sloping//
            if (Vector3.Angle(slopeCheck_Hit.normal, Vector3.up) <= slopeCheck_MaxSlope && Vector3.Angle(slopeCheck_Sphere_Hit.normal, Vector3.up) <= slopeCheck_MaxSlope)
            {
                slopeCheck_IsOnSlope_Started = false;
            }
            else //Otherways update normal//
            {
                slopeNormals = slopeCheck_Sphere_Hit.normal;
            }
        }



        if (playerMain.isOnSteepSlope != slopeCheck_IsOnSlope_Started)
        {
            playerMain.isOnSteepSlope = slopeCheck_IsOnSlope_Started;
        }

        if (slopeCheck_IsOnSlope_Started)
        {
            //  playerDefaultMovement.horizontalMove_SlopeNormal = slopeNormals;
        }
    }

    #endregion  Check Functions

    #region Debug Draws

    void OnDrawGizmos()
    {
        if (DEBUG_groundCheck)
        {
            DrawGizmo_Grounding(); //Draws Ground Sphere cast//
        }

        if (DEBUG_cellingCheck)
        {
            DrawGizmo_Celling_SphereCast();
        }

        if (DEBUG_slopeCheck)
        {
            DrawGizmo_Sloping();
        }
    }

    void DrawGizmo_Grounding()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerCheckPosition.position + DEBUG_groundCheck_Lenght * Vector3.down, groundCheck_SphereRadius);
    }

    void DrawGizmo_Celling_SphereCast()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(playerCheckPosition.position + DEBUG_cellingCheck_Crouch_Lenght * Vector3.up, cellingCheck_Crouch_SphereRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(playerCheckPosition.position + DEBUG_cellingCheck_Stand_Lenght * Vector3.up, cellingCheck_Stand_SphereRadius);
    }

    void DrawGizmo_Sloping()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(playerCheckPosition.position, playerCheckPosition.position + DEBUG_slopeCheck_Lenght * Vector3.down);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(playerCheckPosition.position + DEBUG_slopeCheck_Sphere_Lenght * Vector3.down, slopeCheck_Sphere_Radius);
    }

    #endregion Debug Draws

}
