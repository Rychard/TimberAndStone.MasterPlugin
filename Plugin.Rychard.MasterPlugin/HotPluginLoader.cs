using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Timber_and_Stone.API;

namespace Plugin.Rychard.MasterPlugin
{
    internal sealed class HotPluginLoader
    {
        private readonly Type _pluginType = typeof(IPlugin);
        private readonly String _pluginsPath;
        private readonly Action<String> _logWriter;
        private readonly BasicFileSystemWatcher _fileSystemWatcher;
        private readonly Dictionary<String, List<HotPlugin>> _plugins;

        public IEnumerable<IPlugin> Plugins
        {
            get { return _plugins.SelectMany(obj => obj.Value).Select(obj => obj.Plugin); }
        }

        public IEnumerable<IPlugin> LoadedPlugins
        {
            get { return _plugins.SelectMany(obj => obj.Value).Where(obj => obj.IsLoaded).Select(obj => obj.Plugin); }
        }

        public IEnumerable<IPlugin> EnabledPlugins
        {
            get { return _plugins.SelectMany(obj => obj.Value).Where(obj => obj.IsEnabled).Select(obj => obj.Plugin); }
        }

        public HotPluginLoader(String pluginsPath, Action<String> logWriter)
        {
            if (!Directory.Exists(pluginsPath))
            {
                logWriter("Plugins directory does not exist!");
                return;
            }

            _pluginsPath = pluginsPath;
            _logWriter = logWriter;
            _fileSystemWatcher = new BasicFileSystemWatcher(_pluginsPath);
            _fileSystemWatcher.Added += FileSystemWatcherOnAdded;
            _fileSystemWatcher.Removed += FileSystemWatcherOnRemoved;
            _plugins = new Dictionary<String, List<HotPlugin>>();

            IEnumerable<String> assemblies = GetAssemblyFiles();
            foreach (var assemblyPath in assemblies)
            {
                LoadPluginsFromAssembly(assemblyPath);
            }
        }

        private void FileSystemWatcherOnRemoved(Object sender, BasicFileSystemWatcherRemovedEventArgs e)
        {
            String key = e.RemovedPath;
            if (_plugins.ContainsKey(key))
            {
                Invoke(_plugins[key], plugin => plugin.Disable());
                _plugins.Remove(key);
            }
        }

        private void FileSystemWatcherOnAdded(Object sender, BasicFileSystemWatcherAddedEventArgs e)
        {
            String key = e.AddedPath;
            if (_plugins.ContainsKey(key))
            {
                Invoke(_plugins[key], plugin => plugin.Disable());
                _plugins.Remove(key);
            }

            LoadPluginsFromAssembly(key);
            if (_plugins.ContainsKey(key))
            {
                Invoke(_plugins[key], plugin => plugin.Load());
            }
        }

        public void OnLoad()
        {
            Invoke(plugin => plugin.Load());
        }

        public void OnEnable()
        {
            Invoke(plugin => plugin.Enable());
        }

        public void OnDisable()
        {
            Invoke(plugin => plugin.Disable());
        }

        private void Invoke(Action<HotPlugin> action)
        {
            // If initialization has not yet been performed, do nothing.
            if (_plugins == null)
            {
                return;
            }

            foreach (var path in _plugins)
            {
                Invoke(path.Value, action);
            }
        }

        private static void Invoke(IEnumerable<HotPlugin> plugins, Action<HotPlugin> action)
        {
            foreach (var plugin in plugins)
            {
                action(plugin);
            }
        }

        private void LoadPluginsFromAssembly(String assemblyPath)
        {
            _logWriter(String.Format("HotPluginLoader.LoadPluginsFromAssembly(\"{0}\")", assemblyPath));
            
            // We're only interested in changes to assemblies in the specified directory.
            if (Path.GetExtension(assemblyPath) != ".dll")
            {
                return;
            }

            if (_plugins.ContainsKey(assemblyPath))
            {
                // Ensure that we disable the existing plugins from this assembly.
                Invoke(_plugins[assemblyPath], plugin => plugin.Disable());

                // Remove the existing plugin references.
                _plugins[assemblyPath].Clear();
            }
            else
            {
                // If the assembly was not previously loaded, we need to add an entry for it in the dictionary.
                _plugins[assemblyPath] = new List<HotPlugin>();
            }

            if (!File.Exists(assemblyPath))
            {
                // If the file has been deleted, we simply don't try to re-load the assembly.
                return;
            }

            var newPlugins = GetPluginsFromAssembly(assemblyPath);
            foreach (var plugin in newPlugins)
            {
                _plugins[assemblyPath].Add(new HotPlugin(plugin));
            }
        }

