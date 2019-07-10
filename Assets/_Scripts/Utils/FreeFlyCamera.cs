using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FreeFlyCamera : MonoBehaviour
{

    [Space]

    public bool _active = true;

    [Space]

    public bool _enableMovement = true;

    [SerializeField]
    private float _boostedSpeed = 50f;



    //global inputs
    [HideInInspector]
    public float _horizontalInputMSACC;
    [HideInInspector]
    public float _verticalInputMSACC;
    [HideInInspector]
    public float _scrollInputMSACC;

    Vector2 cameraRotationFly;

    [Header("Configure Inputs")]
    [Tooltip("The input that will define the horizontal movement of the cameras.")]
    public string inputMouseX = "Mouse X";
    [Tooltip("The input that will define the vertical movement of the cameras.")]
    public string inputMouseY = "Mouse Y";
    [Tooltip("The input that allows you to zoom in or out of the camera.")]
    public string inputMouseScrollWheel = "Mouse ScrollWheel";

    [Header("Inputs")]
    [Tooltip("Here you can configure the 'Horizontal' inputs that should be used to move the camera 'CameraFly'.")]
    public string horizontalMove = "Horizontal";
    [Tooltip("Here you can configure the 'Vertical' inputs that should be used to move the camera 'CameraFly'.")]
    public string verticalMove = "Vertical";
    [Tooltip("Here you can configure the keys that must be pressed to accelerate the movement of the camera 'CameraFly'.")]
    public KeyCode speedKeyCode = KeyCode.LeftShift;
    [Tooltip("Here you can configure the key that must be pressed to move the camera 'CameraFly' up.")]
    public KeyCode moveUp = KeyCode.E;
    [Tooltip("Here you can configure the key that must be pressed to move the camera 'CameraFly' down.")]
    public KeyCode moveDown = KeyCode.Q;
    //
    [Header("Settings")]
    [Range(1, 20)]
    [Tooltip("Horizontal camera rotation sensitivity.")]
    public float sensibilityX = 10.0f;
    [Range(1, 20)]
    [Tooltip("Vertical camera rotation sensitivity.")]
    public float sensibilityY = 10.0f;
    [Range(1, 100)]
    [Tooltip("The speed of movement of this camera.")]
    public float movementSpeed = 20.0f;
    [Space(7)]
    [Tooltip("If this variable is true, the X-axis input will be inverted.")]
    public bool invertXInput = false;
    [Tooltip("If this variable is true, the X-axis input will be inverted.")]
    public bool invertYInput = false;

    private float curSpeed;
    private void Start()
    {
        curSpeed = movementSpeed;
    }


    

    private void Update()
    {
        if (!_active)
            return;

        if (Input.GetKeyUp(speedKeyCode))
        {
            this.curSpeed = movementSpeed;
        }

        _horizontalInputMSACC = Input.GetAxis(inputMouseX);
        _verticalInputMSACC = Input.GetAxis(inputMouseY);
        _scrollInputMSACC = Input.GetAxis(inputMouseScrollWheel);
        _horizontalInputMSACC = Mathf.Clamp(_horizontalInputMSACC, -1, 1);
        _verticalInputMSACC = Mathf.Clamp(_verticalInputMSACC, -1, 1);
        _scrollInputMSACC = Mathf.Clamp(_scrollInputMSACC, -1, 1);

        float xInputFly = _horizontalInputMSACC;
        float yInputFly = _verticalInputMSACC;
        if (invertXInput)
        {
            xInputFly = -_horizontalInputMSACC;
        }
        if (invertYInput)
        {
            yInputFly = -_verticalInputMSACC;
        }
        //
        cameraRotationFly.x += xInputFly * sensibilityX * 15 * Time.deltaTime;
        cameraRotationFly.y += yInputFly * sensibilityY * 15 * Time.deltaTime;
        cameraRotationFly.y = Mathf.Clamp(cameraRotationFly.y, -90, 90);
        transform.rotation = Quaternion.AngleAxis(cameraRotationFly.x, Vector3.up);
        transform.rotation *= Quaternion.AngleAxis(cameraRotationFly.y, Vector3.left);
        //
        if (Input.GetKey(speedKeyCode))
        {
            curSpeed += 2f;
            if (curSpeed > _boostedSpeed)
            {
                curSpeed = _boostedSpeed;
            }
        }
        if (_enableMovement)
        {
            transform.position += transform.right * curSpeed * Input.GetAxis(horizontalMove) * Time.deltaTime;
            transform.position += transform.forward * curSpeed * Input.GetAxis(verticalMove) * Time.deltaTime;
        }
        //
        if (Input.GetKey(moveUp))
        {
            transform.position += Vector3.up * curSpeed * Time.deltaTime;
        }
        if (Input.GetKey(moveDown))
        {
            transform.position -= Vector3.up * curSpeed * Time.deltaTime;
        }


    }
}
