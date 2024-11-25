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

    private bool timerStarted = false;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        timer = document.rootVisualElement.Q<Label>("Timer");
        p1_score = document.rootVisualElement.Q<Label>("RedScore");
        p2_score = document.rootVisualElement.Q<Label>("BlueScore");
        winnerText = document.rootVisualElement.Q<Label>("winnerText");
        winnerRoot = document.rootVisualElement.Q<VisualElement>("WinnerDisplay");
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
        p1_score.text = score.ToString();
    }

    public void UpdateBlueScore(int score)
    {
        p2_score.text = score.ToString();
    }

    public void ShowWinner(bool show)
    {
        winnerRoot.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        timerStarted = false;
        StopAllCoroutines();
    }

    public void SetBlueWinner()
    {
        winnerText.text = "<color='blue'>BLUE</color> <color='white'>WINS!</color>";
    }

    public void SetRedWinner()
    {
        winnerText.text = "<color='red'>RED</color> <color='white'>WINS!</color>";
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
    }

    private IEnumerator TimerAnimation()
    {
        while (true)
        {
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
