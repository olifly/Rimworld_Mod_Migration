using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace AltReality.Rimworld.MigrationMod
{
    class IncidentParms_MigrationWave : IncidentParms
    {
        public IntVec3 enterVec;
        public IntVec3 exitVec;
        
        public PawnKindDef animalPawnKindDef;
    }
}
