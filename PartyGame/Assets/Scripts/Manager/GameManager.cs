
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PlayerInputManager inputManager;
    public MainMenuUI mainMenuUI;
    public GUI gui;
    public PauseUI pauseUI;
    public AudioSource audioS;

    //TIMER
    public float timer = 30;
    public float baseTimer = 30;
    public bool startTimer = false;

    //PLAYERS
    private List<PlayerMovement> players = new();
    private Dictionary<int, bool> availableIDs = new Dictionary<int, bool>() { {1, true }, {2, true }};

    public Transform redPlayerSpawn;
    public Transform bluePlayerSpawn;
    public Transform winPosition;

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

    //TIMELINE
    public PlayableDirector timelineStart;
    public PlayableDirector timelineEnd;

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
        audioS = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ResetGame();
    }


    private void Update()
    {
        CheckRespawn();
        DoTimer();
    }

    public void CheckTimelineEnd()
    {
        timelineStart.gameObject.SetActive(false);
        StartGame();
        VolumeProfile volumeProfile = FindFirstObjectByType<Volume>()?.profile;
        DepthOfField depthOfField;
        if (volumeProfile.TryGet(out depthOfField))
        {
            depthOfField.mode.Override(DepthOfFieldMode.Gaussian);
        }
    }

    private void EndTimelineFinish()
    {
        Invoke("BackToMenu", 5f);
    }

    private void DoTimer()
    {
        if (!startTimer) return;
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            gui.UpdateTimer(Mathf.CeilToInt(timer));
            if(timer <= 5f)
            {
                gui.StartTimerAnimation();
            }
        }
        else
        {
            EndGame();
            startTimer = false;
        }

    }

    private void CheckRespawn()
    {
        if(points.Count < pointsAliveForRespawn)
        {
            RespawnPoints();
        }
    }

    private void RespawnPoints()
    {
        foreach(var point in pointsForRespawn)
        {
            points.Add(point);
            point.ActivateSelf();
        }

        pointsForRespawn.Clear();
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
            gui.UpdateBlueScore(blueTeamPoints);
        }
        else
        {
            redTeamPoints += point.pointsToGive;
            gui.UpdateRedScore(redTeamPoints);
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
        playerInput.GetComponent<PlayerMovement>().isGettingPushed = true;
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

    private IEnumerator DelayCheerAnimation(PlayerMovement player, float delay)
    {
        yield return new WaitForSeconds(delay);
        player.playerAnimator.enabled = true;
        player.playerAnimator.SetBool("doCheer", true);
    }

    public void EndGame()
    {
        if (redTeamPoints > blueTeamPoints)
        {
            gui.SetRedWinner();
            PlayerMovement player = players.Find(x => x.playerID == 1);
            player.isGettingPushed = false;
            player.TeleportSelf(winPosition.localPosition);
            StartCoroutine(DelayCheerAnimation(player, 0.75f));
            Debug.Log("Red team won!");
        }
        else if(blueTeamPoints > redTeamPoints)
        {
            gui.SetBlueWinner();
            PlayerMovement player = players.Find(x => x.playerID == 2);
            player.isGettingPushed = false;
            player.TeleportSelf(winPosition.localPosition);
            StartCoroutine(DelayCheerAnimation(player, 0.75f));
            Debug.Log("Blue team won!");
        }
        else
        {
            gui.SetDrawText();
            Debug.Log("Draw!");
        }

        VolumeProfile volumeProfile = FindFirstObjectByType<Volume>()?.profile;
        DepthOfField depthOfField;
        if (volumeProfile.TryGet(out depthOfField))
        {
            depthOfField.mode.Override(DepthOfFieldMode.Off);
        }

        gui.ShowWinner(true);

        timelineEnd.gameObject.SetActive(true);
        Invoke("EndTimelineFinish", (float)timelineEnd.duration);

        players.ForEach(x => x.isGettingPushed = true);
        audioS.Stop();
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting game...");
    }

    public void ResumeGame()
    {
        pauseUI.document.rootVisualElement.style.display = DisplayStyle.None;
        pauseUI.OnShow();
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        pauseUI.document.rootVisualElement.style.display = DisplayStyle.Flex;
        Time.timeScale = 0;
    }

    public void BackToMenu()
    {
        timelineEnd.gameObject.SetActive(false);
        ResetGame();
        Time.timeScale = 1;
    }

    public void StartGameFromMenu()
    {
        if (players.Count < 2) return;

        mainMenuUI.document.rootVisualElement.style.display = DisplayStyle.None;
        gui.document.rootVisualElement.style.display = DisplayStyle.Flex;
        inputManager.DisableJoining();

        foreach (var player in players)
        {
            player.canLeave = false;
        }

        players.Find(x => x.playerID == 1).TeleportSelf(redPlayerSpawn.position);
        players.Find(x => x.playerID == 1).EnableRedVersion();
        players.Find(x => x.playerID == 2).TeleportSelf(bluePlayerSpawn.position);
        players.Find(x => x.playerID == 2).EnableBlueVersion();

        timelineStart.gameObject.SetActive(true);
        Invoke("CheckTimelineEnd", (float)timelineStart.duration);
        VolumeProfile volumeProfile = FindFirstObjectByType<Volume>()?.profile;
        DepthOfField depthOfField;
        if(volumeProfile.TryGet(out depthOfField))
        {
            depthOfField.mode.Override(DepthOfFieldMode.Off);
        }
    }

    public void StartGame()
    {
        audioS.Play();

        SetupPoints();
        foreach(var point in points)
        {
            point.ActivateSelf();
        }

        foreach (var player in players)
        {
            player.isGettingPushed = false;
        }

        startTimer = true;
        gui.canUpdateScore = true;

        Debug.Log("Game Started");
    }

    public void ResetGame()
    {
        redTeamPoints = 0;
        blueTeamPoints = 0;
        timer = baseTimer;
        gui.ResetText(baseTimer);
        startTimer = false;
        DestroyAllPointsAndPlayers();
        mainMenuUI.document.rootVisualElement.style.display = DisplayStyle.Flex;
        gui.document.rootVisualElement.style.display = DisplayStyle.None;
        pauseUI.document.rootVisualElement.style.display = DisplayStyle.None;
        timelineStart.gameObject.SetActive(false);
        timelineEnd.gameObject.SetActive(false);
        CancelInvoke();
        gui.CancelInvoke();
        pauseUI.CancelInvoke();
        mainMenuUI.CancelInvoke();
        gui.ShowWinner(false);
        inputManager.EnableJoining();
    }

    private void DestroyAllPointsAndPlayers()
    {
        // Destroy all points safely
        for (int i = points.Count - 1; i >= 0; i--)
        {
            if(points[i] != null)
            {
                Destroy(points[i].gameObject);
            }
        }
        points.Clear();

        // Destroy all players safely
        for (int i = players.Count - 1; i >= 0; i--)
        {
            if (players[i] != null)
            {
                Destroy(players[i].gameObject);
            }
        }
        players.Clear();
    }
}
