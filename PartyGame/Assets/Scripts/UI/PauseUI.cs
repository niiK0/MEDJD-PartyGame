using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseUI : MonoBehaviour
{
    public UIDocument document;
    private Button playButton;
    private Button quitButton;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        //Play btn setup
        playButton = document.rootVisualElement.Q<Button>("resumeBtn");
        playButton.clicked += OnPlayGameClick;

        //Quit btn setup
        quitButton = document.rootVisualElement.Q<Button>("quitBtn");
        quitButton.clicked += OnQuitGameClick;
    }

    private void Start()
    {
        document.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void OnShow()
    {
        playButton.Focus();
    }

    private void OnDisable()
    {
        playButton.clicked -= OnPlayGameClick;
        quitButton.clicked -= OnQuitGameClick;
    }

    public void OnPlayGameClick()
    {
        GameManager.Instance.ResumeGame();
    }

    public void OnQuitGameClick()
    {
        GameManager.Instance.BackToMenu();
    }
}
