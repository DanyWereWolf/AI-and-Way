using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class RandomMovement : MonoBehaviour
{
    public Transform[] zones;
    public GameObject[] points;
    public float minDistance = 1f;
    public string danceZoneName = "DanceFloor";

    private NavMeshAgent agent;
    private Animator animator;

    private bool isDancing = false;
    private GameObject currentPoint;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ChooseRandomDestination();
    }

    void Update()
    {
        if (isDancing)
        {
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= minDistance)
        {
            if (IsPointInDanceZone(currentPoint))
            {
                StartCoroutine(DoDance());
            }
            else
            {
                ChooseRandomDestination();
            }
        }

        bool isWalking = agent.velocity.magnitude > 0.1f && !isDancing;
        animator.SetBool("isWalking", isWalking);
    }

    void ChooseRandomDestination()
    {
        int zoneIndex = Random.Range(0, zones.Length);
        Transform selectedZone = zones[zoneIndex];
        GameObject[] pointsInZone = System.Array.FindAll(points, point => point.transform.parent == selectedZone);

        if (pointsInZone.Length > 0)
        {
            int pointIndex = Random.Range(0, pointsInZone.Length);
            currentPoint = pointsInZone[pointIndex];
            agent.SetDestination(currentPoint.transform.position);
        }
    }

    bool IsPointInDanceZone(GameObject point)
    {
        if (point == null) return false;
        Transform parentZone = point.transform.parent;
        return parentZone != null && parentZone.name == danceZoneName;
    }

    IEnumerator DoDance()
    {
        isDancing = true;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("Dance");

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        float danceLength = 0f;

        yield return new WaitForSeconds(0.1f);

        state = animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("Dance"))
        {
            danceLength = state.length;
        }
        else
        {
            danceLength = 15.3f;
        }

        yield return new WaitForSeconds(danceLength);

        isDancing = false;
        ChooseRandomDestination();
    }
}
