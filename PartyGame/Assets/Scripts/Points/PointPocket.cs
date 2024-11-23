using UnityEngine;

public enum PocketColor { Blue, Red}

public class PointPocket : MonoBehaviour
{
    public PocketColor team;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: " + other.name);
        if (!other.CompareTag("Point")) return;

        GameManager.Instance.ScorePoint(team, other.GetComponent<Point>());
    }
}
