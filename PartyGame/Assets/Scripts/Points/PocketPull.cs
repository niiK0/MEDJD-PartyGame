using UnityEngine;

public class PocketPull : MonoBehaviour
{
    public float pullSpeed = 2.0f; // Speed of pulling effect
    public float pullRadius = 5.0f; // Range within which the pull effect applies

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Point")) return;

        var point = other.GetComponent<Point>();

        if (point.holder != null) return;
        if (point.isBeingPulled) return;

        Debug.Log("Pulling " + other.name);

        ApplyPull(point);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Point")) return;

        var point = other.GetComponent<Point>();

        if (point.holder != null) return;

        point.StopPulling();
    }

    private void ApplyPull(Point point)
    {
        point.StartPullingTowards(transform.position, pullSpeed);
    }
}
