using System;
using System.Diagnostics.CodeAnalysis;
using Timber_and_Stone.API;
using Timber_and_Stone.API.Event;
using EventManager = Timber_and_Stone.Event.EventManager;

namespace Plugin.Rychard.Template
{
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Timber and Stone invokes event handlers by convention.")]
    public sealed class TemplatePlugin : CSharpPlugin, IEventListener
    {
        public override void OnLoad()
        {
            GUIManager.getInstance().AddTextLine("TemplatePlugin.OnLoad");
        }

        public override void OnEnable()
        {
            GUIManager.getInstance().AddTextLine("TemplatePlugin.OnEnable");
            EventManager.getInstance().Register(this);
        }

        public override void OnDisable()
        {
            GUIManager.getInstance().AddTextLine("TemplatePlugin.OnDisable");
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventBlockGrow(EventBlockGrow e)
        {
            WriteEvent(e);
        }

        /// <summary>
        /// Called when a block changes, such as when a block is mined by a miner, built by a builder, or tilled by a farmer.
        /// </summary>
        /// <param name="e">The e.</param>
        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventBlockSet(EventBlockSet e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventBuildStructure(EventBuildStructure e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventCraft(EventCraft e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventDesignationClose(EventDesignationClose e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void EventEntityDeath(EventEntityDeath e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventEntityFactionChange(EventEntityFactionChange e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventEntityGroupChange(EventEntityGroupChange e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventGameLoad(EventGameLoad e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventInvasion(EventInvasion e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventMerchantArrived(EventMerchantArrived e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventMigrant(EventMigrant e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventMigrantAccept(EventMigrantAccept e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventMigrantControl(EventMigrantControl e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventMigrantDeny(EventMigrantDeny e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventMine(EventMine e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventStructureFactionChange(EventStructureFactionChange e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventTrade(EventTrade e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventUnitRename(EventUnitRename e)
        {
            WriteEvent(e);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void OnEventWorkTools(EventWorkTools e)
        {
            WriteEvent(e);
        }

        private void WriteEvent(AEvent e)
        {
            String eventTypeName = e.GetType().Name;
            GUIManager.getInstance().AddTextLine(String.Format("Event: {0}", eventTypeName));
        }
    }
}
