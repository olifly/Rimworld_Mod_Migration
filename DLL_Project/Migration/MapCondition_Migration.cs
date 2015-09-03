using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace AltReality.Rimworld.MigrationMod
{
    class MapCondition_Migration : MapCondition
    {
        private PawnKindDef animalPawnDef;
        
        private IntVec3 migrationEnterVec;                        
        private IntVec3 migrationExitVec;

        private int ticksUntilNextWave;
                                                
        public MapCondition_Migration()
        {            
            animalPawnDef = MigrationUtilities.GetMigrationAnimalDef();

            migrationEnterVec = MigrationUtilities.FindMigrationEnterVec();
            migrationExitVec = MigrationUtilities.FindMigrationExitVec(migrationEnterVec);
                        
            ticksUntilNextWave = GenerateTicksUntilNexWave(true);                        
        }

        public override void MapConditionTick()
        {
            base.MapConditionTick();            
            HandleMigrationWave();
            
        }
        
        private void HandleMigrationWave()
        {
            ticksUntilNextWave--;
            if (ticksUntilNextWave <= 0)
            {
                Log.Message("Spawning a migration wave");
                ticksUntilNextWave = GenerateTicksUntilNexWave();
                IncidentParms_MigrationWave incidentParms = new IncidentParms_MigrationWave();
                incidentParms.points = 0f;
                incidentParms.enterVec = migrationEnterVec;
                incidentParms.exitVec = migrationExitVec;
                incidentParms.animalPawnKindDef = animalPawnDef;                
                IncidentDef.Named("AnimalMigrationWave").Worker.TryExecute(incidentParms);
            }
        }
               
        private int GenerateTicksUntilNexWave(bool first = false)
        {            
            if(first)
            {
                return 0;
            }
            //Calculate the time until next migration waves.  Base time is 12-24 gamehours.  Harder difficulity increases the time between waves while easier difficulity decreases it.
            return Rand.RangeInclusive(MigrationUtilities.DaysToTicks((0.50f * Find.Storyteller.difficulty.threatScale).Clamp(0.25f,1.0f)), MigrationUtilities.DaysToTicks((1.0f* Find.Storyteller.difficulty.threatScale).Clamp(0.75f,2.0f)));            
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.LookDef(ref this.animalPawnDef, "animalPawnDef");            
            Scribe_Values.LookValue(ref this.migrationEnterVec, "migrationEnterVec");
            Scribe_Values.LookValue(ref this.migrationExitVec, "migrationExitVec");
            Scribe_Values.LookValue(ref this.ticksUntilNextWave, "ticksUntilNextWave");            
        }
    }      
}
