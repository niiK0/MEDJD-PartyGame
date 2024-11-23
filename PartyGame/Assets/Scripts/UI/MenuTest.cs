using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class MenuTest : MonoBehaviour
{

    private UIDocument document;
    private Button btn1;

    private void Awake()
    {
        document = GetComponent<UIDocument>();
    }

    private void OnEnable()
    {
        btn1 = document.rootVisualElement.Q<Button>();
        btn1.Focus();
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
