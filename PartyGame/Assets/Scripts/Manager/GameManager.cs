using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerInputManager inputManager;
    public MainMenuUI mainMenuUI;

    private List<PlayerMovement> players = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
    }

    public void LeaveRoom(PlayerMovement player)
    {
        //players.Remove(player);
        Destroy(player.gameObject);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.Log("Player left: " + playerInput.devices[0].displayName);

        if (!players.Contains(playerInput.GetComponent<PlayerMovement>())) return;
        players.Remove(playerInput.GetComponent<PlayerMovement>());
        mainMenuUI.SetPlayerReady(playerInput.GetComponent<PlayerMovement>().playerID, ControllerType.None, false);
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("Player joined: " + playerInput.devices[0].displayName);

        if (players.Contains(playerInput.GetComponent<PlayerMovement>())) return;

        players.Add(playerInput.GetComponent<PlayerMovement>());
        playerInput.GetComponent<PlayerMovement>().playerID = players.Count;

        ControllerType controllerType = ControllerType.None;

        if (playerInput.devices[0].displayName == "Keyboard")
        {
            controllerType = ControllerType.KBM;
        }
        else
        {
            controllerType = ControllerType.GPAD;
        }

        mainMenuUI.SetPlayerReady(playerInput.GetComponent<PlayerMovement>().playerID, controllerType, true);
    }

    public void StartGame()
    {
        //START GAME HERE
        Debug.Log("Game Started");
    }
}
