using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathFindManager : MonoBehaviour
{

    Queue<PathRequest> pathRequestsQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathFindManager instance;
    PathFinding pathFinding;

    bool isBuildingPath;

    void Awake()
    {
        instance = this;
        pathFinding = GetComponent<PathFinding>();
    }

    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callBack)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callBack);
        instance.pathRequestsQueue.Enqueue(newRequest); //Add to the Queue
        instance.TryBuildNext();
    }

    // Check if you are already processing a path 
    // and if not it will ask the PathFinder script
    // to process the next path
    void TryBuildNext()
    {
        if(!isBuildingPath && pathRequestsQueue.Count > 0)
        {
            currentPathRequest = pathRequestsQueue.Dequeue(); // Get first path in the queue and takes it out of the queue
            isBuildingPath = true;
            pathFinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd); // Starts looking for a new path
        }
    }

    // Check if new path is built
    public void FinishedBuildPath(Vector3[] path, bool success)
    {
        currentPathRequest.callBack(path, success);
        isBuildingPath = false;
        TryBuildNext();
    }


    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callBack;

        public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> _callBack)
        {
            pathStart = start;
            pathEnd = end;
            callBack = _callBack;
        }
    }
}
