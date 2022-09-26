using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class GUI_Controller_HUD : MonoBehaviour
{
    #region Assignments
    Player_Interaction_Controller player_Interaction_Controller;

    void Awake()
    {
        player_Interaction_Controller = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Interaction_Controller>();
    }
    #endregion Assignments


    #region Variables

    #region Crosshair & Interaction
    [Header("Interaction Progress Object")]
    [SerializeField] GameObject HUD_Interaction_Progress_Object;
    #endregion

    #endregion Variables

    void Update()
    {
        HUD_Interaction_Progress_Controller();
    }

    #region Crosshair & Interaction

    void HUD_Interaction_Progress_Controller()
    {
        if (player_Interaction_Controller.interaction_InteractiveObject_Actual != null)
        {
            float progress = player_Interaction_Controller.interaction_InteractiveObject_Actual.GetComponent<Interactable_Main>().interactable_Interaction_HoldProgress / player_Interaction_Controller.interaction_InteractiveObject_Actual.GetComponent<Interactable_Main>().interactable_Interaction_HoldProgress_Target;

            //Set to be active again//
            if (!HUD_Interaction_Progress_Object.activeInHierarchy)
            {
                HUD_Interaction_Progress_Object.SetActive(true);
            }

            HUD_Interaction_Progress_Object.GetComponent<Image>().fillAmount = progress;
        }
        else //When any object is active//
        {
            if (HUD_Interaction_Progress_Object.activeInHierarchy)
            {
                //Turn Inactive//
                HUD_Interaction_Progress_Object.SetActive(false);

                //Reset Progress//
                HUD_Interaction_Progress_Object.GetComponent<Image>().fillAmount = 0;
            }
        }
    }


    #endregion Crosshair & Interaction

}
