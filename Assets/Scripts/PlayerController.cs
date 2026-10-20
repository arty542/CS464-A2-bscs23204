using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Min(0f)] private float speed = 5f;
    [SerializeField, Min(0f)] private float jumpHeight = 1.8f;

    private Rigidbody rb;
    private InputAction moveAction;
    private InputAction jumpAction;
    private Vector3 move;
    private bool jumpQueued;
    private bool isGrounded;
    private readonly HashSet<Collider> groundContacts = new HashSet<Collider>();
    private Vector3 groundPoint;
    private Vector3 groundNormal = Vector3.up;
    private bool hasGroundPoint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        moveAction = InputSystem.actions.FindAction("Player/Move", true);
        jumpAction = InputSystem.actions.FindAction("Player/Jump", true);
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        // These project-wide actions are shared with the next player on scene reload.
        // Disabling this component already stops it from reading input.
        move = Vector3.zero;
        jumpQueued = false;
        groundContacts.Clear();
        isGrounded = false;
    }

    private void Update()
    {
        // Read input every rendered frame, so a short Space press is not lost.
        Vector2 input = moveAction.ReadValue<Vector2>();
        move = Vector3.ClampMagnitude(new Vector3(input.x, 0f, input.y), 1f);
        isGrounded = groundContacts.Count > 0;

        if (isGrounded && !jumpQueued && jumpAction.WasPressedThisFrame())
            jumpQueued = true;
    }

    private void FixedUpdate()
    {
        // Set only horizontal velocity; gravity and jumping own the Y component.
        rb.linearVelocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);

        if (jumpQueued)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            float launchSpeed = Mathf.Sqrt(2f * -Physics.gravity.y * jumpHeight);
            // Impulse = mass * change in velocity, so changing mass preserves height.
            rb.AddForce(Vector3.up * rb.mass * launchSpeed, ForceMode.Impulse);
            jumpQueued = false;
            groundContacts.Clear();
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision) => UpdateGroundContact(collision);
    private void OnCollisionStay(Collision collision) => UpdateGroundContact(collision);

    private void OnCollisionExit(Collision collision)
    {
        groundContacts.Remove(collision.collider);
        isGrounded = groundContacts.Count > 0;
    }

    private void UpdateGroundContact(Collision collision)
    {
        // Lecture 04: an upward-facing contact counts as ground, including ramps.
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                groundContacts.Add(collision.collider);
                groundPoint = contact.point;
                groundNormal = contact.normal;
                hasGroundPoint = true;
                isGrounded = true;
                return;
            }
        }

        groundContacts.Remove(collision.collider);
        isGrounded = groundContacts.Count > 0;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 point = hasGroundPoint ? groundPoint : transform.position + Vector3.down;
        Gizmos.DrawWireSphere(point, 0.1f);
        Gizmos.DrawRay(point, groundNormal * 0.75f);
    }
}
