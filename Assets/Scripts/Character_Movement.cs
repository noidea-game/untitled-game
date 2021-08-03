using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character_Movement : MonoBehaviour
{
    private CharacterController mCharacter;
    private Vector3 vVerticalVelocity;
    private bool bIsCrouched;
    private Vector3 vInputAngle;
    private Vector3 vMovementDirection;
    private Ray ray;
    private RaycastHit raycastHitInfo;

    public enum CharacterState { idle = 0, walking, jumping, crouching };
    public CharacterState currentState;
    CharacterState nextState;

    public Camera mCamera;
    public float fWalkSpeed = 1.0f;
    public float fTurnSpeed = 1.0f;
    public float fJumpHeight = 1.5f;
    public bool bCrouchToggle = false;
    public float fFriction = 1.0f;
    public Animator animator;

    public Text DebugText { get; set; }
    private Vector3 JumpVelocity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        vMovementDirection = Vector3.zero;
        mCharacter = gameObject.GetComponent<CharacterController>();
        raycastHitInfo = new RaycastHit();
        currentState = CharacterState.walking;
        DebugText = GameObject.Find("DebugText").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        vMovementDirection = mCharacter.velocity;

        switch(currentState)
        {
            case CharacterState.walking:
                {
                    UpdateStance();
                    ApplySlope();
                    //Prevents character from popping up when going up a slope.
                    GroundClamp();
                    break;
                }
            case CharacterState.jumping:
                {
                    if (mCharacter.isGrounded)
                        currentState = CharacterState.walking;
                    break;
                }
        }

        RotateCharacter();
        MoveCharacter();
        ApplyFriction();
        ApplyGravity();

        if(!mCharacter.isGrounded)
        {
            //vMovementDirection *= .2;
            JumpVelocity.y -= Physics.gravity.y * Time.deltaTime;
        }

        mCharacter.Move((vMovementDirection) * Time.deltaTime);
    }

    void RotateCharacter()
    {
        float rotationX = Input.GetAxis("Mouse X") * fTurnSpeed * Time.deltaTime;
        vInputAngle = new Vector3(0.0f, vInputAngle.y + rotationX, 0.0f);
        transform.localEulerAngles = vInputAngle;
    }

    void ApplyGravity()
    {
        vMovementDirection += (Physics.gravity * Time.deltaTime);
        //if (!mCharacter.isGrounded)
        //    vMovementDirection += (Physics.gravity * Time.deltaTime);
        //else
        //    vMovementDirection += Physics.gravity.normalized;

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
        if (Input.GetKey(KeyCode.W) && currentState != CharacterState.jumping)
        {
            vMovementDirection += Walk(mCharacter.transform.forward);
        }

        if (Input.GetKey(KeyCode.S) && currentState != CharacterState.jumping)
        {
            vMovementDirection += Walk(Invert(mCharacter.transform.forward));
        }

        if (Input.GetKey(KeyCode.A) && currentState != CharacterState.jumping)
        {
            vMovementDirection += Walk(Invert(mCharacter.transform.right));
        }

        if (Input.GetKey(KeyCode.D) && currentState != CharacterState.jumping)
        {
            vMovementDirection += Walk(mCharacter.transform.right);
        }

        if (mCharacter.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            vMovementDirection += Jump();
            currentState = CharacterState.jumping;
        }
        else
        {
            JumpVelocity = Vector3.zero;
        }

        if(currentState == CharacterState.jumping)
        {
            vMovementDirection += mCharacter.transform.forward;
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
    int i = 1;
    Vector3 Jump()
    {
        DebugText.text = $"Jump! {i.ToString()}";
        i++;
        JumpVelocity = vMovementDirection;
        JumpVelocity.y = 2;

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
}
