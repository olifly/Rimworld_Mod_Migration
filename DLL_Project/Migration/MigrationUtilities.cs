using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace AltReality.Rimworld.MigrationMod
{
    class MigrationUtilities
    {
        public static int DaysToTicks(float days)
        {
            return (int) Math.Ceiling(days * GenDate.TicksPerDay);
        }

        public static IntVec3 RandomStandableEdgeCellNear(IntVec3 c, int squareRadius)
        {            
            IntVec3 retCell;
            if (!CellFinder.TryFindRandomEdgeCellWith(pos => Math.Abs(pos.x - c.x) <= squareRadius && Math.Abs(pos.z - c.z) <= squareRadius && !pos.Impassable(), out retCell))
            {
                Log.Warning("Could not find a suitable cell. Returning input cell");
                retCell = c;
            }
            return retCell;
        }

        public static IntVec3 FindMigrationEnterVec()
        {
            IntVec3 retVec;

            if (!RCellFinder.TryFindRandomPawnEntryCell(out retVec))
            {
                Log.Error("Could not find a suitable entry location for migration enter vector");
                //For now let's just find a random animal spawning cell. If this fails we're screwed!
                retVec = RCellFinder.RandomAnimalSpawnCell_MapGen();
            }
            Log.Message("EnterVector set to: " + retVec);
            return retVec;
        }

        public static IntVec3 mirrorIntVec3(IntVec3 vec)
        {
            IntVec3 mapCenter = Find.Map.Center;
            IntVec3 mapSize = Find.Map.info.Size;

            return new IntVec3((-(vec.x - mapCenter.x) + mapCenter.x).Clamp(0,mapSize.x-1), 0, (-(vec.z - mapCenter.z) + mapCenter.z).Clamp(0, mapSize.z - 1));
            
        }

        public static IntVec3 FindMigrationExitVec(IntVec3 enterVec)
        {


            //mirror EnterVec
            IntVec3 mirrorVec = mirrorIntVec3(enterVec);
            Log.Message("MirrorVector is: " + mirrorVec);

            //use the mirrorVec if it is suitable
            if (!mirrorVec.Roofed() && mirrorVec.OnEdge() && mirrorVec.Standable())
            {
                Log.Message("ExitVector set to: " + mirrorVec);
                return mirrorVec;
            } else
            {
                IntVec3 retCell;
                int squareRadius = 10;

                while (!CellFinder.TryFindRandomEdgeCellWith(pos => Math.Abs(pos.x - mirrorVec.x) <= squareRadius && Math.Abs(pos.y - mirrorVec.y) <= squareRadius && Math.Abs(pos.z - mirrorVec.z) <= squareRadius && !pos.Impassable(), out retCell))
                {
                    Log.Message("ExitVector not found with SquareRadius " + squareRadius + ". Increasing it by 10");
                    squareRadius += 10;                    
                }
                Log.Message("ExitVector set to: " + retCell);
                return retCell;

            }
        }

        public static float GetChallengeModifier()
        {
            return (2.0f - Find.Storyteller.difficulty.threatScale).Clamp(0.1f, 2.0f);
        }

        public static PawnKindDef GetMigrationAnimalDef()
        {
            //Pick the animal type to migrate. For now we will pick the animal with the highest healthrange.  That should get us (hopefully) the biggest animal in the biome.
            //return Find.Map.Biome.AllWildAnimals.MaxBy(def => def.pointsCost);
            return Find.Map.Biome.AllWildAnimals.MaxBy(def => def.gearHealthRange.max);
        }

    }

    static class ExtensionMethods
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
    }
}
