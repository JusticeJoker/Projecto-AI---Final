using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public Transform target;
    float speed = 30f;
    Vector3[] path;
    int targetIndex;

    void Start()
    {
        PathFindManager.RequestPath(transform.position, target.position, PathFound);
    }

    public void PathFound(Vector3[] newPath, bool pathSuccess)
    {
        if (pathSuccess)
        {
            path = newPath;
            StopCoroutine("FollowWay");
            StartCoroutine("FollowWay");
        }
    }

    IEnumerator FollowWay()
    {
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    yield break;
                }

                currentWaypoint = path[targetIndex];
            }

            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    // Draw Path for each NPC 
    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                //Gizmos.color = Color.blue;
                Gizmos.DrawCube(path[i], Vector3.one);
                Gizmos.color = Color.magenta;

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                    Gizmos.color = Color.cyan;
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                    Gizmos.color = Color.cyan;
                }
            }
        }
    }
}
