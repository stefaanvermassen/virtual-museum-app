using UnityEngine;
using System.Collections;
using System;

public class AsyncLoader : MonoBehaviour {

    public bool forceDone = false;

    int interval = 1;
    int randomInterval = 1;
    int timer = 0;
    Action startup, whileLoading, whenDone;
    Func<bool> isDone;
	bool criticalSection = false;

    public static AsyncLoader CreateAsyncLoader(Action startup, Func<bool> isDone, Action whileLoading, Action whenDone, int interval = 1) {
        GameObject ob = new GameObject();
        ob.name = "AsyncLoader"; 
        var async = ob.AddComponent<AsyncLoader>();
        async.startup = startup;
        async.whileLoading = whileLoading;
        async.whenDone = whenDone;
        async.isDone = isDone;
        async.interval = interval;
        return async;
    }
    public static AsyncLoader CreateAsyncLoader(Action startup, Action whileLoading, Action whenDone, int interval = 1) {
        return CreateAsyncLoader(startup, () => false, whileLoading, whenDone, interval);
    }

    public static AsyncLoader CreateAsyncLoader(Action startup, Action whenDone, int interval = 1) {
        return CreateAsyncLoader(startup, () => false, () => { }, whenDone, interval);
    }

    public static AsyncLoader CreateAsyncLoader(Action whenDone, int interval = 1) {
        return CreateAsyncLoader(() => { }, () => false, () => { }, whenDone, interval);
    }

    public static AsyncLoader CreateAsyncLoader(Func<bool> isDone, Action whenDone, int interval = 1) {
        return CreateAsyncLoader(() => { }, isDone, () => { }, whenDone, interval);
    }


	void Start () {
        startup();
	}
	
	void FixedUpdate () {
		if (criticalSection) {
			return;
		}
		criticalSection = true;
        timer++;
        if (timer >= randomInterval) {
            if (isDone() || forceDone) {
                whenDone();
                Util.Destroy(gameObject);
            } else {
                timer = 0;
                randomInterval = (int)(UnityEngine.Random.value * interval);
            }
        } else {
            whileLoading();
        }
		criticalSection = false;
	}
}
