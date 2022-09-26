using UnityEngine;

public class Player_Position_Reset : MonoBehaviour
{
    #region Variables
    [Header("New Position")]
    [SerializeField] bool newPositionAsVector;
    [SerializeField] Vector3 newPosition_Vector;
    [Space]
    [SerializeField] bool newPositionAsTransform;
    [SerializeField] Transform newPosition_Transform = null;
    #endregion Variables

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(newPositionAsVector)
            {
                other.transform.position = newPosition_Vector;
            }
            else if(newPositionAsTransform)
            {
                other.transform.position = newPosition_Transform.position;
            }
        }
    }
}
