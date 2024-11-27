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

    public AudioSource audioS;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 5f;
        rb.linearDamping = 0.5f;
        rb.angularDamping = 4f;
        audioS = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (isBeingPulled)
            PullPoint();
    }

    private void PullPoint()
    {
        Vector3 direction = (pullTarget - transform.localPosition).normalized;
        rb.AddForce(direction * pullSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    void Update()
    {
        //if (isBeingPulled && Vector3.Distance(transform.localPosition, pullTarget) < 0.1f)
        //{
        //    isBeingPulled = false;
        //    rb.linearVelocity = Vector3.zero;
        //}
    }

    public void StartPullingTowards(Vector3 target, float speed)
    {
        pullTarget = target;
        pullSpeed = speed;
        isBeingPulled = true;
        rb.useGravity = false;
        //rb.linearVelocity = Vector3.zero;
        //rb.angularVelocity = Vector3.zero;
    }

    public void StopPulling()
    {
        isBeingPulled = false;
    }

    public void ActivateSelf()
    {
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.None;
        GetComponent<MeshRenderer>().shadowCastingMode = ShadowCastingMode.On;
        Invoke("AdjustFallingSpeed", 0.5f);
    }

    public void AdjustFallingSpeed()
    {
        rb.linearDamping = 0.1f;
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

    private void OnCollisionEnter(Collision collision)
    {
        if(audioS.isPlaying) return;
        audioS.Play();
    }
}
