using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public int playerID;
    public PlayerInput playerInput;
    public CharacterController playerController;
    public float moveSpeed;
    public bool canLeave = true;

    private Vector2 moveInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<CharacterController>();
        canLeave = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput.onDeviceLost += PlayerLeave;
    }

    // Update is called once per frame
    void Update()
    {
        GetMovement();
        GetInteractInput();
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

    private void GetInteractInput()
    {
        float leaveInput = playerInput.actions["Interact"].ReadValue<float>();
        if(leaveInput == 1)
        {
            if(canLeave)
                PlayerLeave(playerInput);

            if (GetComponentInChildren<Magnet>().CanActivate())
                GetComponentInChildren<Magnet>().Activate();
        }
    }

    public void PlayerLeave(PlayerInput input)
    {
        GameManager.Instance.LeaveRoom(input);
    }
}
