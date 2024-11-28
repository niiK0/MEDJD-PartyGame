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
    public AudioSource moveAudio;
    public float moveSpeed;
    public float baseMoveSpeed;
    public bool canLeave = true;
    public Magnet magnet;
    public float maxSpeedReduction = 0.5f;
    public int maxPointsForPenalty = 10;

    public GameObject redVersion;
    public GameObject blueVersion;

    public Vector2 moveInput;
    public GameObject visuals;
    public GameObject playerHead;
    public GameObject playerHeadBlue;
    public GameObject playerHeadOrange;
    public Animator playerAnimator;
    public float tiltAmount = 1f;
    public float tiltAmountFront = 5f;
    public float headUDTurnAmount = 20f;
    public float headLRTurnAmount = 60f;

    public bool isGettingPushed = false;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        playerController = GetComponent<CharacterController>();
        moveAudio = GetComponent<AudioSource>();
        canLeave = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput.onDeviceLost += PlayerLeave;
        moveAudio.Stop();
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

        SetPlayerHeight();
    }

    private void SetPlayerHeight()
    {
        Vector3 curPos = transform.position;
        curPos.y = 3f;
        transform.position = curPos;
    }

    public void EnableRedVersion()
    {
        redVersion.SetActive(true);
        blueVersion.SetActive(false);
        playerHead = playerHeadOrange;
        playerAnimator = redVersion.GetComponentInChildren<Animator>();
    }

    public void EnableBlueVersion()
    {
        redVersion.SetActive(false);
        blueVersion.SetActive(true);
        playerHead = playerHeadBlue;
        playerAnimator = blueVersion.GetComponentInChildren<Animator>();
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

        if(moveInputV3 != Vector3.zero)
        {
            if (!moveAudio.isPlaying)
                moveAudio.Play();
        }
        else
        {
            if(moveAudio.isPlaying)
                moveAudio.Stop();
        }

        TiltVisuals(moveInputV3);
    }

    private void GetMovement()
    {
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();
    }

    private void GetInteractInput()
    {
        float leaveInput = playerInput.actions["Interact"].ReadValue<float>();
        if(leaveInput == 1 && !isGettingPushed)
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
        transform.position = tpPos;
        Debug.Log("Teleported player to " + tpPos);
    }

    public void PlayerLeave(PlayerInput input)
    {
        GameManager.Instance.LeaveRoom(input);
    }

    private void TiltVisuals(Vector3 movementDirection)
    {
        if (movementDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.Euler(new Vector3(
                tiltAmountFront * movementDirection.z,
                0,
                tiltAmount * -movementDirection.x
            ));

            visuals.transform.rotation = Quaternion.Lerp(
                visuals.transform.rotation,
                targetRotation,
                Time.deltaTime * 10f
            );
        }
        else
        {
            visuals.transform.rotation = Quaternion.Lerp(
                visuals.transform.rotation,
                Quaternion.identity, // Neutral position
                Time.deltaTime * 10f
            );
        }

        if (playerHead != null)
        {
            if(movementDirection != Vector3.zero)
            {
                if(playerAnimator.enabled)
                    playerAnimator.enabled = false;

                Quaternion headTargetRotation = Quaternion.Euler(
                    -movementDirection.z * headUDTurnAmount,
                    -movementDirection.x * headLRTurnAmount,
                    0
                );

                playerHead.transform.localRotation = Quaternion.Lerp(
                    playerHead.transform.localRotation,
                    headTargetRotation,
                    Time.deltaTime * 10f
                );
            }
            else
            {
                playerHead.transform.localRotation = Quaternion.Lerp(
                    playerHead.transform.localRotation,
                    Quaternion.identity,
                    Time.deltaTime * 10f
                );

                if(playerHead.transform.localRotation == Quaternion.identity)
                    playerAnimator.enabled = true;
            }
        }
    }
}
