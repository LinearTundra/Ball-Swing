using UnityEngine;
using UnityEngine.InputSystem;

public class PositionController : MonoBehaviour
{

    [SerializeField]
    private float speed;

    [Header("Input Settings")]
    [SerializeField]
    private InputActionAsset inputActions;

    private InputAction moveInput;


    private void Awake()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    private void Start()
    {
        moveInput = inputActions.FindAction("Move");
    }

    private void Update()
    {
        MoveMarker();
    }

    private void MoveMarker()
    {
        Vector2 moveDir = moveInput.ReadValue<Vector2>();

        transform.position += new Vector3(moveDir.x, 0, moveDir.y) * speed * Time.deltaTime;
    }

    public Vector3 GetPosition()
    {
        return transform.position; 
    }

}
