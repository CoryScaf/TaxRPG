using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterFallInVoid : MonoBehaviour
{
    public GameObject playerObject;
    public bool returnToClosestHorizontal = true;
    public bool returnToClosestVertical = false;
    public float displacement = 0.5f;
    public Vector3 alternativeReturnPos;

    void OnTriggerStay2D(Collider2D col) {
        if(col.gameObject == playerObject) {
            // possibly remove health
            Bounds bnds = GetComponent<BoxCollider2D>().bounds;
            if(!bnds.Contains(col.bounds.max) || !bnds.Contains(col.bounds.min)) return;
            if(playerObject.GetComponent<CharacterMovement>().isCurrentlyRolling()) return;
            Vector3 returnPos = playerObject.transform.position;
            if(returnToClosestHorizontal) {
                if(playerObject.transform.position.x < bnds.center.x)
                    returnPos.x = bnds.center.x - (bnds.extents.x + displacement);
                else
                    returnPos.x = bnds.center.x + bnds.extents.x + displacement;
            }
            else if(returnToClosestVertical) {
                if(playerObject.transform.position.y < bnds.center.y)
                    returnPos.y = bnds.center.y - (bnds.extents.y + displacement);
                else
                    returnPos.y = bnds.center.y + bnds.extents.y + displacement;
            }
            else {
                returnPos = alternativeReturnPos;
            }
            playerObject.transform.position = returnPos;

        }
    }
}
