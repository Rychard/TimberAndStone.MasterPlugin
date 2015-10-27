using System;

namespace Plugin.Rychard.MasterPlugin
{
    internal sealed class BasicFileSystemWatcherAddedEventArgs : EventArgs
    {
        internal String AddedPath { get; private set; }

        internal BasicFileSystemWatcherAddedEventArgs(String addedPath)
        {
            AddedPath = addedPath;
        }
    }
}