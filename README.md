# MasterPlugin

An automatic plugin-loader for [Timber And Stone](http://www.timberandstonegame.com/).

As the game only allows a single plugin/modification to be installed at once, this modification acts as a proxy, allowing multiple plugins/modifications to be installed at the same time.

This modification watches a specific directory for changes, and will attempt to load any plugins contained within them.  If those files are removed, the corresponding plugins are disabled in the game.

Due to constraints regarding the way the .NET framework loads assemblies, they can't be unloaded from memory completely unless they are loaded into their own AppDomain (which this mod doesn't bother with doing).  Thus, this works only when the author of a given plugin abides by the rules set forth by the game's developer, and cleans up after themselves in the `OnDisable` method of their `IPlugin`-derived class.

## Requirements

- Visual Studio 2015
- Timber And Stone ([Normal](http://www.timberandstonegame.com/) or [Steam](http://store.steampowered.com/app/408990/) version)

## Building From Source

1. Open the `Assemblies` directory and follow the instructions in the `PUT_ASSEMBLIES_HERE.txt` file.
2. Open the `Plugin.sln` file in Visual Studio 2015
3. Build the project:
  - Keyboard Shortcut: <kbd>Ctrl</kbd> + <kbd>Shift</kbd> + <kbd>B</kbd>
  - Using the Menu: <kbd>Build</kbd> > <kbd>Build Solution</kbd>
  
The compiled assembly will be located here:

    TimberAndStone.MasterPlugin\Plugin.Rychard.MasterPlugin\bin\Debug\plugin.dll

## Installation

1. Copy the `plugin.dll` file into your `saves` directory
  - e.g. `C:\Program Files (x86)\Steam\steamapps\common\Timber and Stone\saves\`
2. Launch the game
  
Installation of additional modifications is very simple:

1. In the `saves` directory, create a new directory named `plugins`
  - e.g. `C:\Program Files (x86)\Steam\steamapps\common\Timber and Stone\saves\plugins`
2. When downloading modifications, rename *their* `plugin.dll` file to whatever you like
3. Copy the renamed `dll` file into the `plugins` folder you created
4. Launch the game