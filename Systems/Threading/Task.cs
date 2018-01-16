using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace Spaceworks {

    public class TaskPool {

        public static void EnqueueInvocation(Task t) {
            ThreadPool.QueueUserWorkItem(new WaitCallback(t.Invoke));
        }

        public static void EnqueueInvocationAndResolution(Task t) {
            ThreadPool.QueueUserWorkItem(new WaitCallback(t.InvokeAndResolve));
        }

    }

    public enum TaskStatus {
        Idle, Active, Complete
    }

    public class Task {

        public TaskStatus state { get; private set; }
        public System.Action<Task> action { get; private set; }
        public System.Action<Task> callback { get; private set; }

        public Task() {
            state = TaskStatus.Idle;
        }

        public Task(System.Action<Task> fn) {
            this.action = fn;
            state = TaskStatus.Idle;
        }

        public Task(System.Action<Task> fn, System.Action<Task> callback) {
            this.action = fn;
            this.callback = callback;
            state = TaskStatus.Idle;
        }

        public void InvokeAndResolve(object threadCtx) {
            Invoke(threadCtx);
            Resolve();
        }

        public void Invoke(object threadCtx) {
            this.state = TaskStatus.Active;
            if (action != null)
                action.Invoke(this);
            this.state = TaskStatus.Complete;
        }

        public void Resolve() {
            if (callback != null)
                callback.Invoke(this);
        }
    }

}