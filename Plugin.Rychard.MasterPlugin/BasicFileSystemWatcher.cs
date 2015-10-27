using System;
using System.IO;

namespace Plugin.Rychard.MasterPlugin
{
    /// <summary>
    /// Wrapper for the <see cref="FileSystemWatcher"/> that funnels all events (Changed, Created, Deleted, and Renamed) down into simple Added and Removed events.
    /// </summary>
    /// <remarks>
    /// Some events (e.g. Changed) will raise the "Removed" event, immediately followed by the "Added" event.
    /// </remarks>
    internal sealed class BasicFileSystemWatcher : IDisposable
    {
        internal event EventHandler<BasicFileSystemWatcherRemovedEventArgs> Removed;
        internal event EventHandler<BasicFileSystemWatcherAddedEventArgs> Added;

        private readonly FileSystemWatcher _fileSystemWatcher;
        private readonly String _pluginsPath;

        internal BasicFileSystemWatcher(String path)
        {
            _pluginsPath = path;

            // Monitor the directory so we can remove plugins as needed.
            _fileSystemWatcher = new FileSystemWatcher(_pluginsPath);

            // Ensure the watcher raises events.
            _fileSystemWatcher.EnableRaisingEvents = true;

            // Add handlers for all the events it exposes.
            _fileSystemWatcher.Changed += FileSystemWatcherOnChanged;
            _fileSystemWatcher.Created += FileSystemWatcherOnCreated;
            _fileSystemWatcher.Deleted += FileSystemWatcherOnDeleted;
            _fileSystemWatcher.Renamed += FileSystemWatcherOnRenamed;
        }

        private void OnRemoved(String file)
        {
            var handler = Removed;
            if (handler != null)
            {
                handler(this, new BasicFileSystemWatcherRemovedEventArgs(file));
            }
        }

        private void OnAdded(String file)
        {
            var handler = Added;
            if (handler != null)
            {
                handler(this, new BasicFileSystemWatcherAddedEventArgs(file));
            }
        }

        private void FileSystemWatcherOnRenamed(Object sender, RenamedEventArgs e)
        {
            OnRemoved(e.OldFullPath);
            OnAdded(e.FullPath);
        }

        private void FileSystemWatcherOnDeleted(Object sender, FileSystemEventArgs e)
        {
            OnRemoved(e.FullPath);
        }

        private void FileSystemWatcherOnCreated(Object sender, FileSystemEventArgs e)
        {
            OnAdded(e.FullPath);
        }

        private void FileSystemWatcherOnChanged(Object sender, FileSystemEventArgs e)
        {
            OnRemoved(e.FullPath);
            OnAdded(e.FullPath);
        }

        public void Dispose()
        {
            _fileSystemWatcher.Dispose();
        }
    }
}