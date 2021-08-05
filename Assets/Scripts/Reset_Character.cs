using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset_Character : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        print("HIT " + collider.name);
        collider.GetComponent<Character_Movement>().ResetCharacter();
    }
}
