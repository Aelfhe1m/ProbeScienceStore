# ProbeScienceStore
Small KSP module for moving science data from experiments into a science container on a probe.

Intended for use with KOS for automating multiple science results during planet/moon fly-by missions.

Example usage:

    function StoreScience {
      parameter storagePart.
      
      SET m TO storagePart:GETMODULE("ProbeScienceStore").
      m:DOEVENT("Gather available science").
    }
    
    function fly_by_science {
      parameter expirimentList. // list of modules containing science experiments to run.
      parameter storagePart. 
      
      WAIT UNTIL Altitude < 200000. // start collecting at 200km.
      
      FOR experiment IN experimentList {
        experiment:DEPLOY.
        WAIT UNTIL experiment:HASDATA.
      }
      
      StoreScience(storagePart). // store results and reset experiments - duplicates will be discarded.
      
      WAIT 10. // delay between re-running experiments in hope of new biome.
    }
    
    LOCAL probeStore IS SHIP:PARTSTAGGED("probe")[0].
    LOCAL experimentParts IS SHIP:PARTSTAGGED("flybyexperiment").
    LOCAL experimentModules IS LIST().
    
    FOR p IN experimentParts {
      experimentModules.ADD(p:GETMODULE("ModuleScienceExperiment")).
    }
    
    fly_by_science(experimentModules, probeStore).
