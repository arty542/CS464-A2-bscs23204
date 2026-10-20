using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class LandingSquash : MonoBehaviour
{
    [SerializeField] private Transform body;
    [SerializeField, Range(0.1f, 1f)] private float squashY = 0.7f;
    [SerializeField, Min(0.01f)] private float duration = 0.2f;

    private PlayerController controller;
    private Vector3 restScale;
    private Vector3 restPosition;
    private bool wasGrounded;
    private float airStarted = -1f;
    private Coroutine squashRoutine;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        restScale = body.localScale;
        restPosition = body.localPosition;
    }

    private void LateUpdate()
    {
        // All Updates have finished, including the controller's ground check.
        if (!controller.enabled)
        {
            ResetSquash();
            wasGrounded = false;
            airStarted = -1f;
            return;
        }

        bool grounded = controller.IsGrounded;
        if (!grounded && wasGrounded)
            airStarted = Time.time;

        if (grounded && !wasGrounded)
        {
            if (airStarted >= 0f && Time.time - airStarted > 0.1f)
            {
                ResetSquash();
                squashRoutine = StartCoroutine(Squash());
            }
            airStarted = -1f;
        }

        wasGrounded = grounded;
    }

    private IEnumerator Squash()
    {
        for (float elapsed = 0f; elapsed < duration; elapsed += Time.deltaTime)
        {
            float p = Ease.OutBack(elapsed / duration);
            float y = Mathf.LerpUnclamped(squashY, 1f, p);
            float width = 1f / Mathf.Sqrt(y);
            body.localScale = Vector3.Scale(restScale, new Vector3(width, y, width));
            // Body is a height-2 capsule: keep its bottom at the original foot position.
            body.localPosition = restPosition + Vector3.up * (body.localScale.y - restScale.y);
            yield return null;
        }

        body.localScale = restScale;
        body.localPosition = restPosition;
        squashRoutine = null;
    }

    private void OnDisable()
    {
        ResetSquash();
        wasGrounded = false;
        airStarted = -1f;
    }

    private void ResetSquash()
    {
        if (squashRoutine != null)
        {
            StopCoroutine(squashRoutine);
            squashRoutine = null;
        }

        if (body != null)
        {
            body.localScale = restScale;
            body.localPosition = restPosition;
        }
    }
}
