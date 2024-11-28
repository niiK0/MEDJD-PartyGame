using UnityEngine;

public enum PocketColor { Blue, Red}

public class PointPocket : MonoBehaviour
{
    public PocketColor team;
    public ParticleSystem vfx;
    public AudioSource audioS;

    private float lastCheerTime = 0f;
    public float cheerCooldown = 2.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Point")) return;

        Debug.Log("Collided with: " + other.name);

        GameManager.Instance.ScorePoint(team, other.GetComponent<Point>());
        vfx.Play();
        // audioS.Play();
        AudioSource.PlayClipAtPoint(audioS.clip, transform.position, audioS.volume);

        int teamID = team == PocketColor.Red ? 1 : 2;

        // Check if enough time has passed since the last cheer
        if (Time.time - lastCheerTime >= cheerCooldown)
        {
            if (Random.Range(0, 2) == 1)
            {
                GameManager.Instance.GetPlayerByID(teamID).playerAnimator.GetComponent<PlayerWohoo>().PlayCheer();
                lastCheerTime = Time.time; // Update the last cheer time
            }
        }
    }

}
