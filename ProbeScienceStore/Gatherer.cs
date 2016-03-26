using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ProbeScienceStore
{
    public class ModuleGatherScience : PartModule
    {
        public override string GetInfo()
        {
            return "This part allows science data from other parts of the vessel to be gathered into the probe's science container.";
        }

        #region Actions and Events
        [KSPEvent(guiActive = true, guiActiveEditor = false, guiName = "Gather available science")]
        public void GatherScience()
        {
            try
            {
                var containers = part.FindModulesImplementing<IScienceDataContainer>();
                // Print($"Debug - found {containers.Count} ScienceDataContainers in current part");
                var experimentModules = FlightGlobals.ActiveVessel.FindPartModulesImplementing<ModuleScienceExperiment>();
                // Print($"Debug - found {experimentModules.Count} experiments in vessel.");

                if (containers.Count < 1)
                {
                    Print("ModuleScienceExperiment not found in current part.");
                    return;
                }
                var container = containers[0]; // always store data in first datacontainer found if there are multiples

                if (experimentModules.Count < 1)
                    return; // no experiments to collect

                foreach (var module in experimentModules)
                {
                    var science = ((IScienceDataContainer)module).GetData();
                    if (science != null && science.Length > 0)
                    {
                        // Print($"Found {science.Sum(s => s.dataAmount)} science in {module.GUIName}");
                        if (((ModuleScienceContainer)container).StoreData(new List<IScienceDataContainer> { (IScienceDataContainer)module}, false))
                        {
                            //Print($"{module.GUIName} Dumping and reseting");
                            foreach (var data in science) ((IScienceDataContainer)module).DumpData(data);
                            ((ModuleScienceExperiment)module).ResetExperiment();
                            //Print($"{module.GUIName} OK.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Print($"Exception in GatherScience:  Error:  {ex.Message}\n{ex.StackTrace}");
            }
        }
        #endregion

        public void Print(string format, params object[] args)
        {
            Print(string.Format(format, args));
        }

        public void Print(string message)
        {
            Debug.Log("[ProbeScienceStore] " + message);
        }
    }
}
