using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR.Haptics;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class PlayerMovement : MonoBehaviour
{
    public int playerID;
    public PlayerInput playerInput;
    public CharacterController playerController;
    public float moveSpeed;
    public float baseMoveSpeed;
    public bool canLeave = true;
    public Magnet magnet;
    public float maxSpeedReduction = 0.5f;
    public int maxPointsForPenalty = 10;

    public GameObject redVersion;
    public GameObject blueVersion;

    public Vector2 moveInput;

    public bool isGettingPushed = false;

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
        GetPauseInput();
    }

    private void FixedUpdate()
    {
        if(!isGettingPushed)
            MovePlayer();
    }

    public void EnableRedVersion()
    {
        redVersion.SetActive(true);
        blueVersion.SetActive(false);
    }

    public void EnableBlueVersion()
    {
        redVersion.SetActive(false);
        blueVersion.SetActive(true);
    }

    private void MovePlayer()
    {
        if (magnet.points.Count > 0)
        {
            float reductionFactor = Mathf.Clamp01(magnet.points.Count / (float)maxPointsForPenalty);
            moveSpeed = baseMoveSpeed * (1.0f - (reductionFactor * maxSpeedReduction));
        }
        else
        {
            moveSpeed = baseMoveSpeed;
        }
        Vector3 moveInputV3 = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;
        playerController.Move(moveInputV3 * Time.fixedDeltaTime);
        Vector3 curPos = transform.position;
        curPos.y = 3f;
        transform.position = curPos;
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

            GetComponentInChildren<Magnet>().OnPlayerClick();
        }
    }

    private void GetPauseInput()
    {
        float pauseInput = playerInput.actions["Pause"].ReadValue<float>();
        if (pauseInput == 1)
        {
            if (!canLeave)
                GameManager.Instance.PauseGame();
        }
    }

    public void TeleportSelf(Vector3 tpPos)
    {
        playerController.Move(tpPos);
    }

    public void PlayerLeave(PlayerInput input)
    {
        GameManager.Instance.LeaveRoom(input);
    }
}
