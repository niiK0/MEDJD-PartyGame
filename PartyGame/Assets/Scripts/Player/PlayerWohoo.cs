using UnityEngine;

public class PlayerWohoo : MonoBehaviour
{
    public AudioClip audioC;

    public void PlayCheer()
    {
        AudioSource.PlayClipAtPoint(audioC, transform.localPosition, 1f);
    }
}
