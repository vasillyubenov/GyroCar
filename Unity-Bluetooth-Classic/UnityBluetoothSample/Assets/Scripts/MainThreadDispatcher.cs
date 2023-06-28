using System.Collections.Concurrent;
using System;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private static ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();

    public static void RunOnMainThread(Action action)
    {
        actions.Enqueue(action);
    }

    private void Update()
    {
        while (actions.TryDequeue(out Action action))
        {
            action.Invoke();
        }
    }
}