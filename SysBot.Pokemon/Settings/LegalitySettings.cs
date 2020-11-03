﻿using PKHeX.Core;
using System.ComponentModel;

namespace SysBot.Pokemon
{
    public class LegalitySettings
    {
        private const string Generate = nameof(Generate);
        private const string Misc = nameof(Misc);
        public override string ToString() => "Legality Generating Settings";

        // Generate
        [Category(Generate), Description("MGDB directory path for Wonder Cards.")]
        public string MGDBPath { get; set; } = string.Empty;

        [Category(Generate), Description("Folder for PKM files with trainer data to use for regenerated PKM files.")]
        public string GeneratePathTrainerInfo { get; set; } = string.Empty;

        [Category(Generate), Description("Default Original Trainer name for PKM files that don't match any of the provided PKM files.")]
        public string GenerateOT { get; set; } = "SysBot";

        [Category(Generate), Description("Default 16 Bit Trainer ID (TID) for PKM files that don't match any of the provided PKM files.")]
        public int GenerateTID16 { get; set; } = 12345;

        [Category(Generate), Description("Default 16 Bit Secret ID (SID) for PKM files that that don't match any of the provided PKM files.")]
        public int GenerateSID16 { get; set; } = 54321;

        [Category(Generate), Description("Default Language for PKM files that don't match any of the provided PKM files.")]
        public LanguageID GenerateLanguage { get; set; } = LanguageID.English;

        [Category(Generate), Description("Set all possible legal ribbons for any generated Pokémon.")]
        public bool SetAllLegalRibbons { get; set; }

        [Category(Generate), Description("Set a matching ball (based on color) for any generated Pokémon.")]
        public bool SetMatchingBalls { get; set; }

        [Category(Generate), Description("Force the specified ball if legal.")]
        public bool ForceSpecifiedBall { get; set; } = false;

        [Category(Generate), Description("Allow XOROSHIRO when generating Gen 8 Raid Pokémon.")]
        public bool UseXOROSHIRO { get; set; } = true;

        [Category(Generate), Description("Bot will create an Easter Egg Pokémon if provided an illegal set.")]
        public bool EnableEasterEggs { get; set; } = false;

        [Category(Generate), Description("When set, the bot will only send a Pokémon if it is legal! *This option controls the ability to LAN Trade!* (false = LAN, true = Online)")]
        public bool VerifyLegality { get; set; } = true;

        [Category(Generate), Description("Allow users to submit custom OT, TID, SID, and OT Gender in Showdown sets.")]
        public bool AllowTrainerDataOverride { get; set; } = false;

        [Category(Generate), Description("Enable people to gen Pokémon with OT with advertisements.")]
        public bool AllowAdOT { get; set; } = true;

        [Category(Generate), Description("Allow users to submit further customization with Batch Editor commands.")]
        public bool AllowBatchCommands { get; set; } = false;

        // Misc

        [Category(Misc), Description("Zero out HOME tracker regardless of current tracker value. Applies to user requested PKM files as well.")]
        public bool ResetHOMETracker { get; set; } = true;
    }
}