using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class GuardController : MonoBehaviour
{
    public float stopDistance = 3f;
    public float speed = 5f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 8f;
    public float detectionRadius = 10f;

    private NavMeshAgent agent;
    private Animator animator;
    private Transform currentVisitor = null;
    private bool waiting = false;
    private Coroutine searchCoroutine = null;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        animator = GetComponent<Animator>();

        animator.SetBool("isIdle", true);
        searchCoroutine = StartCoroutine(SearchForVisitors());
    }

    void Update()
    {
        if (currentVisitor != null)
        {
            float distanceToVisitor = Vector3.Distance(transform.position, currentVisitor.position);
            if (distanceToVisitor > detectionRadius)
            {
                currentVisitor = null;
                agent.isStopped = true;
                animator.SetBool("isIdle", true);
                return;
            }

            if (distanceToVisitor > stopDistance)
            {
                if (agent.isStopped)
                    agent.isStopped = false;
                animator.SetBool("isIdle", false);
                agent.SetDestination(currentVisitor.position);
            }
            else
            {
                if (!agent.isStopped)
                {
                    agent.isStopped = true;
                    animator.SetBool("isIdle", true);
                    if (!waiting)
                        StartCoroutine(WaitAndChooseNext());
                }
            }
        }
        else
        {
            if (!agent.isStopped)
                agent.isStopped = true;
            animator.SetBool("isIdle", true);
        }
    }

    void ChooseRandomVisitor()
    {
        GameObject[] visitors = GameObject.FindGameObjectsWithTag("Visitor");
        List<GameObject> nearbyVisitors = new List<GameObject>();

        foreach (GameObject visitor in visitors)
        {
            if (Vector3.Distance(transform.position, visitor.transform.position) <= detectionRadius)
            {
                nearbyVisitors.Add(visitor);
            }
        }

        if (nearbyVisitors.Count > 0)
        {
            GameObject randomVisitor = nearbyVisitors[Random.Range(0, nearbyVisitors.Count)];
            currentVisitor = randomVisitor.transform;

            agent.isStopped = false;
            animator.SetBool("isIdle", false);
            agent.SetDestination(currentVisitor.position);
        }
        else
        {
            currentVisitor = null;
            agent.isStopped = true;
            animator.SetBool("isIdle", true);
        }
    }

    private IEnumerator WaitAndChooseNext()
    {
        waiting = true;
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
        waiting = false;
        ChooseRandomVisitor();
    }

    private IEnumerator SearchForVisitors()
    {
        while (true)
        {
            if (currentVisitor == null)
            {
                ChooseRandomVisitor();
            }
            yield return new WaitForSeconds(2f);
        }
    }
}

