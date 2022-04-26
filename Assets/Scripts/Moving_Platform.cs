using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving_Platform : MonoBehaviour
{
    public GameObject player;
    public GameObject startPoint;
    public GameObject endPoint;
    public float moveSpeed;

    Vector3 platformTarget;
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
        platformTarget = Vector3.MoveTowards(transform.position, endPoint.transform.position, moveSpeed * Time.deltaTime);

        float distFromStart = Vector3.Distance(transform.position, startPoint.transform.position);
        float distFromEnd = Vector3.Distance(transform.position, endPoint.transform.position);

        if (distFromStart < bounceDist || distFromEnd < bounceDist)
        {
            moveSpeed *= -1;
        }

        rb.MovePosition(platformTarget);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            cc = other.GetComponent<CharacterController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            cc.GetComponent<Character_Movement>().ExternalVelocity(rb.velocity);
    }
}
