using UnityEngine;

public class RotateSkybox : MonoBehaviour
{
    public float rotateAmount = 0.1f;
    // Update is called once per frame
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", rotateAmount * Time.time);
    }
}
