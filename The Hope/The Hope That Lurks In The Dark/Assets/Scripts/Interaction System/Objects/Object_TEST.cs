using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_TEST : Interactable_Object
{
    public override void Object_Interact()
    {
        gameObject.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
    }
}
