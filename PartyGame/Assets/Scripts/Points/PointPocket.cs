using UnityEngine;

public enum PocketColor { Blue, Red}

public class PointPocket : MonoBehaviour
{
    public PocketColor team;
    public ParticleSystem vfx;
    public AudioSource audioS;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Point")) return;

        Debug.Log("Collided with: " + other.name);

        GameManager.Instance.ScorePoint(team, other.GetComponent<Point>());
        vfx.Play();
        //audioS.Play();
        AudioSource.PlayClipAtPoint(audioS.clip, transform.position, audioS.volume);
    }
}
