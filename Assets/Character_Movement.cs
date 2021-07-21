using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Movement : MonoBehaviour
{
    private CharacterController mCharacter;
    private Vector3 vVerticalVelocity;
    private bool bIsGrounded;
    private bool bIsCrouched;
    private Vector3 vInputAngle;
    private float fStandHeight;
    private float fTargetHeight;

    public Camera mCamera;
    public float fTurnSpeed = 1.0f;
    public float fWalkSpeed = 1.0f;
    public float fJumpHeight = 2.0f;
    public float fCrouchHeight = 0.5f;
    public float fCrouchSpeed = 1.0f;
    public bool bCrouchToggle = false;

    // Start is called before the first frame update
    void Start()
    {
        this.mCharacter = gameObject.GetComponent<CharacterController>();
        this.fTargetHeight = this.fStandHeight = this.mCharacter.height;
    }

    // Update is called once per frame
    void Update()
    {
        this.RotateCharacter();
        this.MoveCharacter();
        //Don't let the player crouch while jumping
        if(this.bIsGrounded)
            this.UpdateStance();
        //Applying gravity needs to be last
        this.ApplyGravity();
    }

    void RotateCharacter()
    {
        float rotationX = Input.GetAxis("Mouse X") * this.fTurnSpeed * Time.deltaTime;
        vInputAngle = new Vector3(0.0f, vInputAngle.y + rotationX, 0.0f);
        transform.localEulerAngles = vInputAngle;
    }

    void ApplyGravity()
    {
       if(mCharacter.isGrounded && this.vVerticalVelocity.y < 0)
            this.vVerticalVelocity = Vector3.zero;
       else
        {
            this.vVerticalVelocity += Physics.gravity * Time.deltaTime;
            this.mCharacter.Move(this.vVerticalVelocity * Time.deltaTime);
        }
    }

    void MoveCharacter()
    {
        this.bIsGrounded = this.mCharacter.isGrounded;

        //Move
        if (Input.GetKey(KeyCode.W))
        {
            this.Walk(this.mCharacter.transform.forward);
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.Walk(this.Invert(this.mCharacter.transform.forward));
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.Walk(this.Invert(this.mCharacter.transform.right));
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.Walk(this.mCharacter.transform.right);
        }

        //Jump
        if (this.bIsGrounded && Input.GetKeyDown(KeyCode.Space))
            this.Jump();
    }

    void UpdateStance()
    {
        //Check Crouch Conditions
        if (this.bCrouchToggle)
            this.CheckCrouchToggle();
        else
            this.CheckCrouchHold();

        //Update Stance
        if(!mCharacter.height.Equals(this.fTargetHeight))
            mCharacter.height = Mathf.SmoothStep(mCharacter.height, this.fTargetHeight, (Time.deltaTime * this.fCrouchSpeed));
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

    void CheckCrouchToggle()
    {
        if (!this.bIsCrouched && Input.GetKeyDown(KeyCode.C))
            this.Crouch();
        else if (this.bIsCrouched && Input.GetKeyDown(KeyCode.C))
            this.Stand();
    }

    void CheckCrouchHold()
    {
        if (!this.bIsCrouched && Input.GetKeyDown(KeyCode.C))
            this.Crouch();
        else if (this.bIsCrouched && Input.GetKeyUp(KeyCode.C))
            this.Stand();
    }

    void Crouch()
    {
        this.fTargetHeight = this.fCrouchHeight;
        this.bIsCrouched = true;
    }

    void Stand()
    {
        this.fTargetHeight = this.fStandHeight;
        this.bIsCrouched = false;
    }

    Vector3 Invert(Vector3 _direction)
    {
        return _direction * -1;
    }
}
