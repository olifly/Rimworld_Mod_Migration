﻿using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace AltReality.Rimworld.MigrationMod
{
    class IncidentWorker_Migration : IncidentWorker_MakeMapCondition
    {
        protected override bool StorytellerCanUseNowSub()
        {
            //Only trigger the map condition if it's fall or spring and it hasn't already been triggered.
            return !Find.MapConditionManager.ConditionIsActive(this.def.mapCondition) && (GenDate.CurrentSeason == Season.Spring || GenDate.CurrentSeason == Season.Fall);
        }       
    }
}
