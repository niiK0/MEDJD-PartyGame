using UnityEngine;

public class Point : MonoBehaviour
{
    public int pointsToGive;
    private Rigidbody rb;
    public PlayerMovement holder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ActivateSelf()
    {
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.linearDamping = 0f;
    }

    public void RespawnSelf(Vector3 spawnPos)
    {
        rb.useGravity = false;
        rb.position = spawnPos;
        rb.linearVelocity = Vector3.zero;
        rb.linearDamping = 5f;
    }
}