        public IEnumerable<String> GetAssemblyFiles()
        {
            if (!Directory.Exists(_pluginsPath))
            {
                _logWriter("HotPluginLoader: Plugins directory does not exist!");
                _logWriter("HotPluginLoader: Additional plugins will not be loaded.");
                yield break;
            }

            var files = Directory.GetFiles(_pluginsPath, "*.dll");

            foreach (var file in files)
            {
                _logWriter(String.Format("Found Assembly: \"{0}\"", file));
                if (!File.Exists(file))
                {
                    continue;
                }

                yield return file;
            }
        }

        private IEnumerable<IPlugin> GetPluginsFromAssembly(String file)
        {
            String name = Path.GetFileName(file);
            _logWriter(String.Format("Found Assembly: {0}", name));

            var types = GetTypesFromAssembly(file);

            // If the assembly failed to load, then we'll just move on to the next assembly.
            if (types == null)
            {
                yield break;
            }

            foreach (Type type in types)
            {
                if (!_pluginType.IsAssignableFrom(type) || !type.IsClass)
                {
                    // Skip any types that don't derive from IPlugin.
                    continue;
                }

                var plugin = GetPlugin(type);
                if (plugin != null)
                {
                    yield return plugin;
                }
            }
        }

        /// <summary>
        /// Attempts to instantiate the specified type as an <see cref="IPlugin"/>.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>An <see cref="IPlugin"/> instance if the operation succeeds, otherwise <c>null</c>.</returns>
        private IPlugin GetPlugin(Type type)
        {
            IPlugin plugin = null;
            try
            {
                var instance = Activator.CreateInstance(type);
                plugin = instance as IPlugin;
            }
            catch (Exception)
            {
                _logWriter(String.Format("Failed to instantiate type: {0}", type.Name));
            }
            return plugin;
        }

        /// <summary>
        /// Gets an array containing the types defined in the assembly at the specified path.
        /// </summary>
        private Type[] GetTypesFromAssembly(String file)
        {
            Type[] types = null;
            try
            {
                Assembly assembly = LoadAssembly(file);
                types = assembly.GetTypes();
            }
            catch (Exception)
            {
                _logWriter("Assembly failed to load!");
            }
            return types;
        }

        /// <summary>
        /// Loads the specified assembly, along with any debug information if it exists.
        /// </summary>
        private static Assembly LoadAssembly(String assemblyPath)
        {           
            Byte[] assemblyBytes = File.ReadAllBytes(assemblyPath);

            String pdbPath;
            var hasDebugInformation = HasDebugInformation(assemblyPath, out pdbPath);
            if (hasDebugInformation)
            {
                // If debug information is present, we'll load it alongside the assembly.
                Byte[] pdbBytes = File.ReadAllBytes(pdbPath);
                return Assembly.Load(assemblyBytes, pdbBytes);
            }
            else
            {
                // Otherwise, we'll just load the assembly by itself.
                return Assembly.Load(assemblyBytes);
            }
        }

        /// <summary>
        /// Determines whether the assembly at the specified path has debug information.
        /// When this method returns <c>true</c>, the output parameter contains the path of the file containing debug information.
        /// </summary>
        private static Boolean HasDebugInformation(String pathToAssembly, out String pdbPath)
        {
            var baseName = Path.GetFileNameWithoutExtension(pathToAssembly);
            var pdbFile = String.Format("{0}.pdb", baseName);
            pdbPath = pdbFile;

            Boolean pdbExists = File.Exists(pdbFile);
            return pdbExists;
        }


    }
}
