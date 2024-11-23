using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public enum CurrentState { Idle, Lowering, Picking, Returning }

public class Magnet : MonoBehaviour
{
    public GameObject magnet;
    public float resetYValue;
    public float activateYValue;
    public float moveSpeed = 2.0f;
    public float returnDelay = 2.0f;
    public float pressCooldown = 1.0f; // Cooldown duration
    public float pullSpeed = 150f;

    private bool isMoving = false;
    private bool hasPoints = false;
    private bool canPress = true; // Determines if the player can press
    public Transform pullPosition;
    private Vector3 targetPosition;
    public CurrentState currentState = CurrentState.Idle;

    public List<Point> points = new();

    void Start()
    {
        magnet.transform.localPosition = new Vector3(0, resetYValue, 0);
    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            magnet.transform.localPosition = Vector3.Lerp(
                magnet.transform.localPosition,
                targetPosition,
                moveSpeed * Time.fixedDeltaTime
            );

            if (Vector3.Distance(magnet.transform.localPosition, targetPosition) < 0.01f)
            {
                magnet.transform.localPosition = targetPosition;

                if (currentState == CurrentState.Lowering)
                {
                    currentState = CurrentState.Picking;
                    GetComponent<Collider>().enabled = true;
                    isMoving = false;
                    Invoke(nameof(StartReturn), returnDelay);
                }
                else if (currentState == CurrentState.Returning)
                {
                    currentState = CurrentState.Idle;
                    isMoving = false;
                }
            }
        }
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
            StartLowering();
        }
    }

    private void ResetPressCooldown()
    {
        canPress = true;
    }

    private void StartLowering()
    {
        targetPosition = new Vector3(0, activateYValue, 0);
        currentState = CurrentState.Lowering;
        isMoving = true;
    }

    private void StartReturn()
    {
        targetPosition = new Vector3(0, resetYValue, 0);
        currentState = CurrentState.Returning;
        isMoving = true;
        GetComponent<Collider>().enabled = false;
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
}
