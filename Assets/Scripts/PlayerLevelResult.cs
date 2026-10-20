using UnityEngine;

[RequireComponent(typeof(PlayerController), typeof(Rigidbody))]
public class PlayerLevelResult : MonoBehaviour
{
    [SerializeField] private float fallY = -5f;

    private PlayerController controller;
    private Rigidbody rb;
    private LevelResultUI resultUI;
    private bool finished;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        resultUI = FindFirstObjectByType<LevelResultUI>();
    }

    private void Update()
    {
        // Below every level's playable floor: the player cannot recover this fall.
        if (!finished && transform.position.y < fallY)
            Finish(false);
    }

    public void WinLevel()
    {
        Finish(true);
    }

    private void Finish(bool won)
    {
        if (finished)
            return;

        finished = true;
        controller.enabled = false;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        resultUI.Show(won);
    }
}
