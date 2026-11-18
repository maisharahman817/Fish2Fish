using UnityEngine;

public class fishSwimAnimation : MonoBehaviour
{
    [Header("Wiggle")]
    public float wiggleSpeed = 3f;
    public float wiggleAmount = 15f;

    [Header("Bob")]
    public float bobSpeed = 1.5f;
    public float bobAmount = 0.05f;

    private Rigidbody rb;
    private Vector3 startLocalPos;
    private float wiggleTimer;
    private float bobTimer;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        startLocalPos = transform.localPosition;
    }

    void Update()
    {
        float speed = rb != null ? rb.linearVelocity.magnitude : 0f;
        float speedRatio = Mathf.Clamp01(speed / 2f);

        // wiggle side to side
        wiggleTimer += Time.deltaTime * wiggleSpeed * (0.5f + speedRatio);
        float wiggle = Mathf.Sin(wiggleTimer) * wiggleAmount * (0.3f + speedRatio);
        transform.localRotation = Quaternion.Euler(0, wiggle, 0);

        // gentle bob up and down
        bobTimer += Time.deltaTime * bobSpeed;
        float bob = Mathf.Sin(bobTimer) * bobAmount;
        transform.localPosition = startLocalPos + new Vector3(0, bob, 0);
    }
}