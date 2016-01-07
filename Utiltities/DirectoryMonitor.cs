using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Utilities {

	public delegate void FileSystemEvent(string path);

	public interface IDirectoryMonitor {

		event FileSystemEvent Change;
		void Start();

	}

	/// <summary>
	/// From http://spin.atomicobject.com/2010/07/08/consolidate-multiple-filesystemwatcher-events/
	/// </summary>
	public class DirectoryMonitor : IDirectoryMonitor {

		private readonly FileSystemWatcher _fileSystemWatcher = new FileSystemWatcher();
		private readonly Dictionary<string, DateTime> _pendingEvents = new Dictionary<string, DateTime>();
		private readonly Timer _timer;
		private bool _timerStarted;

		public DirectoryMonitor(string folder, string filter) {
			this._fileSystemWatcher.Path = folder;
			this._fileSystemWatcher.Filter = filter;
			this._fileSystemWatcher.IncludeSubdirectories = false;
			this._fileSystemWatcher.Created += this.OnChange;
			this._fileSystemWatcher.Changed += this.OnChange;
			this._timer = new Timer(this.OnTimeout, null, Timeout.Infinite, Timeout.Infinite);
		}

		public event FileSystemEvent Change;

		public void Start() {
			this._fileSystemWatcher.EnableRaisingEvents = true;
		}

		public void Stop() {
			this._fileSystemWatcher.Dispose();
		}

		private void OnChange(object sender, FileSystemEventArgs e) {
			// Don't want other threads messing with the pending events right now 
			lock (this._pendingEvents) {
				// Save a timestamp for the most recent event for this path 
				this._pendingEvents[e.FullPath] = DateTime.Now;
				// Start a timer if not already started 
				if (!this._timerStarted) {
					this._timer.Change(100, 100);
					this._timerStarted = true;
				}
			}
		}

		private void OnTimeout(object state) {
			List<string> paths;
			// Don't want other threads messing with the pending events right now 
			lock (this._pendingEvents) {
				// Get a list of all paths that should have events thrown 
				paths = this.FindReadyPaths(this._pendingEvents);
				// Remove paths that are going to be used now 
				paths.ForEach(delegate(string path) { _pendingEvents.Remove(path); });
				// Stop the timer if there are no more events pending 
				if (this._pendingEvents.Count == 0) {
					this._timer.Change(Timeout.Infinite, Timeout.Infinite);
					this._timerStarted = false;
				}
			}
			// Fire an event for each path that has changed 
			paths.ForEach(this.FireEvent);
		}

		private List<string> FindReadyPaths(Dictionary<string, DateTime> events) {
			List<string> results = new List<string>();
			DateTime now = DateTime.Now;
			foreach (KeyValuePair<string, DateTime> entry in events) {
				// If the path has not received a new event in the last 75ms 
				// an event for the path should be fired 
				double diff = now.Subtract(entry.Value).TotalMilliseconds;
				if (diff >= 75) results.Add(entry.Key);
			}
			return results;
		}

		private void FireEvent(string path) {
			FileSystemEvent evt = this.Change;
			if (evt != null) evt(path);
		}

	}

}