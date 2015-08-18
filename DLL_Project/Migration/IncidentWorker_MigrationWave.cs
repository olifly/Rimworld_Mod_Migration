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

            //safecast this
            IncidentParms_MigrationWave ip = (IncidentParms_MigrationWave)parms;


            PawnKindDef animalPawnDef = ip.animalPawnKindDef;

            IntVec3 migrationEnterVec = ip.enterVec;
            IntVec3 migrationExitVec = ip.exitVec;

            float challengeModifier = MigrationUtilities.GetChallengeModifier();

            int minAnimals = (int) Math.Ceiling(10 * challengeModifier);
            int maxAnimals = (int) Math.Ceiling(20 * challengeModifier);


            int numberOfAnimals = Rand.RangeInclusive(minAnimals, maxAnimals);
            for (int i = 0; i < numberOfAnimals; i++)
            {
                IntVec3 animalSpawnLocation = MigrationUtilities.RandomStandableEdgeCellNear(migrationEnterVec, 20);

                Pawn animalPawn = PawnGenerator.GeneratePawn(animalPawnDef, null, 0);
                GenSpawn.Spawn(animalPawn, animalSpawnLocation);
                animalPawn.jobs.StopAll();

                //find a random exit position
                IntVec3 animalExitLocation = MigrationUtilities.RandomStandableEdgeCellNear(migrationExitVec, 20);

                Job animalJob = new Job(JobDefOf.Goto, new TargetInfo(animalExitLocation));
                animalJob.exitMapOnArrival = true;

                animalPawn.jobs.StartJob(animalJob);

                //Make them crazy!
                //newThing.mindState.broken.StartBrokenState(BrokenStateDefOf.Manhunter);
            }
            
            return true;
        }
    }
}
