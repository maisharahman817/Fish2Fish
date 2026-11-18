using UnityEngine;

public class fishPatrol : MonoBehaviour
{
    [Header("Patrol Volume")]
    public Vector3 patrolCenter = Vector3.zero;
    public Vector3 patrolExtents = new Vector3(5, 0.5f, 5);
    public float arrivalDistance = 1.5f;

    private Vector3 currentTarget;
    private bool hasTarget = false;

    void Start()
    {
        patrolCenter = transform.position;
    }

    public void PickNewTarget(Vector3 fromPosition)
    {
        currentTarget = patrolCenter + new Vector3(
            Random.Range(-patrolExtents.x, patrolExtents.x),
            Random.Range(-patrolExtents.y, patrolExtents.y),
            Random.Range(-patrolExtents.z, patrolExtents.z)
        );

        // never pick a target below starting height
        currentTarget.y = Mathf.Max(currentTarget.y, patrolCenter.y - 1f);
        hasTarget = true;
    }

    public Vector3 GetPatrolTarget(Vector3 currentPos)
    {
        if (!hasTarget) PickNewTarget(currentPos);

        if (Vector3.Distance(currentPos, currentTarget) < arrivalDistance)
            PickNewTarget(currentPos);

        return currentTarget;
    }
}