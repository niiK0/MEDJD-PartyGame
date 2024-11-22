using UnityEngine;

public enum PocketColor { Blue, Red}

public class PointPocket : MonoBehaviour
{
    public PocketColor team;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Point") return;

        GameManager.Instance.ScorePoint(team, other.GetComponent<Point>());
    }
}
