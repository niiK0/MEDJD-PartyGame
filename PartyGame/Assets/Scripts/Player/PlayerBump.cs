using UnityEngine;
using System.Collections.Generic;

public class PlayerBump : MonoBehaviour
{
    public float bumpCooldown = 2.0f;
    private bool canBump = true;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!canBump || !hit.gameObject.CompareTag("Player")) return;

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
