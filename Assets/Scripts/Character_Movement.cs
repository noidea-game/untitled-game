using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Movement : MonoBehaviour
{
    private CharacterController mCharacter;
    private Vector3 vInputAngle;
    private Vector3 vMovementDirection;
    private Ray ray;
    private RaycastHit raycastHitInfo;
    private Vector3 exVel;
    private int nJumpCount = 0;

    public GameObject currSpawnPoint;
    public Camera mCamera;
    public float fMoveSpeed = 1.0f;
    public float fTurnSpeed = 1.0f;
    public float fJumpHeight = 1.0f;
    public int nJumpMax = 1;
    public float fFriction = 1.0f;
    public Animator animator;

    //Toggle between hold to crouch and crouch toggle
    public bool bCrouchToggle = false;

    //Movement Toggles
    private bool bMoveForward = false;
    private bool bMoveBackward = false;
    private bool bMoveLeft = false;
    private bool bMoveRight = false;
    private bool bIsCrouched = false;
    private bool bIsJumping = false;

    // Start is called before the first frame update
    void Start()
    {
        vMovementDirection = Vector3.zero;
        mCharacter = gameObject.GetComponent<CharacterController>();
        raycastHitInfo = new RaycastHit();
        ResetCharacter();
    }

    private void Update()
    {
        InputRotation();
        InputMovement();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ApplyMovement();
        ApplySlope();

        if(bIsJumping)
            ApplyJump();

        ApplyFriction();
        ApplyGravity();

        //Apply external forces and move character
        mCharacter.Move((vMovementDirection + exVel) * Time.deltaTime);

        //Reset external velocities
        exVel = Vector3.zero;
    }

    void InputRotation()
    {
        float rotationX = Input.GetAxis("Mouse X") * fTurnSpeed * Time.deltaTime;
        vInputAngle = new Vector3(0.0f, vInputAngle.y + rotationX, 0.0f);
        transform.localEulerAngles = vInputAngle;
    }

    void InputMovement()
    {
        //Movement Inputs
        if (Input.GetKey(KeyCode.W))
        {
            bMoveForward = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            bMoveBackward = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            bMoveLeft = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            bMoveRight = true;
        }
        //Jump Input
        if (nJumpCount < nJumpMax && Input.GetKeyDown(KeyCode.Space))
            bIsJumping = true;
        
        //Crouch Input
        if (bCrouchToggle)
            CheckCrouchToggle();
        else
            CheckCrouchHold();
    }

    void ApplyMovement()
    {
        //Forward
        if (bMoveForward)
        {
            vMovementDirection += MoveChar(mCharacter.transform.forward);
            bMoveForward = false;
        }
        //Backward
        if (bMoveBackward)
        {
            vMovementDirection += MoveChar(Invert(mCharacter.transform.forward));
            bMoveBackward = false;
        }
        //Left
        if (bMoveLeft)
        {
            vMovementDirection += MoveChar(Invert(mCharacter.transform.right));
            bMoveLeft = false;
        }
        //Right
        if (bMoveRight)
        {
            vMovementDirection += MoveChar(mCharacter.transform.right);
            bMoveRight = false;
        }

        //Reset jumping
        if (!bIsJumping && mCharacter.isGrounded)
        {
            vMovementDirection.y = 0;
            nJumpCount = 0;
        }
    }

    void ApplyGravity()
    {
        if (!mCharacter.isGrounded)
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

    void ApplySlope()
    {
        ray = new Ray(transform.position, Vector3.down);
        Physics.Raycast(ray, out raycastHitInfo, 1);

        //Prevents character from 'bouncing' when going down a slope
        if (raycastHitInfo.normal.y < 1)
            vMovementDirection.y -= raycastHitInfo.normal.normalized.y;
    }

    void ApplyJump()
    {
        //If the player has double or triple jump, zero out vertical momentum before applying
        if (1 < nJumpMax)
            vMovementDirection.y = 0;

        vMovementDirection += new Vector3(0, fJumpHeight);
        nJumpCount += 1;
        bIsJumping = false;
    }

    Vector3 MoveChar(Vector3 direction)
    {
        return direction * fMoveSpeed;
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
        exVel = v;
    }
}
