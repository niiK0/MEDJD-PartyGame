using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class GUI : MonoBehaviour
{
    public UIDocument document;

    private Label timer;
    private Label p1_score;
    private Label p2_score;
    private Label winnerText;
    private VisualElement winnerRoot;
    public AudioSource audioS;
    public AudioClip timeUp;

    private bool timerStarted = false;
    public bool canUpdateScore = true;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        timer = document.rootVisualElement.Q<Label>("Timer");
        p1_score = document.rootVisualElement.Q<Label>("RedScore");
        p2_score = document.rootVisualElement.Q<Label>("BlueScore");
        winnerText = document.rootVisualElement.Q<Label>("winnerText");
        winnerRoot = document.rootVisualElement.Q<VisualElement>("WinnerDisplay");

        audioS = GetComponent<AudioSource>();
    }

    private void Start()
    {
        document.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void UpdateTimer(int time)
    {
        timer.text = time.ToString();
    }

    public void UpdateRedScore(int score)
    {
        if(canUpdateScore)
            p1_score.text = score.ToString();
    }

    public void UpdateBlueScore(int score)
    {
        if (canUpdateScore)
            p2_score.text = score.ToString();
    }

    public void ShowWinner(bool show)
    {
        if (show)
        {
            Invoke("ShowStuffTrue", (float)GameManager.Instance.timelineEnd.duration / 3);
        }
        else
        {
            ShowStuffFalse();
        }
    }

    private void ShowStuffTrue()
    {
        canUpdateScore = false;
        timerStarted = false;
        winnerRoot.style.display = DisplayStyle.Flex;
        StopAllCoroutines();
    }

    public void ResetText(float resetTimer)
    {
        p1_score.text = "0";
        p2_score.text = "0";
        timer.text = resetTimer.ToString();
    }

    private void ShowStuffFalse()
    {
        canUpdateScore = true;
        timerStarted = false;
        winnerRoot.style.display = DisplayStyle.None;
        StopTimerAnimation();
        StopAllCoroutines();
    }

    public void SetBlueWinner()
    {
        winnerText.text = "<color='blue'>BLUE</color> <color='white'>WINS!</color>";
    }

    public void SetRedWinner()
    {
        winnerText.text = "<color='orange'>ORANGE</color> <color='white'>WINS!</color>";
    }

    public void SetDrawText()
    {
        winnerText.text = "<color='white'>ITS A DRAW!</color>";
    }

    public void StartTimerAnimation()
    {
        if (timerStarted) return;

        timerStarted = true;
        StartCoroutine(TimerAnimation());
        audioS.clip = timeUp;
    }

    private IEnumerator TimerAnimation()
    {
        while (true)
        {
            audioS.Play();
            timer.AddToClassList("timeLabelAnim");
            yield return new WaitForSecondsRealtime(0.6f);
            StopTimerAnimation();
            yield return new WaitForSecondsRealtime(0.4f);
        }
    }

    private void StopTimerAnimation()
    {
        timer.RemoveFromClassList("timeLabelAnim");
    }
}
