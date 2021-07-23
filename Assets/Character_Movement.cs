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
    private Vector3 vMovementDirection;

    public Camera mCamera;
    public float fTurnSpeed = 1.0f;
    public float fWalkSpeed = 1.0f;
    public float fJumpHeight = 2.0f;
    public float fCrouchHeight = 0.5f;
    public float fCrouchSpeed = 1.0f;
    public bool bCrouchToggle = false;
    public float fFriction = 1.0f;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        vMovementDirection = Vector3.zero;
        mCharacter = gameObject.GetComponent<CharacterController>();
        fTargetHeight = fStandHeight = mCharacter.height;
    }

    // Update is called once per frame
    void Update()
    {
        vMovementDirection = mCharacter.velocity;

        RotateCharacter();
        MoveCharacter();
        //Don't let the player crouch while jumping
        UpdateStance();
        //Applying gravity needs to be last
        ApplyGravity();
        ApplyFriction();

        mCharacter.Move(vMovementDirection * Time.deltaTime);
    }

    void RotateCharacter()
    {
        float rotationX = Input.GetAxis("Mouse X") * fTurnSpeed * Time.deltaTime;
        vInputAngle = new Vector3(0.0f, vInputAngle.y + rotationX, 0.0f);
        transform.localEulerAngles = vInputAngle;
    }

    void ApplyGravity()
    {
       if(!mCharacter.isGrounded)
        {
            vMovementDirection += (Physics.gravity * Time.deltaTime);
        }
    }

    void ApplyFriction()
    {
        Vector3 frictionForce = vMovementDirection * fFriction;
        frictionForce.y = 0;
        vMovementDirection -= frictionForce * Time.deltaTime;
    }

    void MoveCharacter()
    {
        bIsGrounded = mCharacter.isGrounded;

        //Move
        if (Input.GetKey(KeyCode.W))
        {
            vMovementDirection += Walk(mCharacter.transform.forward);
        }
        if (Input.GetKey(KeyCode.S))
        {
            vMovementDirection += Walk(Invert(mCharacter.transform.forward));
        }
        if (Input.GetKey(KeyCode.A))
        {
            vMovementDirection += Walk(Invert(mCharacter.transform.right));
        }
        if (Input.GetKey(KeyCode.D))
        {
            vMovementDirection += Walk(mCharacter.transform.right);
        }

        //Jump
        if (bIsGrounded && Input.GetKeyDown(KeyCode.Space))
            vMovementDirection += Jump();
    }

    void UpdateStance()
    {
        //Check Crouch Conditions
        if (bCrouchToggle)
            CheckCrouchToggle();
        else
            CheckCrouchHold();
    }

    Vector3 Jump()
    {
        Vector3 direction = new Vector3(0, fJumpHeight);
        return direction;
    }

    Vector3 Walk(Vector3 direction)
    {
        return direction * fWalkSpeed;
    }

    void CheckCrouchToggle()
    {
        if (!bIsCrouched && Input.GetKeyDown(KeyCode.C))
            Crouch();
        else if (bIsCrouched && Input.GetKeyDown(KeyCode.C))
            Stand();
    }

    void CheckCrouchHold()
    {
        if (!bIsCrouched && Input.GetKeyDown(KeyCode.C))
            Crouch();
        else if (bIsCrouched && Input.GetKeyUp(KeyCode.C))
            Stand();
    }

    void Crouch()
    {
        animator.SetBool("isCrouching", true);
        bIsCrouched = true;
    }

    void Stand()
    {
        animator.SetBool("isCrouching", false);
        bIsCrouched = false;
    }

    Vector3 Invert(Vector3 _direction)
    {
        return _direction * -1;
    }
}
