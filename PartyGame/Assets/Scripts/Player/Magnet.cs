using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum CurrentState { Idle, Lowering, Picking, Returning }

public class Magnet : MonoBehaviour
{
    public GameObject magnet;
    public float pressCooldown = 1.0f;
    public float pullSpeed = 150f;

    private bool hasPoints = false;
    private bool canPress = true;
    public Transform pullPosition;
    public CurrentState currentState = CurrentState.Idle;

    public GameObject vfx;
    public Animator anim;

    public List<Point> points = new();

    void Start()
    {
    }

    void FixedUpdate()
    {
    }

    public void OnPlayerClick()
    {
        if (!canPress || currentState != CurrentState.Idle) return;

        canPress = false; // Disable pressing
        Invoke(nameof(ResetPressCooldown), pressCooldown); // Schedule re-enabling pressing

        if (hasPoints)
        {
            DropPoints();
        }
        else
        {
            anim.Play("ShipMagnet", 0, 0.0f);
        }
    }

    private void ResetPressCooldown()
    {
        canPress = true;
    }

    private void DropPoints()
    {
        hasPoints = false;
        foreach (var point in points)
        {
            point.GetReleased();
        }
        points.Clear();
        Debug.Log("Points dropped!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentState != CurrentState.Picking) return;
        if (!other.CompareTag("Point")) return;

        hasPoints = true;
        var point = other.GetComponent<Point>();
        point.GetPickedUP(pullPosition, pullSpeed);
        points.Add(point);
        Debug.Log("Point picked up!");
    }

    public void FinishAnimation()
    {
        currentState = CurrentState.Idle;
    }

    public void StartDrop()
    {
        currentState = CurrentState.Lowering;
    }

    public void ActivateCollider()
    {
        GetComponent<Collider>().enabled = true;
        currentState = CurrentState.Picking;
        vfx.SetActive(true);
    }

    public void DeactivateCollider()
    {
        GetComponent<Collider>().enabled = false;
        currentState = CurrentState.Returning;
        Invoke("ResetPressCooldown", pressCooldown);
        vfx.SetActive(false);
    }
}
