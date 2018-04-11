using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace Spaceworks.Threading {

    /// <summary>
    /// Container manages tasks sent to a thread pool
    /// </summary>
    public class TaskPool {

        /// <summary>
        /// Enqueue a task to the thread pool, and invoke but do not resolve
        /// </summary>
        /// <param name="t"></param>
        public static void EnqueueInvocation(Task t) {
            ThreadPool.QueueUserWorkItem(new WaitCallback(t.Invoke));
        }

        /// <summary>
        /// Enqueue a task to the thread pool, and invoke it. Resolve it when complete
        /// </summary>
        /// <param name="t"></param>
        public static void EnqueueInvocationAndResolution(Task t) {
            ThreadPool.QueueUserWorkItem(new WaitCallback(t.InvokeAndResolve));
        }

    }

    /// <summary>
    /// Flag represents stage of execution a task is in
    /// </summary>
    public enum TaskStatus {
        Idle, Active, Complete
    }

    /// <summary>
    /// Task represents async action and resolution
    /// </summary>
    public class Task {

        public TaskStatus state { get; private set; }
        public System.Action<Task> action { get; private set; }
        public System.Action<Task> callback { get; private set; }

        public Task() {
            state = TaskStatus.Idle;
        }

        /// <summary>
        /// Create a task with no resolution callback
        /// </summary>
        /// <param name="fn"></param>
        public Task(System.Action<Task> fn) {
            this.action = fn;
            state = TaskStatus.Idle;
        }

        /// <summary>
        /// Create a task with a resolution callback
        /// </summary>
        /// <param name="fn"></param>
        /// <param name="callback"></param>
        public Task(System.Action<Task> fn, System.Action<Task> callback) {
            this.action = fn;
            this.callback = callback;
            state = TaskStatus.Idle;
        }

        /// <summary>
        /// Run and invoke callback on the current thread context
        /// </summary>
        /// <param name="threadCtx"></param>
        public void InvokeAndResolve(object threadCtx) {
            Invoke(threadCtx);
            Resolve();
        }

        /// <summary>
        /// Run a task on the current thread context
        /// </summary>
        /// <param name="threadCtx"></param>
        public void Invoke(object threadCtx) {
            this.state = TaskStatus.Active;
            if (action != null)
                action.Invoke(this);
            this.state = TaskStatus.Complete;
        }

        /// <summary>
        /// Invoke callback on current thread
        /// </summary>
        public void Resolve() {
            if (callback != null)
                callback.Invoke(this);
        }
    }

}