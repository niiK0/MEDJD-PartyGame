using UnityEngine;

public enum PocketColor { Blue, Red}

public class PointPocket : MonoBehaviour
{
    public PocketColor team;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Point")) return;

        Debug.Log("Collided with: " + other.name);

        GameManager.Instance.ScorePoint(team, other.GetComponent<Point>());
    }
}
