using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_TEST : Interactable_Item_Interactions
{
    public override void Item_Interaction_Primary()
    {
        if (interactable_Item_IsInHands)
            Debug.Log(gameObject.name + ": Left Click");
    }

    public override void Item_Interaction_Secondary()
    {
        if (interactable_Item_IsInHands)
            Debug.Log(gameObject.name + ": Right Click");
    }
}
