using System;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class Point : MonoBehaviour
{
    public int pointsToGive;
    private Rigidbody rb;
    public bool isBeingPulled = false;
    private Vector3 pullTarget;
    public float pullSpeed;

    public Transform holder;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (isBeingPulled)
            PullPoint();
    }

    private void PullPoint()
    {
        Vector3 direction = (pullTarget - transform.localPosition).normalized;
        rb.linearVelocity = direction * pullSpeed * Time.fixedDeltaTime;
    }

    void Update()
    {
        if (isBeingPulled)
        {
            if (Vector3.Distance(transform.localPosition, pullTarget) < 0.1f)
            {
                isBeingPulled = false;
                rb.linearVelocity = Vector3.zero;
            }
        }
    }

    public void StartPullingTowards(Vector3 target, float speed)
    {
        pullTarget = target;
        pullSpeed = speed;
        isBeingPulled = true;
        rb.useGravity = false;
    }

    public void StopPulling()
    {
        isBeingPulled = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void ActivateSelf()
    {
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.linearDamping = 0f;
        GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.On;
    }

    public void RespawnSelf(Vector3 spawnPos)
    {
        StopPulling();
        rb.useGravity = false;
        rb.position = spawnPos;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.linearDamping = 5f;
        GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.Off;
    }

    public void GetPickedUP(Transform magnet, float speed)
    {
        holder = magnet;
        transform.parent = holder;
        StartPullingTowards(magnet.localPosition, speed);
    }

    public void GetReleased()
    {
        rb.useGravity = true;
        holder = null;
        transform.parent = GameManager.Instance.pointsParent;
        StopPulling();
    }
}
