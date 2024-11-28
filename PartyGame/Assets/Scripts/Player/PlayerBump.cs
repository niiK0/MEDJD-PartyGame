using UnityEngine;
using System.Collections;

public class PlayerBump : MonoBehaviour
{
    public float bumpCooldown = 2.0f;
    public float bumpForce = 10.0f;
    public float bumpDuration = 0.2f;
    private bool canDropPoints = true; // Separate flag for point dropping

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Ignore if not colliding with another player
        if (!hit.gameObject.CompareTag("Player")) return;

        PlayerMovement thisPlayer = GetComponent<PlayerMovement>();
        PlayerMovement otherPlayer = hit.gameObject.GetComponent<PlayerMovement>();

        if (thisPlayer == null || otherPlayer == null) return;

        // Prevent bump if either player is already being pushed
        if (thisPlayer.isGettingPushed || otherPlayer.isGettingPushed) return;

        // Apply pushback to both players
        Vector3 collisionNormal = hit.normal;

        ApplyPushback(thisPlayer.playerController, collisionNormal, bumpForce);
        ApplyPushback(otherPlayer.playerController, -collisionNormal, bumpForce);

        // Handle point dropping only if allowed
        if (canDropPoints)
        {
            HandlePointDrop(thisPlayer.magnet);
            HandlePointDrop(otherPlayer.magnet);

            // Set cooldown for point dropping
            canDropPoints = false;
            Invoke(nameof(ResetPointDropCooldown), bumpCooldown);
        }
    }

    private void ApplyPushback(CharacterController controller, Vector3 direction, float force)
    {
        if (controller == null) return;

        PlayerMovement player = controller.GetComponent<PlayerMovement>();
        if (player == null) return;

        player.isGettingPushed = true;

        // Normalize direction and apply force
        Vector3 pushVelocity = direction.normalized * force;

        StartCoroutine(ApplyPushbackOverTime(controller, pushVelocity));
    }

    private IEnumerator ApplyPushbackOverTime(CharacterController controller, Vector3 velocity)
    {
        float elapsed = 0f;

        while (elapsed < bumpDuration)
        {
            controller.Move(velocity * Time.deltaTime); // Incremental movement
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset push state
        PlayerMovement player = controller.GetComponent<PlayerMovement>();
        if (player != null)
        {
            player.isGettingPushed = false;
        }
    }

    private void HandlePointDrop(Magnet magnet)
    {
        if (magnet == null || magnet.points.Count == 0) return;

        int randomIndex = Random.Range(0, magnet.points.Count);
        Point pointToDrop = magnet.points[randomIndex];

        magnet.points.RemoveAt(randomIndex);
        pointToDrop.GetReleased();

        Debug.Log($"Dropped point {pointToDrop.name} from {magnet.name}");
    }

    private void ResetPointDropCooldown()
    {
        canDropPoints = true;
    }
}
