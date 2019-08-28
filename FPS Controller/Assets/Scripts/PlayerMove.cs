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
        float horizInput = Input.GetAxis(horizontalInputName) * movementSpeed;
        float vertInput = Input.GetAxis(verticalInputName) * movementSpeed;

        // calculating movement
        Vector3 forwardMovement = transform.forward * vertInput;
        Vector3 rightMovement = transform.right * horizInput;

        // perfoming a movement
        charController.SimpleMove(forwardMovement + rightMovement);

        JumpInput();
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
