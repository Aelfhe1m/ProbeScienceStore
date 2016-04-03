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
            var info = new StringBuilder();
            try
            {
                var containers = part.FindModulesImplementing<IScienceDataContainer>();
                info.Append($"Debug - found {containers.Count} ScienceDataContainers in current part\n");
                if (containers.Count > 0)
                {
                    for (int i = 0; i < containers.Count; i++)
                    {
                        info.Append($"    [{i}]: {containers[i].GetType()}\n");
                    }
                }
                var experimentModules = FlightGlobals.ActiveVessel.FindPartModulesImplementing<ModuleScienceExperiment>();
                info.Append($"Debug - found {experimentModules.Count} experiments in vessel.\n");
                if (experimentModules.Count > 0)
                {
                    for (int i = 0; i < experimentModules.Count; i++)
                    {
                        info.Append($"    [{i}]: {experimentModules[i].GetType()}\n");
                    }
                }

                if (containers.Count < 1)
                {
                    Print(info.ToString());
                    Print("ModuleScienceExperiment not found in current part.");
                    return;
                }

                ModuleScienceContainer container = null;
                int c = 0;
                while (container == null && c < containers.Count)
                {
                    container = containers[c] as ModuleScienceContainer;
                    c++;
                }
                
                if (experimentModules.Count < 1)
                {
                    Print(info.ToString());
                    Print("No experiment found.");
                    return; // no experiments to collect
                }

                foreach (var module in experimentModules)
                {
                    var science = ((IScienceDataContainer)module).GetData();
                    if (science != null && science.Length > 0)
                    {
                        
                        info.Append($"Found {science.Sum(s => s.dataAmount)} science in {module.name}\n");
                        var isdcModule = (IScienceDataContainer)module;
                        info.Append("Cast to IScienceDataContainer OK.");

                        if (container.StoreData(new List<IScienceDataContainer> { isdcModule }, false))
                        {
                            info.Append($"{module.name} Dumping\n");
                            foreach (var data in science) isdcModule.DumpData(data);

                            info.Append($"{module.name} Reseting\n");
                            ((ModuleScienceExperiment)module).ResetExperiment();

                            info.Append($"{module.name} OK.\n");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Print(info.ToString());
                Debug.LogException(ex);
                // Print($"Exception in GatherScience:  Error:  {ex.Message}\n{ex.StackTrace}");
            }
        }
        #endregion

        public void Print(string format, params object[] args)
        {
            Print(string.Format(format, args));
        }

        public void Print(string message)
        {
            Debug.Log("[ProbeScienceStore] {\n" + message + "}");
        }
    }
}
