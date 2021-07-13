using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Movement : MonoBehaviour
{
    private CharacterController mCharacter;
    private Vector3 vVerticalVelocity;
    private bool bIsGrounded;

    //float smooth = 5.0f;
    //float tiltAngle = 60.0f;

    public Camera mCamera;
    public float fWalkSpeed = 1.0f;
    public float fJumpHeight = 2.0f;

    Vector3 Angles;
    public float sensitivityX;
    public float sensitivityY;

    // Start is called before the first frame update
    void Start()
    {
        this.mCharacter = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        this.rotateCharacter();
        this.moveCharacter();
        this.applyGravity();
    }

    void rotateCharacter()
    {
        //// Smoothly tilts a transform towards a target rotation.
        //float tiltAroundY = mCamera.transform.rotation.y * tiltAngle;
        //float tiltAroundX = mCamera.transform.rotation.x * tiltAngle;

        //// Rotate the cube by converting the angles into a quaternion.
        //Quaternion target = Quaternion.Euler(tiltAroundX, tiltAroundY, 0.0f);

        //// Dampen towards the target rotation
        //transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);\

        float rotationY = Input.GetAxis("Mouse Y") * sensitivityX;
        float rotationX = Input.GetAxis("Mouse X") * sensitivityY;
        if (rotationY > 0)
            Angles = new Vector3(Mathf.MoveTowards(Angles.x, -80, rotationY), Angles.y + rotationX, 0);
        else
            Angles = new Vector3(Mathf.MoveTowards(Angles.x, 80, -rotationY), Angles.y + rotationX, 0);
        transform.localEulerAngles = Angles;
    }

    void applyGravity()
    {
       if(mCharacter.isGrounded && this.vVerticalVelocity.y < 0)
        {
            this.vVerticalVelocity = Vector3.zero;
        }
       else
        {
            this.vVerticalVelocity += Physics.gravity * Time.deltaTime;
            this.mCharacter.Move(this.vVerticalVelocity * Time.deltaTime);
        }
    }

    void moveCharacter()
    {
        this.bIsGrounded = this.mCharacter.isGrounded;

        if (Input.GetKey("w"))
        {
            this.Walk(Vector3.forward);
        }
        if (Input.GetKey("s"))
        {
            this.Walk(Vector3.back);
        }
        if (Input.GetKey("a"))
        {
            this.Walk(Vector3.left);
        }
        if (Input.GetKey("d"))
        {
            this.Walk(Vector3.right);
        }
        if (Input.GetKey("space") && this.bIsGrounded)
        {
            this.Jump();
        }
    }

    void Jump()
    {
        this.vVerticalVelocity.y += Mathf.Sqrt(this.fJumpHeight * -3.0f * Physics.gravity.y);
        this.mCharacter.Move(this.vVerticalVelocity * Time.deltaTime);
    }

    void Walk(Vector3 _direction)
    {
        this.mCharacter.Move(_direction * this.fWalkSpeed * Time.deltaTime);
    }
}
