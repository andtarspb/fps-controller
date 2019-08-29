using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField]
    private string horizontalInputName;
    [SerializeField]
    private string verticalInputName;
    [SerializeField]
    private float movementSpeed;

    [SerializeField]
    private float slopeForce;
    [SerializeField]
    private float slopeForceRayLength;

    private CharacterController charController;

    [SerializeField]
    private AnimationCurve jumpFallOff;
    [SerializeField]
    private float jumpMultiplier;
    [SerializeField]
    private KeyCode jumpKey;

    private bool isJumping;

    private float initSlopeLimit;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
        initSlopeLimit = charController.slopeLimit;
    }

    private void Update()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        // получаем входные данные
        float horizInput = Input.GetAxis(horizontalInputName);
        float vertInput = Input.GetAxis(verticalInputName);

        // calculating movement
        Vector3 forwardMovement = transform.forward * vertInput;
        Vector3 rightMovement = transform.right * horizInput;

        // perfoming a movement
        charController.SimpleMove(Vector3.ClampMagnitude(forwardMovement + rightMovement, 1.0f) * movementSpeed);

        // применение дополнительной силы на склонах
        if ((vertInput != 0 || horizInput != 0) && OnSlope())
            charController.Move(Vector3.down * charController.height / 2 * slopeForce * Time.deltaTime);


        JumpInput();        
    }

    private bool OnSlope()
    {
        if (isJumping)
            return false;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, charController.height / 2 * slopeForceRayLength))
            if (hit.normal != Vector3.up)
                return true ;

        return false;
    }

    private void JumpInput()
    {
        if (Input.GetKeyDown(jumpKey) && !isJumping)
        {
            isJumping = true;
            StartCoroutine(JumpEvent());
        }
    }

    private IEnumerator JumpEvent()
    {
        charController.slopeLimit = 90;
        float timeInTheAir = 0;

        do
        {
            float jumpForce = jumpFallOff.Evaluate(timeInTheAir);
            charController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
            timeInTheAir += Time.deltaTime;

            yield return null;
        }
        while (!charController.isGrounded && charController.collisionFlags != CollisionFlags.Above);

        charController.slopeLimit = initSlopeLimit;
        isJumping = false;
    }
}
