using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Movement : MonoBehaviour
{
    private CharacterController mCharacter;
    private Vector3 vVerticalVelocity;
    private bool bIsGrounded;
    private Vector3 vInputAngle;

    public Camera mCamera;
    public float fTurnSpeed = 1.0f;
    public float fWalkSpeed = 1.0f;
    public float fJumpHeight = 2.0f;

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
        float rotationX = Input.GetAxis("Mouse X") * this.fTurnSpeed * Time.deltaTime;

        vInputAngle = new Vector3(0.0f, vInputAngle.y + rotationX, 0.0f);

        transform.localEulerAngles = vInputAngle;
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
            this.Walk(this.mCharacter.transform.forward);
        }
        if (Input.GetKey("s"))
        {
            this.Walk(this.Inverse(this.mCharacter.transform.forward));
        }
        if (Input.GetKey("a"))
        {
            this.Walk(this.Inverse(this.mCharacter.transform.right));
        }
        if (Input.GetKey("d"))
        {
            this.Walk(this.mCharacter.transform.right);
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

    Vector3 Inverse(Vector3 _direction)
    {
        return _direction * -1;
    }
}
