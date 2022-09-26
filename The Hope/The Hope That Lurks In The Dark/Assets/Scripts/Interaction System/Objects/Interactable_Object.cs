using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable_Object : Interactable_Main
{
    #region Variables
    [Header("Object Parameters", order = 0)]

    [Header("Require Item To Use?", order = 1)]
    public bool interactable_Object_RequireItem = false; //Item that must be in hand//
    public int interactable_Object_RequireItem_ID;

    [Header("Cooldown Parameters")]
    public bool interactable_Object_Cooldown = false;
    public float interactable_Object_Cooldown_Time = 1.5f;
    [HideInInspector] public bool interactable_Object_Cooldown_Done = true;
    #endregion Variables

    public override void Interact()
    {
        #region Check for item
        if (interactable_Object_RequireItem)
        {
            if (interactable_Object_RequireItem_ID == 0 /*The Item in hand*/) //When good item in hand
            {

            }
            else //Else
            {
                //Trow ui message//

                return;
            }
        }
        #endregion Check for item

        if (interactable_Object_Cooldown)
        {
            if (interactable_Object_Cooldown_Done)
            {
                Interactable_Object_Cooldown_Controller();
                Object_Interact();
            }
            else
            {
                Debug.Log("Cooldown not done");
                return;
            }
        }
        else //When not using cooldowns, then just interact//
        {
            Object_Interact();
        }
    }

    public abstract void Object_Interact();


    #region Cooldown
    void Interactable_Object_Cooldown_Controller()
    {
        //Set Done to false//
        interactable_Object_Cooldown_Done = false;

        //Start Counter to reset cooldown//
        StartCoroutine(Interactable_Object_Cooldown_Counter());
    }

    IEnumerator Interactable_Object_Cooldown_Counter()
    {
        yield return new WaitForSeconds(interactable_Object_Cooldown_Time);
        interactable_Object_Cooldown_Done = true;
    }

    #endregion
}
