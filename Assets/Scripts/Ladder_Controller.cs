using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder_Controller : MonoBehaviour
{
    public Transform characterController;
    public bool active = false; //when true, the player is able to climb
    public bool enable = false; //when true, the player is moved into climbing position
    public float climbSpeed = 3.0f;
    public Character_Movement character_movment;

    private GameObject ladder;

    // Start is called before the first frame update
    void Start()
    {
        active = false;
        character_movment = GetComponent<Character_Movement>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E) && other.gameObject.CompareTag("Ladder"))
        {
            ladder = other.gameObject;
            character_movment.enabled = false;
            enable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            character_movment.enabled = true;
            active = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(enable)
        {
            float angle = Mathf.MoveTowardsAngle(characterController.transform.eulerAngles.y, ladder.transform.eulerAngles.y, Time.deltaTime);
            characterController.transform.Rotate(0, angle, 0, Space.Self);
            print(angle);
            if (angle < 0)
                enable = false;
        }
        if(active && Input.GetKey(KeyCode.W))
        {
            characterController.transform.position += climbSpeed * Time.deltaTime * Vector3.up;
        }
        if(active && Input.GetKey(KeyCode.S))
        {
            characterController.transform.position += climbSpeed * Time.deltaTime * Vector3.down;
        }
    }
}
