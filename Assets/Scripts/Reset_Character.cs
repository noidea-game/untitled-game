using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset_Character : MonoBehaviour
{
    void OnTriggerEnter(Collider collider)
    {
        collider.GetComponent<Character_Movement>().ResetCharacter();
    }
}
