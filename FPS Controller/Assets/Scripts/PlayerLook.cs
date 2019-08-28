using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    // PRIVATE INIT     
    [SerializeField]
    private string mouseXInputName, mouseYInputName;
    [SerializeField]
    private float mouseSensitivity;

    private float xAxisClamp;

    [SerializeField]
    private Transform playerBody;

    private void Awake()
    {
        LockCursor();
        xAxisClamp = 0.0f;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        // получаем входные данные
        float mouseX = Input.GetAxis(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(mouseYInputName) * mouseSensitivity * Time.deltaTime;

        // ограничиваем вращение по y
        xAxisClamp += mouseY;

        if (xAxisClamp > 90.0f)
        {
            xAxisClamp = 90;
            mouseY = 0f;
            ClampXAxisRotationToValue(270);
        } else if (xAxisClamp < -90.0f)
        {
            xAxisClamp = -90;
            mouseY = 0f;
            ClampXAxisRotationToValue(90);

        }

        // вращаем камеру
        transform.Rotate(Vector3.left * mouseY);

        // вращаем игрока
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void ClampXAxisRotationToValue(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }
}
