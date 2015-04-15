using UnityEngine;
using System.Collections;
using System;

public class AsyncLoader : MonoBehaviour {

    public bool forceDone = false;

    Action startup, whileLoading, whenDone;
    Func<bool> isDone;

    public static AsyncLoader CreateAsyncLoader(Action startup, Func<bool> isDone, Action whileLoading, Action whenDone) {
        Debug.Log("Creating AsyncLoader");
        GameObject ob = new GameObject();
        ob.name = "AsyncLoader"; 
        var async = ob.AddComponent<AsyncLoader>();
        async.startup = startup;
        async.whileLoading = whileLoading;
        async.whenDone = whenDone;
        async.isDone = isDone;
        return async;
    }
    public static AsyncLoader CreateAsyncLoader(Action startup, Action whileLoading, Action whenDone) {
        return CreateAsyncLoader(startup, () => false, whileLoading, whenDone);
    }

    public static AsyncLoader CreateAsyncLoader(Action startup, Action whenDone) {
        return CreateAsyncLoader(startup, () => false, () => { }, whenDone);
    }

    public static AsyncLoader CreateAsyncLoader(Action whenDone) {
        return CreateAsyncLoader(() => { }, () => false, () => { }, whenDone);
    }

    public static AsyncLoader CreateAsyncLoader(Func<bool> isDone, Action whenDone) {
        return CreateAsyncLoader(() => { }, isDone, () => { }, whenDone);
    }


	void Start () {
        startup();
	}
	
	void Update () {
        if (isDone() || forceDone) {
            whenDone();
            Util.Destroy(gameObject);
        } else {
            whileLoading();
        }
	}
}
