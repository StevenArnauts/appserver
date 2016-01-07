using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Core {

	/// <summary>
	/// From http://codereview.stackexchange.com/questions/43000/a-taskscheduler-that-always-run-tasks-in-a-specific-thread
	/// </summary>
	public class SameThreadTaskScheduler : TaskScheduler, IDisposable {

		private readonly Queue<Task> _scheduledTasks;
		private readonly string _threadName;
		private Thread _myThread;
		private bool _quit;

		public SameThreadTaskScheduler(string name) {
			this._scheduledTasks = new Queue<Task>();
			this._threadName = name;
		}

		public override int MaximumConcurrencyLevel {
			get { return 1; }
		}

		public void Dispose() {
			lock(this._scheduledTasks) {
				this._quit = true;
				Monitor.PulseAll(this._scheduledTasks);
			}
		}

		protected override IEnumerable<Task> GetScheduledTasks() {
			lock(this._scheduledTasks) {
				return this._scheduledTasks.ToList();
			}
		}

		protected override void QueueTask(Task task) {
			if(this._myThread == null) {
				this._myThread = this.StartThread(this._threadName);
			}
			if(!this._myThread.IsAlive) {
				throw new ObjectDisposedException("My thread is not alive, so this object has been disposed!");
			}
			lock(this._scheduledTasks) {
				this._scheduledTasks.Enqueue(task);
				Monitor.PulseAll(this._scheduledTasks);
			}
		}

		protected override bool TryExecuteTaskInline(Task task, bool task_was_previously_queued) {
			return false;
		}

		private Thread StartThread(string name) {
			Thread t = new Thread(this.MyThread) { Name = name };
			using(Barrier start = new Barrier(2)) {
				t.Start(start);
				ReachBarrier(start);
			}
			return t;
		}

		private void MyThread(object o) {
			Task tsk;
			lock(this._scheduledTasks) {
				//When reaches the barrier, we know it holds the lock.
				//
				//So there is no Pulse call can trigger until
				//this thread starts to wait for signals.
				//
				//It is important not to call StartThread within a lock.
				//Otherwise, deadlock!
				ReachBarrier(o as Barrier);
				tsk = this.WaitAndDequeueTask();
			}
			for(; ; ) {
				if(tsk == null) {
					break;
				}
				this.TryExecuteTask(tsk);
				lock(this._scheduledTasks) {
					tsk = this.WaitAndDequeueTask();
				}
			}
		}

		private Task WaitAndDequeueTask() {
			while(!this._scheduledTasks.Any() && !this._quit) {
				Monitor.Wait(this._scheduledTasks);
			}
			return this._quit ? null : this._scheduledTasks.Dequeue();
		}

		private static void ReachBarrier(Barrier b) {
			if(b != null) {
				b.SignalAndWait();
			}
		}

	}

}