using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public enum ControllerType {None, KBM, GPAD}

public class MainMenuUI : MonoBehaviour
{
    private UIDocument document;
    private Button playButton;
    private Button quitButton;

    public Sprite kbmSprite;
    public Sprite gpadSprite;

    private VisualElement p1_image;
    private VisualElement p2_image;

    private Label p1_ready;
    private Label p2_ready;


    private void Awake()
    {
        document = GetComponent<UIDocument>();

        //Play btn setup
        playButton = document.rootVisualElement.Q<Button>("StartBtn");
        playButton.RegisterCallback<ClickEvent>(OnPlayGameClick);

        //Quit btn setup
        quitButton = document.rootVisualElement.Q<Button>("QuitBtn");
        quitButton.RegisterCallback<ClickEvent>(OnQuitGameClick);

        //players images
        p1_image = document.rootVisualElement.Q<VisualElement>("IMG_JoinP1");
        p2_image = document.rootVisualElement.Q<VisualElement>("IMG_JoinP2");

        //players labels
        p1_ready = document.rootVisualElement.Q<Label>("P1_ReadyLabel");
        p2_ready = document.rootVisualElement.Q<Label>("P2_ReadyLabel");
    }

    private void OnDisable()
    {
        playButton.UnregisterCallback<ClickEvent>(OnPlayGameClick);
        quitButton.UnregisterCallback<ClickEvent>(OnQuitGameClick);
    }

    public void SetPlayerReady(int player, ControllerType controllerType, bool isReady)
    {
        if(player == 1)
        {
            p1_image.style.backgroundImage = controllerType == ControllerType.KBM ? new StyleBackground(kbmSprite) : new StyleBackground(gpadSprite);
            p1_image.style.visibility = isReady ? Visibility.Visible : Visibility.Hidden;
            p1_ready.style.visibility = isReady ? Visibility.Visible : Visibility.Hidden;
        }
        
        if (player == 2)
        {
            p2_image.style.backgroundImage = controllerType == ControllerType.KBM ? new StyleBackground(kbmSprite) : new StyleBackground(gpadSprite);
            p2_image.style.visibility = isReady ? Visibility.Visible : Visibility.Hidden;
            p2_ready.style.visibility = isReady ? Visibility.Visible : Visibility.Hidden;
        }
    }

    public void OnPlayGameClick(ClickEvent evt)
    {
        GameManager.Instance.StartGame();
    }

    public void OnQuitGameClick(ClickEvent evt)
    {
        Debug.Log("Quit game :D");
    }
}
