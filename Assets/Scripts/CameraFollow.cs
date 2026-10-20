using UnityEngine;

// Lecture 05: exact smoothing and horizontal-velocity look-ahead in LateUpdate.
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 4f, -7f);
    [SerializeField, Min(0f)] private float smoothSpeed = 5f;
    [SerializeField, Min(0f)] private float lookAhead = 0.35f;

    private Rigidbody targetBody;
    private Vector3 smoothedAimPoint;

    private void Awake()
    {
        if (target == null)
        {
            PlayerController player = FindFirstObjectByType<PlayerController>();
            if (player != null)
                target = player.transform;
        }

        if (target != null)
        {
            targetBody = target.GetComponent<Rigidbody>();
            smoothedAimPoint = target.position;
            transform.position = smoothedAimPoint + offset;
            transform.LookAt(smoothedAimPoint);
        }
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 velocity = targetBody != null ? targetBody.linearVelocity : Vector3.zero;
        Vector3 flatVelocity = new Vector3(velocity.x, 0f, velocity.z);

        Vector3 wantedAimPoint = target.position + flatVelocity * lookAhead;
        float followBlend = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
        // Smooth one shared point for position and aim, so direction changes cannot
        // make LookAt snap while the camera position is still catching up.
        smoothedAimPoint = Vector3.Lerp(smoothedAimPoint, wantedAimPoint, followBlend);
        transform.position = smoothedAimPoint + offset;
        transform.LookAt(smoothedAimPoint);
    }
}
