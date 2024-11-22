
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerInputManager inputManager;
    public MainMenuUI mainMenuUI;

    //PLAYERS
    private List<PlayerMovement> players = new();
    private Dictionary<int, bool> availableIDs = new Dictionary<int, bool>() { {1, true }, {2, true }};

    //POINTS
    public List<Point> points = new();
    public List<Point> pointsForRespawn = new();
    public GameObject goldenPointPrefab;
    public GameObject normalPointPrefab;
    public int normalPointsAmount;
    public int goldenPointsAmount;
    public int pointsAliveForRespawn;
    public Transform upCornerArena;
    public Transform downCornerArena;
    public Transform pointsParent;

    //TEAMS
    public int blueTeamPoints;
    public int redTeamPoints;

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
        SetupPoints();
    }

    public void SetupPoints()
    {
        for (int i = 0; i < normalPointsAmount; i++)
        {
            Vector3 randPos = GetRandomSpawnPosition();
            Debug.Log("Instantiating point at: " + randPos);
            GameObject spawnedPointObj = Instantiate(normalPointPrefab, pointsParent);
            Point spawnedPoint = spawnedPointObj.GetComponent<Point>();
            points.Add(spawnedPoint);
            spawnedPoint.RespawnSelf(randPos);
        }

        for (int i = 0; i < goldenPointsAmount; i++)
        {
            Vector3 randPos = GetRandomSpawnPosition();
            GameObject spawnedPointObj = Instantiate(goldenPointPrefab, pointsParent);
            Point spawnedPoint = spawnedPointObj.GetComponent<Point>();
            points.Add(spawnedPoint);
            spawnedPoint.RespawnSelf(randPos);
        }
    }

    public void ScorePoint(PocketColor team, Point point)
    {
        if(team == PocketColor.Blue)
        {
            blueTeamPoints += point.pointsToGive;
        }
        else
        {
            redTeamPoints += point.pointsToGive;
        }

        points.Remove(point);
        pointsForRespawn.Add(point);
        Vector3 randPos = GetRandomSpawnPosition();
        point.RespawnSelf(randPos);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float randX = Random.Range(upCornerArena.position.x, downCornerArena.position.x);
        float randZ = Random.Range(upCornerArena.position.z, downCornerArena.position.z);
        Vector3 randPos = new Vector3(randX, upCornerArena.position.y, randZ);
        return randPos;
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

        foreach(var point in points)
        {
            point.ActivateSelf();
        }

        Debug.Log("Game Started");
    }
}
