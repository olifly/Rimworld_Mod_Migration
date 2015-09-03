using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace AltReality.Rimworld.MigrationMod
{
    public class IncidentWorker_MigrationWave : IncidentWorker
    {
        public override bool TryExecute(IncidentParms parms)
        {

            IntVec3 migrationEnterVec;
            IntVec3 migrationExitVec;
            PawnKindDef animalPawnDef;

            IncidentParms_MigrationWave ip = parms as IncidentParms_MigrationWave;

            if(ip != null) {
                animalPawnDef = ip.animalPawnKindDef;
                migrationEnterVec = ip.enterVec;
                migrationExitVec = ip.exitVec;
            } else
            { /*For some reason this was not passed the correct subclass of IncidentParms.  
                The incident was probably triggered by development console or the def for it has been changed.
                Need to generate all the data ourself.*/
                migrationEnterVec = MigrationUtilities.FindMigrationEnterVec();
                migrationExitVec = MigrationUtilities.FindMigrationExitVec(migrationEnterVec);
                animalPawnDef = MigrationUtilities.GetMigrationAnimalDef();
            }
            
            float challengeModifier = MigrationUtilities.GetChallengeModifier();

            int minAnimals = (int) Math.Ceiling(10 * challengeModifier);
            int maxAnimals = (int) Math.Ceiling(20 * challengeModifier);


            int numberOfAnimals = Rand.RangeInclusive(minAnimals, maxAnimals);
            for (int i = 0; i < numberOfAnimals; i++)
            {
                IntVec3 animalSpawnLocation = MigrationUtilities.RandomStandableEdgeCellNear(migrationEnterVec, 20);

                Pawn animalPawn = PawnGenerator.GeneratePawn(animalPawnDef, null,Rand.RangeInclusive(1,10) <= 4 ? true : false);
                GenSpawn.Spawn(animalPawn, animalSpawnLocation);
                animalPawn.jobs.StopAll();

                //find a random exit position
                IntVec3 animalExitLocation = MigrationUtilities.RandomStandableEdgeCellNear(migrationExitVec, 20);                
                Job animalJob = new Job(JobDefOf.Goto, new TargetInfo(animalExitLocation));
                animalJob.exitMapOnArrival = true;

                animalPawn.jobs.StartJob(animalJob);

                //Make 'em crazy!
                //animalPawn.mindState.broken.StartBrokenState(BrokenStateDefOf.Berserk);
            }
            
            return true;
        }
    }
}
