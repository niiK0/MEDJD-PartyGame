using System;
using System.Collections.Generic;
using System.Linq;
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
    private Dictionary<int, bool> availableIDs = new Dictionary<int, bool>() { {1, true }, {2, true }};

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

    public void LeaveRoom(PlayerInput player)
    {
        //players.Remove(player);
        Destroy(player.gameObject);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        Debug.Log("Player left: " + playerInput.devices[0].displayName);

        if (!players.Contains(playerInput.GetComponent<PlayerMovement>())) return;
        availableIDs[playerInput.GetComponent<PlayerMovement>().playerID] = true;
        players.Remove(playerInput.GetComponent<PlayerMovement>());
        mainMenuUI.SetPlayerReady(playerInput.GetComponent<PlayerMovement>().playerID, ControllerType.None, false);
    }

    public int FindAvailableID()
    {
        if(!availableIDs.ContainsValue(true)) return -1;

        return availableIDs.First(x => x.Value == true).Key;
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("Player joined: " + playerInput.devices[0].displayName);

        if (players.Contains(playerInput.GetComponent<PlayerMovement>())) return;

        int playerID = FindAvailableID();
        if (playerID == -1) return;

        players.Add(playerInput.GetComponent<PlayerMovement>());
        playerInput.GetComponent<PlayerMovement>().playerID = playerID;
        availableIDs[playerID] = false;

        ControllerType controllerType = ControllerType.None;

        InputDevice deviceType = InputSystem.devices.ToArray().ToList<InputDevice>().Find(x => x.deviceId == playerInput.devices[0].deviceId);

        if (deviceType is Keyboard)
        {
            controllerType = ControllerType.KBM;
        }

        if(deviceType is Gamepad)
        {
            controllerType = ControllerType.GPAD;
        }

        mainMenuUI.SetPlayerReady(playerInput.GetComponent<PlayerMovement>().playerID, controllerType, true);
    }

    public void StartGame()
    {
        //START GAME HERE
        //STOP CONTROLLER JOIN
        mainMenuUI.GetComponent<UIDocument>().enabled = false;
        inputManager.DisableJoining();
        foreach (var player in players)
        {
            player.canLeave = false;
        }
        Debug.Log("Game Started");
    }
}
