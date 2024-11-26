using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerBump : MonoBehaviour
{
    public float bumpCooldown = 2.0f;
    public float bumpForce = 10.0f;
    private bool canBump = true;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.gameObject.CompareTag("Player")) return;

        ApplyPushback(hit.controller);

        if (!canBump) return;

        var otherPlayerMagnet = hit.gameObject.GetComponentInChildren<Magnet>();
        var thisPlayerMagnet = GetComponentInChildren<Magnet>();

        if (thisPlayerMagnet == null || otherPlayerMagnet == null) return;

        bool thisPlayerHasPoints = thisPlayerMagnet.points.Count > 0;
        bool otherPlayerHasPoints = otherPlayerMagnet.points.Count > 0;

        if (thisPlayerHasPoints)
        {
            DropRandomPoint(thisPlayerMagnet);
        }

        if (otherPlayerHasPoints)
        {
            DropRandomPoint(otherPlayerMagnet);
        }


        canBump = false;
        Invoke(nameof(ResetBumpCooldown), bumpCooldown);
    }

    private void ApplyPushback(CharacterController controller)
    {
        if (controller == null) return;

        Vector2 pushDirectionInput = controller.GetComponent<PlayerMovement>().moveInput;
        Vector3 pushDirection = new Vector3(pushDirectionInput.x, 0, pushDirectionInput.y);

        Vector3 pushVelocity = -pushDirection.normalized * bumpForce;

        controller.GetComponent<PlayerMovement>().isGettingPushed = true;

        StartCoroutine(ApplyPushbackOverTime(controller, pushVelocity, 0.1f));
    }

    private IEnumerator ApplyPushbackOverTime(CharacterController controller, Vector3 velocity, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            controller.Move(velocity * Time.deltaTime); // Incremental movement
            elapsed += Time.deltaTime;
            yield return null;
        }

        controller.GetComponent<PlayerMovement>().isGettingPushed = false;
    }

    private void DropRandomPoint(Magnet playerMagnet)
    {
        if (playerMagnet.points.Count == 0) return;

        int randomIndex = Random.Range(0, playerMagnet.points.Count);
        Point pointToDrop = playerMagnet.points[randomIndex];

        playerMagnet.points.RemoveAt(randomIndex);
        pointToDrop.GetReleased();

        Debug.Log($"Dropped point {pointToDrop.name} from {playerMagnet.name}");
    }

    private void ResetBumpCooldown()
    {
        canBump = true;
    }
}
