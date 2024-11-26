using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseUI : MonoBehaviour
{
    public UIDocument document;
    private Button playButton;
    private Button quitButton;
    private AudioSource audioS;

    private void Awake()
    {
        document = GetComponent<UIDocument>();

        //Play btn setup
        playButton = document.rootVisualElement.Q<Button>("resumeBtn");
        playButton.clicked += OnPlayGameClick;

        //Quit btn setup
        quitButton = document.rootVisualElement.Q<Button>("quitBtn");
        quitButton.clicked += OnQuitGameClick;
        audioS = GetComponent<AudioSource>();
    }

    private void Start()
    {
        document.rootVisualElement.style.display = DisplayStyle.None;
    }

    public void OnShow()
    {
        playButton.Focus();
    }

    private void OnEnable()
    {
        playButton.RegisterCallback<FocusEvent>(OnFocus);
        playButton.RegisterCallback<MouseEnterEvent>(MouseEnterEventFunc);
        quitButton.RegisterCallback<FocusEvent>(OnFocus);
        quitButton.RegisterCallback<MouseEnterEvent>(MouseEnterEventFunc);
    }

    private void OnDisable()
    {
        playButton.clicked -= OnPlayGameClick;
        quitButton.clicked -= OnQuitGameClick;
        playButton.UnregisterCallback<FocusEvent>(OnFocus);
        playButton.UnregisterCallback<MouseEnterEvent>(MouseEnterEventFunc);
        quitButton.UnregisterCallback<FocusEvent>(OnFocus);
        quitButton.UnregisterCallback<MouseEnterEvent>(MouseEnterEventFunc);
    }

    private void MouseEnterEventFunc(MouseEnterEvent evt)
    {
        audioS.Play();
    }

    private void OnFocus(FocusEvent evt)
    {
        audioS.Play();
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
