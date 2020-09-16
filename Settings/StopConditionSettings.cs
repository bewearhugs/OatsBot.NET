﻿using PKHeX.Core;
using System;
using System.ComponentModel;

namespace SysBot.Pokemon
{
    public class StopConditionSettings
    {
        private const string StopConditions = nameof(StopConditions);
        public override string ToString() => "Stop Condition Settings";

        [Category(StopConditions), Description("Stops only on Pokémon of this species. No restrictions if set to \"None\".")]
        public Species StopOnSpecies { get; set; }

        [Category(StopConditions), Description("Stop only on Pokémon of the specified nature.")]
        public Nature TargetNature { get; set; } = Nature.Random;

        [Category(StopConditions), Description("Targets the specified IVs HP/Atk/Def/SpA/SpD/Spe. Matches 0's and 31's, checks min value otherwise. Use \"x\" for unchecked IVs and \"/\" as a separator.")]
        public string TargetIVs { get; set; } = "";

        [Category(StopConditions), Description("Selects the shiny type to stop on.")]
        public TargetShinyType ShinyTarget { get; set; } = TargetShinyType.DisableOption;

        [Category(StopConditions), Description("Stop only on Pokémon that have a mark.")]
        public bool MarkOnly { get; set; } = false;

        [Category(StopConditions), Description("Holds Capture button to record a 30 second clip when a matching Pokémon is found by EncounterBot or Fossilbot.")]
        public bool CaptureVideoClip { get; set; }

        [Category(StopConditions), Description("Extra time in milliseconds to wait after an encounter is matched before pressing Capture for EncounterBot or Fossilbot.")]
        public int ExtraTimeWaitCaptureVideo { get; set; } = 10000;

        [Category(StopConditions), Description("Toggle catching Pokémon. Master Ball will be used to guarantee a catch.")]
        public bool CatchEncounter { get; set; } = false;

        [Category(StopConditions), Description("Toggle whether to inject Master Balls when we run out.")]
        public bool InjectPokeBalls { get; set; } = false;

        [Category(StopConditions), Description("Enter your numerical Discord ID to be pinged in a log channel upon EggFetch, FossilBot or EncounterBot result.")]
        public string PingOnMatch { get; set; } = string.Empty;

        public static bool EncounterFound(PK8 pk, int[] targetIVs, StopConditionSettings settings)
        {
            if (settings.ShinyTarget != TargetShinyType.DisableOption)
            {
                if (settings.ShinyTarget == TargetShinyType.NonShiny && pk.IsShiny)
                    return false;
                if (settings.ShinyTarget != TargetShinyType.NonShiny && !pk.IsShiny)
                    return false;
                if (settings.ShinyTarget == TargetShinyType.StarOnly && pk.ShinyXor == 0)
                    return false;
                if (settings.ShinyTarget == TargetShinyType.SquareOnly && pk.ShinyXor != 0)
                    return false;
            }

            // Match Nature and Species if they were specified.
            if (settings.StopOnSpecies != Species.None && settings.StopOnSpecies != (Species)pk.Species)
                return false;

            if (settings.TargetNature != Nature.Random && settings.TargetNature != (Nature)pk.Nature)
                return false;

            if (settings.MarkOnly && !HasMark(pk))
                return false;

            int[] pkIVList = PKX.ReorderSpeedLast(pk.IVs);

            for (int i = 0; i < 6; i++)
            {
                // Match all 0's.
                if (targetIVs[i] == 0 && pk.IVs[i] != 0)
                    return false;
                // Wild cards should be -1, so they will always be less than the Pokemon's IVs.
                if (targetIVs[i] > pkIVList[i])
                    return false;
            }
            return true;
        }

        public static int[] InitializeTargetIVs(PokeTradeHub<PK8> hub)
        {
            int[] targetIVs = { -1, -1, -1, -1, -1, -1 };

            /* Populate targetIVs array.  Bot matches 0 and 31 IVs.
             * Any other nonzero IV is treated as a minimum accepted value.
             * If they put "x", this is a wild card so we can leave -1. */
            string[] splitIVs = hub.Config.StopConditions.TargetIVs.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            // Only accept up to 6 values in case people can't count.
            for (int i = 0; i < 6 && i < splitIVs.Length; i++)
            {
                if (splitIVs[i] == "x" || splitIVs[i] == "X")
                    continue;
                targetIVs[i] = Convert.ToInt32(splitIVs[i]);
            }
            return targetIVs;
        }

        private static bool HasMark(PK8 pk)
        {
            for (var mark = RibbonIndex.MarkLunchtime; mark <= RibbonIndex.MarkSlump; mark++)
            {
                if (pk.GetRibbon((int)mark))
                    return true;
            }
            return false;
        }
    }

    public enum TargetShinyType
    {
        DisableOption,  // Doesn't care
        NonShiny,       // Match nonshiny only
        AnyShiny,       // Match any shiny regardless of type
        StarOnly,       // Match star shiny only
        SquareOnly,     // Match square shiny only
    }
}
