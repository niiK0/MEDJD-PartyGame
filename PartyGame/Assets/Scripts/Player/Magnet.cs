using UnityEngine;

public class Magnet : MonoBehaviour
{
    public GameObject magnet;
    public float resetYValue;
    public float activateYValue;
    public float moveSpeed = 2.0f;
    public float returnDelay = 2.0f;

    private bool isActivating = false;
    private bool isReturning = false;
    private Vector3 targetPosition;

    void Start()
    {
        magnet.transform.localPosition = new Vector3(0, resetYValue, 0);
    }

    public bool CanActivate()
    {
        return !isActivating && !isReturning;
    }

    void FixedUpdate()
    {
        if (isActivating || isReturning)
        {
            magnet.transform.localPosition = Vector3.Lerp(
                magnet.transform.localPosition,
                targetPosition,
                moveSpeed * Time.fixedDeltaTime
            );

            if (Vector3.Distance(magnet.transform.localPosition, targetPosition) < 0.01f)
            {
                magnet.transform.localPosition = targetPosition;

                if (isActivating)
                {
                    isActivating = false;
                    Invoke(nameof(StartReturn), returnDelay);
                }
                else if (isReturning)
                {
                    isReturning = false;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: "+ other.name);

        if (!other.CompareTag("Point")) return;
        //if (isActivating || isReturning) return;

        other.transform.SetParent(magnet.transform);
    }

    public void Activate()
    {
        targetPosition = new Vector3(0, activateYValue, 0);
        isActivating = true;
    }

    private void StartReturn()
    {
        targetPosition = new Vector3(0, resetYValue, 0);
        isReturning = true;
    }
}
