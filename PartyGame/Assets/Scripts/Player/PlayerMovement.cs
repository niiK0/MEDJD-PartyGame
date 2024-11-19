using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public PlayerInput playerInput;
    public CharacterController playerController;
    public float moveSpeed;

    private Vector2 moveInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<CharacterController>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        GetMovement();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 moveInputV3 = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
        playerController.Move(moveInputV3 * Time.deltaTime);
    }

    private void GetMovement()
    {
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
    }
}
