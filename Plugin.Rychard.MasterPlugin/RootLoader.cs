using System;
using System.IO;
using Timber_and_Stone.API;

namespace Plugin.Rychard.MasterPlugin
{
    // ReSharper disable once UnusedMember.Global
    public sealed class RootLoader : CSharpPlugin
    {
        private readonly String _pluginsPath = Path.Combine("saves", "plugins");
        private GUIManager _guiManager;
        private HotPluginLoader _pluginLoader;

        public override void OnLoad()
        {
            _guiManager = GUIManager.getInstance();
            _pluginLoader = new HotPluginLoader(_pluginsPath, PluginLoaderWriteLine);
            
            WriteLine("Root Loader - Loaded");
            _pluginLoader.OnLoad();
        }

        public override void OnEnable()
        {
            WriteLine("Root Loader - Enabled");
            _pluginLoader.OnEnable();
        }

        public override void OnDisable()
        {
            _pluginLoader.OnDisable();
        }

        private void PluginLoaderWriteLine(String line)
        {
            WriteLine(line);
        }

        /// <summary>
        /// Writes the specified text to the in-game message log.  Lengthy text is split across multiple lines.
        /// </summary>
        private void WriteLine(String line, Int32 indentWrapSize = 4)
        {
            // How many characters can we fit on a line?
            Int32 wrapLength = 70;
            if (line != null)
            {
                String indentText = new String(' ', indentWrapSize);
                Boolean firstLine = true;
                // If the line is longer than our limit, we need to split it across multiple lines.
                while (line.Length > wrapLength)
                {
                    // Calculate the contents of the line, omitting anything that should be placed on subsequent lines.
                    var partial = line.Substring(0, wrapLength);

                    if (!firstLine)
                    {
                        // Wrapped text should be indented for readability.
                        partial = String.Format("{0}{1}", indentText, partial);
                    }

                    // Write the line to the UI.
                    _guiManager.AddTextLine(partial);

                    // Assign the remaining text to our line, so that we can continue wrapping, until everything is written.
                    line = line.Substring(wrapLength);

                    if (firstLine)
                    {
                        // On subsequent lines, reduce the wrap length by the number of characters that will be reserved for indentation.
                        firstLine = false;
                        wrapLength = (wrapLength - indentWrapSize);
                    }
                }
                // After the above loop breaks, the value of "line" is short enough to fit on its own line.
                _guiManager.AddTextLine(line);
            }
        }
    }
}
