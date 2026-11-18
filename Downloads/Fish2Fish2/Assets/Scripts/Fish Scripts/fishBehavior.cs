using UnityEngine;

public class fishBehavior : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRadius = 6f;
    public float safeRadius = 10f;

    [Header("Idle")]
    public float idleMinDuration = 1f;
    public float idleMaxDuration = 3f;

    [Header("Smoothing")]
    public float directionSmoothSpeed = 2f;
    public float rotationSmoothSpeed = 2f;

    private fishMovement movement;
    private fishPatrol patrol;
    private Transform player;
    private float stateTimer;
    private FishState currentState = FishState.Idle;
    private Vector3 smoothedDir = Vector3.forward;

    void Awake()
    {
        movement = GetComponent<fishMovement>();
        patrol = GetComponent<fishPatrol>();
    }

    private void Start()
    {
        EnterState(FishState.Patrol);
    }

    public void enable()
    {
        enabled = true;
        EnterState(FishState.Patrol);
    }

    public void disable()
    {
        enabled = false;
        movement.applyMovement(Vector3.zero);
    }

    void Update()
    {
        if (PlayerFishTracker.Current != null)
            player = PlayerFishTracker.Current;

        if (player != null && player == transform)
            return;

        float dist = player != null
            ? Vector3.Distance(transform.position, player.position)
            : float.MaxValue;

        switch (currentState)
        {
            case FishState.Idle: UpdateIdle(dist); break;
            case FishState.Patrol: UpdatePatrol(dist); break;
            case FishState.Flee: UpdateFlee(dist); break;
        }
    }

    void UpdateIdle(float dist)
    {
        smoothedDir = Vector3.Lerp(smoothedDir, Vector3.zero, Time.deltaTime * directionSmoothSpeed);
        movement.applyMovement(smoothedDir);
        stateTimer -= Time.deltaTime;

        if (dist < detectionRadius) { EnterState(FishState.Flee); return; }
        if (stateTimer <= 0f) { EnterState(FishState.Patrol); return; }
    }

    void UpdatePatrol(float dist)
    {
        if (dist < detectionRadius) { EnterState(FishState.Flee); return; }

        Vector3 target = patrol.GetPatrolTarget(transform.position);
        Vector3 desiredDir = (target - transform.position);
        desiredDir.y *= 0.2f;
        desiredDir = desiredDir.normalized;

        smoothedDir = Vector3.Lerp(smoothedDir, desiredDir, Time.deltaTime * directionSmoothSpeed);
        movement.applyMovement(smoothedDir);

        Vector3 flatDir = new Vector3(smoothedDir.x, 0, smoothedDir.z).normalized;
        if (flatDir.magnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatDir, Vector3.up);
            Quaternion smoothRot = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSmoothSpeed);
            movement.applyRotation(smoothRot);
        }

        stateTimer -= Time.deltaTime;
        if (stateTimer <= 0f) { EnterState(FishState.Idle); return; }
    }

    void UpdateFlee(float dist)
    {
        if (dist > safeRadius) { EnterState(FishState.Patrol); return; }

        Vector3 desiredDir = (transform.position - player.position);
        desiredDir.y *= 0.2f;
        desiredDir = desiredDir.normalized;

        smoothedDir = Vector3.Lerp(smoothedDir, desiredDir, Time.deltaTime * directionSmoothSpeed);
        movement.applyMovement(smoothedDir);

        Vector3 flatDir = new Vector3(smoothedDir.x, 0, smoothedDir.z).normalized;
        if (flatDir.magnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(flatDir, Vector3.up);
            Quaternion smoothRot = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationSmoothSpeed);
            movement.applyRotation(smoothRot);
        }
    }

    void EnterState(FishState newState)
    {
        currentState = newState;
        switch (newState)
        {
            case FishState.Idle:
                stateTimer = Random.Range(idleMinDuration, idleMaxDuration);
                break;
            case FishState.Patrol:
                stateTimer = Random.Range(3f, 7f);
                patrol.PickNewTarget(transform.position);
                break;
        }
    }
}

public enum FishState { Idle, Patrol, Flee }