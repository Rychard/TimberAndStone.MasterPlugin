using System;

namespace Plugin.Rychard.MasterPlugin
{
    internal sealed class BasicFileSystemWatcherRemovedEventArgs : EventArgs
    {
        internal String RemovedPath { get; private set; }

        internal BasicFileSystemWatcherRemovedEventArgs(String removedPath)
        {
            RemovedPath = removedPath;
        }
    }
}