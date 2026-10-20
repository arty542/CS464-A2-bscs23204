using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GoalTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerLevelResult player = other.GetComponentInParent<PlayerLevelResult>();
        if (player != null)
            player.WinLevel();
    }
}
