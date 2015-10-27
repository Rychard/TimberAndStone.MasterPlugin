using System;
using Timber_and_Stone.API;

namespace Plugin.Rychard.MasterPlugin
{
    internal sealed class HotPlugin
    {
        private readonly IPlugin _plugin;
        private Boolean _isLoaded;
        private Boolean _isEnabled;

        internal Boolean IsLoaded
        {
            get { return _isLoaded; }
        }

        internal Boolean IsEnabled
        {
            get { return _isEnabled; }
        }

        internal IPlugin Plugin
        {
            get { return _plugin; }
        }

        internal HotPlugin(IPlugin plugin)
        {
            _plugin = plugin;
            _isLoaded = false;
            _isEnabled = false;
        }

        internal void Load()
        {
            // If it's already loaded, there's nothing to do.
            if (_isLoaded) { return; }

            _plugin.OnLoad();
            _isLoaded = true;
        }

        internal void Enable()
        {
            // If it's already enabled, there's nothing to do.
            if (_isEnabled) { return; }

            _plugin.OnEnable();
            _isEnabled = true;
        }

        internal void Disable()
        {
            // If it's already disabled, there's nothing to do.
            if (!_isEnabled) { return; }

            _plugin.OnDisable();
            _isEnabled = false;
        }
    }
}