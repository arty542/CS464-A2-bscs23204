using UnityEngine;

// Lecture 04 follow, with Lecture 05 smoothing and a movement-based recenter extension.
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 4f, -7f);
    [SerializeField, Min(0f)] private float smoothSpeed = 5f;
    [SerializeField] private bool autoRecenter = true;
    [SerializeField, Min(0f)] private float turnSharpness = 3f;

    private Rigidbody targetBody;
    private Quaternion followRotation = Quaternion.identity;
    private Quaternion wantedRotation = Quaternion.identity;

    private void Start()
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
            transform.position = target.position + offset;
            transform.LookAt(target);
        }
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        if (autoRecenter && targetBody != null)
        {
            Vector3 velocity = targetBody.linearVelocity;
            Vector3 flatVelocity = new Vector3(velocity.x, 0f, velocity.z);

            // Keep the previous heading while stopped; jumping cannot tilt the orbit.
            if (flatVelocity.sqrMagnitude > 0.1f * 0.1f)
                wantedRotation = Quaternion.LookRotation(flatVelocity, Vector3.up);

            float turnBlend = 1f - Mathf.Exp(-turnSharpness * Time.deltaTime);
            followRotation = Quaternion.Slerp(followRotation, wantedRotation, turnBlend);
        }

        Vector3 followOffset = autoRecenter ? followRotation * offset : offset;
        Vector3 wantedPosition = target.position + followOffset;
        // Lecture 05's exact blend keeps the smoothing consistent across frame rates.
        float followBlend = 1f - Mathf.Exp(-smoothSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(transform.position, wantedPosition, followBlend);
        transform.LookAt(target);
    }
}
