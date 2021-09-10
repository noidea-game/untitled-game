using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving_Platform : MonoBehaviour
{
    public GameObject player;
    public GameObject startPoint;
    public GameObject endPoint;
    public float moveSpeed;

    Vector3 platformVelocity;
    float bounceDist = 1f;

    CharacterController cc;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        platformVelocity = Vector3.MoveTowards(transform.position, endPoint.transform.position, moveSpeed * Time.deltaTime);

        float distFromStart = Vector3.Distance(transform.position, startPoint.transform.position);
        float distFromEnd = Vector3.Distance(transform.position, endPoint.transform.position);

        if (distFromStart < bounceDist || distFromEnd < bounceDist)
        {
            moveSpeed *= -1;
        }

        rb.MovePosition(platformVelocity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
            cc = other.GetComponent<CharacterController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
            cc.GetComponent<Character_Movement>().ExternalVelocity(rb.velocity);
    }
}
