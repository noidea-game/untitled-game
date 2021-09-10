using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Movement : MonoBehaviour
{
    private CharacterController mCharacter;
    private bool bIsCrouched;
    private Vector3 vInputAngle;
    private Vector3 vMovementDirection;
    private Ray ray;
    private RaycastHit raycastHitInfo;
    private Vector3 exVel;

    public GameObject currSpawnPoint;
    public Camera mCamera;
    public float fWalkSpeed = 1.0f;
    public float fTurnSpeed = 1.0f;
    public float fJumpHeight = 1.0f;
    public bool bCrouchToggle = false;
    public float fFriction = 1.0f;
    public Animator animator;
    public bool bIsJumping = false;

    // Start is called before the first frame update
    void Start()
    {
        vMovementDirection = Vector3.zero;
        mCharacter = gameObject.GetComponent<CharacterController>();
        raycastHitInfo = new RaycastHit();
        ResetCharacter();
    }

    // Update is called once per frame
    void Update()
    {
        vMovementDirection = mCharacter.velocity;

        //apply any external veleocities
        vMovementDirection += exVel;

        RotateCharacter();
        MoveCharacter();
        UpdateStance();
        ApplyFriction();
        ApplyGravity();
        JumpCheck();

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
        if (!mCharacter.isGrounded)
            vMovementDirection += (Physics.gravity * Time.deltaTime);
        else
        {
            bIsJumping = false;
            vMovementDirection += Physics.gravity.normalized;
        }
    }

    void ApplyFriction()
    {
        Vector3 frictionForce = vMovementDirection * fFriction;
        frictionForce.y = 0;
        vMovementDirection -= frictionForce * Time.deltaTime;
    }

    void ApplySlope()
    {
        ray = new Ray(transform.position, Vector3.down);
        Physics.Raycast(ray, out raycastHitInfo, 100);

        //Prevents character from 'bouncing' when going down a slope
        if (raycastHitInfo.normal.y < 1)
            vMovementDirection.y -= raycastHitInfo.normal.normalized.y;
    }

    void MoveCharacter()
    {
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
    }

    void UpdateStance()
    {
        //Check Crouch Conditions
        if (bCrouchToggle)
            CheckCrouchToggle();
        else
            CheckCrouchHold();
    }

    void JumpCheck()
    {
        //Is the character jumping?
        if (mCharacter.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            vMovementDirection += Jump();
            bIsJumping = true;
        }

        //Or are they staying on the ground?
        if (!bIsJumping)
        {
            ApplySlope();
            //Prevents character from popping up when going up a slope.
            GroundClamp();
        }
    }

    Vector3 Jump()
    {
        return new Vector3(0, fJumpHeight);
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

    void GroundClamp()
    {
        if (vMovementDirection.y > 0)
            vMovementDirection.y = 0;
    }

    public void ResetCharacter()
    {
        mCharacter.enabled = false;

        //Zero out the velocities
        mCharacter.velocity.Set(0, 0, 0);
        vMovementDirection.Set(0, 0, 0);

        //Set Character position to current spawn point
        mCharacter.transform.SetPositionAndRotation(currSpawnPoint.transform.position, Quaternion.identity);

        mCharacter.enabled = true;
    }

    public void ExternalVelocity(Vector3 v)
    {
        exVel = v * Time.deltaTime;
        print("Velocity:" + v);
        print("ExVel: " + exVel);
    }
}
