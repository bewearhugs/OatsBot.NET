using Discord.Commands;
using PKHeX.Core;
using System.Linq;
using System.Threading.Tasks;

namespace SysBot.Pokemon.Discord.Commands.Bots
{
    [Summary("Queues new LanTrade trades")]
    public class LanTradeModule : ModuleBase<SocketCommandContext>
    {
        private static TradeQueueInfo<PK8> Info => SysCordInstance.Self.Hub.Queues.Info;
        const int gen = 8;

        [Command("lantrade")]
        [Alias("lt")]
        [Summary("Makes the bot trade you the possibly illegal attached file.")]
        [RequireQueueRole(nameof(DiscordManager.RolesLanTrade))]
        public async Task LanTradeAttachAsync()
        {
            var attachment = Context.Message.Attachments.FirstOrDefault();
            if (attachment == default)
            {
                await ReplyAsync("No attachment provided!").ConfigureAwait(false);
                return;
            }

            var att = await NetUtil.DownloadPKMAsync(attachment).ConfigureAwait(false);
            if (!att.Success || !(att.Data is PK8 pk8))
            {
                await ReplyAsync("No PK8 attachment provided!").ConfigureAwait(false);
                return;
            }

            if (!pk8.CanBeTraded())
            {
                var msg = "Provided Pokémon content is blocked from trading!";
                await ReplyAsync($"{msg}").ConfigureAwait(false);
                return;
            }

            var code = Info.GetRandomTradeCode(); // Ignored, but used for method arguments
            var sig = Context.User.GetFavor();
            await Context.AddToQueueAsync(code, Context.User.Username, sig, pk8, PokeRoutineType.LanTrade, PokeTradeType.LanTrade).ConfigureAwait(false);
        }

        [Command("lantrade")]
        [Alias("lt")]
        [Summary("Makes the bot trade you the possibly illegal attached file.")]
        [RequireQueueRole(nameof(DiscordManager.RolesLanTrade))]
        public async Task LanTradeAttachAsync([Summary("User Requested IGN")][Remainder] string content = "")
        {
            int numLines = content.Split('\n').Length; // To determine if content is Showdown

            if (content.Length <= 12 && Info.Hub.Config.LanTrade.RequeueWhenSpecificIgnNotFound)
            {
                var attachment = Context.Message.Attachments.FirstOrDefault();
                if (attachment == default)
                {
                    await ReplyAsync("No attachment provided!").ConfigureAwait(false);
                    return;
                }

                var att = await NetUtil.DownloadPKMAsync(attachment).ConfigureAwait(false);
                if (!att.Success || !(att.Data is PK8 pk8))
                {
                    await ReplyAsync("No PK8 attachment provided!").ConfigureAwait(false);
                    return;
                }

                if (!pk8.CanBeTraded())
                {
                    var msg = "Provided Pokémon content is blocked from trading!";
                    await ReplyAsync($"{msg}").ConfigureAwait(false);
                    return;
                }

                var code = Info.GetRandomTradeCode(); // Ignored, but used for method arguments
                var sig = Context.User.GetFavor();
                await Context.AddToQueueAsync(code, Context.User.Username, sig, pk8, PokeRoutineType.LanTrade, PokeTradeType.LanTrade, content).ConfigureAwait(false);
            }
            else if (content.Length > 12 && numLines > 1)
                await ReplyAsync("Showdown Sets are disabled for LAN Trading.").ConfigureAwait(false);
            else if (content.Length > 12 && numLines == 1 && Info.Hub.Config.LanTrade.RequeueWhenSpecificIgnNotFound)
                await ReplyAsync("IGN cannot exceed 12 characters.");
            else // If an IGN is specified but host does not have RequeueWhenSpecificIgnNotFound on, do normal lantrade command
                await LanTradeAttachAsync().ConfigureAwait(false);
        }


        [Command("lanroll")]
        [Alias("lroll", "lr")]
        [Summary("Makes the bot trade you a completely random and possibly illegal egg via LAN.")]
        [RequireQueueRole(nameof(DiscordManager.RolesLanRoll))]
        public async Task LanRollAsync([Summary("User Requested IGN")][Remainder] string ign = "")
        {

            if (ign.Length > 12)
            {
                await ReplyAsync("IGN cannot exceed 12 characters.").ConfigureAwait(false);
                return;
            }

            var code = Info.GetRandomTradeCode();

            int[] existantMon =
                { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 25, 26, 27, 28, 29, 30, 31, 32,
                  33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 50, 51, 52, 53, 54,
                  55, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 72, 73, 77, 78, 79, 80,
                  81, 82, 83, 90, 91, 92, 93, 94, 95, 98, 99, 102, 103, 104, 105, 106, 107,
                  108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122,
                  123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137,
                  138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 163,
                  164, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 182, 183, 184, 185,
                  186, 194, 195, 196, 197, 199, 202, 206, 208, 211, 212, 213, 214, 215, 220,
                  221, 222, 223, 224, 225, 226, 227, 230, 233, 236, 237, 238, 239, 240, 241,
                  242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256,
                  257, 258, 259, 260, 263, 264, 270, 271, 272, 273, 274, 275, 278, 279, 280,
                  281, 282, 290, 291, 292, 293, 294, 295, 298, 302, 303, 304, 305, 306, 309,
                  310, 315, 318, 319, 320, 321, 324, 328, 329, 330, 333, 334, 337, 338, 339,
                  340, 341, 342, 343, 344, 345, 346, 347, 348, 349, 350, 355, 356, 359, 360,
                  361, 362, 363, 364, 365, 369, 371, 372, 373, 374, 375, 376, 377, 378, 379,
                  380, 381, 382, 383, 384, 385, 403, 404, 405, 406, 407, 415, 416, 420, 421,
                  422, 423, 425, 426, 427, 428, 434, 436, 437, 438, 439, 440, 442, 443, 444,
                  445, 446, 447, 448, 449, 450, 451, 452, 453, 454, 458, 459, 460, 461, 462,
                  463, 464, 465, 466, 467, 468, 470, 471, 473, 474, 475, 477, 478, 479, 480,
                  481, 482, 483, 484, 485, 486, 487, 488, 494, 506, 507, 508, 509, 510, 517,
                  518, 519, 520, 521, 524, 525, 526, 527, 528, 529, 530, 531, 532, 533, 534,
                  535, 536, 537, 538, 539, 543, 544, 545, 546, 547, 548, 549, 550, 551, 552,
                  553, 554, 555, 556, 557, 558, 559, 560, 561, 562, 563, 564, 565, 566, 567,
                  568, 569, 570, 571, 572, 573, 574, 575, 576, 577, 578, 579, 582, 583, 584,
                  587, 588, 589, 590, 591, 592, 593, 595, 596, 597, 598, 599, 600, 601, 605,
                  606, 607, 608, 609, 610, 611, 612, 613, 614, 615, 616, 617, 618, 619, 620,
                  621, 622, 623, 624, 625, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635,
                  636, 637, 638, 639, 640, 641, 642, 643, 644, 645, 646, 647, 649, 659, 660,
                  661, 662, 663, 674, 675, 677, 678, 679, 680, 681, 682, 683, 684, 685, 686,
                  687, 688, 689, 690, 691, 692, 693, 694, 695, 696, 697, 698, 699, 700, 701,
                  702, 703, 704, 705, 706, 707, 708, 709, 710, 711, 712, 713, 714, 715, 716,
                  717, 718, 719, 721, 722, 723, 724, 725, 726, 727, 728, 729, 730, 736, 737,
                  738, 742, 743, 744, 745, 746, 747, 748, 749, 750, 751, 752, 753, 754, 755,
                  756, 757, 758, 759, 760, 761, 762, 763, 764, 765, 766, 767, 768, 769, 770,
                  771, 772, 773, 776, 777, 778, 780, 781, 782, 783, 784, 785, 786, 787, 788,
                  789, 790, 791, 792, 793, 794, 795, 796, 797, 798, 799, 800, 801, 802, 803,
                  804, 805, 806, 807, 808, 809, 810, 811, 812, 813, 814, 815, 816, 817, 818,
                  819, 820, 821, 822, 823, 824, 825, 826, 827, 828, 829, 830, 831, 832, 833,
                  834, 835, 836, 837, 838, 839, 840, 841, 842, 843, 844, 845, 846, 847, 848,
                  849, 850, 851, 852, 853, 854, 855, 856, 857, 858, 859, 860, 861, 862, 863,
                  864, 865, 866, 867, 868, 869, 870, 871, 872, 873, 874, 875, 876, 877, 878,
                  879, 880, 881, 882, 883, 884, 885, 886, 887, 888, 889, 890, 891, 892, 893,
                  894, 895, 896, 897, 898 };

            var rng = new System.Random();
            var set = new ShowdownSet($"Egg({SpeciesName.GetSpeciesName(rng.Next(new Zukan8Index(Zukan8Type.None, 1).Index, GameUtil.GetMaxSpeciesID(GameVersion.SWSH)), 2)})");
            while (!existantMon.ToList().Contains(set.Species))
                set = new ShowdownSet($"Egg({SpeciesName.GetSpeciesName(rng.Next(new Zukan8Index(Zukan8Type.None, 1).Index, GameUtil.GetMaxSpeciesID(GameVersion.SWSH)), 2)})");

            var template = AutoLegalityWrapper.GetTemplate(set);
            var sav = AutoLegalityWrapper.GetTrainerInfo(gen);
            var pkm = (PK8)sav.GetLegal(template, out _); ;

            LanRollTrade(pkm);

            pkm.ClearRecordFlags();
            pkm.GetSuggestedRelearnMoves();

            pkm.ResetPartyStats();
            var sig = Context.User.GetFavor();
            await Context.AddToQueueAsync(code, Context.User.Username, sig, pkm, PokeRoutineType.LanRoll, PokeTradeType.LanRoll, ign).ConfigureAwait(false);
        }

        private static void LanRollTrade(PK8 pkm)
        {
            int[] regional = { 26, 27, 28, 37, 38, 50, 51, 52, 53, 77, 78, 79, 80, 83, 103,
                  105, 110, 122, 144, 145, 146, 199, 222, 263, 264, 554, 555, 562, 618 };

            int[] shinyOdds = { 3, 3, 3, 3, 3, 5, 5, 5, 5, 5,
                                5, 5, 5, 5, 5, 6, 6, 6, 6, 6 };

            int[] formIndex1 = { 0, 1 };
            int[] formIndex2 = { 0, 1, 2 };

            var rng = new System.Random();
            int shinyRng = rng.Next(0, shinyOdds.Length);

            if (regional.ToList().Contains(pkm.Species)) // Randomize Regional Form
            {
                int formRng = rng.Next(0, formIndex1.Length);
                int formRng2 = rng.Next(0, formIndex2.Length);

                if (pkm.Species != 52) // Checks for Meowth because he's got 2 regional forms
                    pkm.SetAltForm(formIndex1[formRng]);
                else pkm.SetAltForm(formIndex2[formRng2]);
            }

            pkm.Nature = rng.Next(0, 24);
            pkm.StatNature = pkm.Nature;
            pkm.IVs = pkm.SetRandomIVs(4);

            int randomBall = rng.Next(0, pkm.MaxBallID);
            pkm.Ball = randomBall;

            // Source: https://bulbapedia.bulbagarden.net/wiki/Ability#List_of_Abilities
            // https://game8.co/games/pokemon-sword-shield/archives/271828 to see if it exists in the game.
            int[] vaildAbilities =
                { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 17, 18, 19, 20, 21, 22,
                  23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 41, 42,
                  43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 60, 61, 62,
                  63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 75, 77, 78, 79, 80, 81, 82, 83,
                  84, 85, 86, 87, 88, 89, 91, 92, 93, 94, 95, 97, 98, 99, 100, 101, 102, 103,
                  104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118,
                  119, 120, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134,
                  135, 136, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150,
                  151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165,
                  166, 167, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 180, 181, 182,
                  183, 186, 187, 188, 192, 193, 194, 195, 196, 198, 199, 200, 201, 202, 203,
                  204, 205, 207, 208, 209, 211, 212, 214, 215, 217, 218, 220, 221, 222, 224,
                  225, 226, 227, 228, 229, 230, 231, 232, 234, 235, 236, 237, 238, 239, 240,
                  241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255,
                  256, 257, 258, 259, 260, 261, 262, 263, 264, 265 }; // 266/267 (As One) is not on here because it's meh...

            int abilityNum = rng.Next(0, vaildAbilities.Length);

            pkm.Ability = vaildAbilities[abilityNum];

            // Source: https://bulbapedia.bulbagarden.net/wiki/List_of_moves
            int[] invalidMoves =
                { 2, 3, 4, 13, 26, 27, 41, 49, 82, 96, 99, 112, 117, 119, 121, 125, 128, 131,
                  132, 134, 140, 145, 146, 148, 149, 159, 169, 171, 185, 193, 216, 218, 222,
                  228, 237, 265, 274, 287, 289, 290, 293, 300, 301, 302, 316, 318, 320, 324,
                  327, 346, 357, 358, 363, 373, 376, 377, 378, 381, 382, 386, 426, 429, 431,
                  443, 445, 456, 466, 477, 481, 485, 498, 507, 516, 531, 537, 563, 569, 622,
                  623, 624, 625, 626, 627, 628, 629, 630, 631, 632, 633, 634, 635, 636, 637,
                  638, 639, 640, 641, 642, 643, 644, 645, 646, 647, 648, 649, 650, 651, 652,
                  653, 654, 655, 656, 657, 658, 695, 696, 697, 698, 699, 700, 701, 702, 703,
                  717, 718, 719, 720, 721, 722, 723, 724, 725, 726, 727, 728, 729, 730, 731,
                  732, 733, 734, 735, 736, 737, 738, 739, 740, 741, 742, 743, 757, 758, 759,
                  760, 761, 762, 763, 764, 765, 766, 767, 768, 769, 770, 771, 772, 773, 774 };

            int moveRng1 = rng.Next(0, pkm.MaxMoveID);
            int moveRng2 = rng.Next(0, pkm.MaxMoveID);
            int moveRng3 = rng.Next(0, pkm.MaxMoveID);
            int moveRng4 = rng.Next(0, pkm.MaxMoveID);

            while (invalidMoves.ToList().Contains(moveRng1)) // Keeps selecting moves until it picks one that exists in Sword and Shield
                moveRng1 = rng.Next(0, pkm.MaxMoveID);

            while (invalidMoves.ToList().Contains(moveRng2) || moveRng1 == moveRng2) // the OR operand is for duplicate moves
                moveRng2 = rng.Next(0, pkm.MaxMoveID);

            while (invalidMoves.ToList().Contains(moveRng3) || moveRng1 == moveRng3 || moveRng2 == moveRng3)
                moveRng3 = rng.Next(0, pkm.MaxMoveID);

            while (invalidMoves.ToList().Contains(moveRng4) || moveRng1 == moveRng4 || moveRng2 == moveRng4 || moveRng3 == moveRng4)
                moveRng4 = rng.Next(0, pkm.MaxMoveID);

            pkm.Move1 = moveRng1;
            pkm.Move2 = moveRng2;
            pkm.Move3 = moveRng3;
            pkm.Move4 = moveRng4;
            MoveApplicator.SetMaximumPPCurrent(pkm);

            pkm.HeldItem = rng.Next(1, pkm.MaxItemID); // random held item
            while (!ItemRestrictions.IsHeldItemAllowed(pkm)) // checks for non-existing items
                pkm.HeldItem = rng.Next(1, pkm.MaxItemID);

            pkm.CurrentLevel = rng.Next(1, 101);
            pkm.IsEgg = true;
            pkm.Egg_Location = 60002;
            pkm.EggMetDate = System.DateTime.Now.Date;
            pkm.DynamaxLevel = 0;
            pkm.Met_Level = 1;
            pkm.Met_Location = 0;
            pkm.MetDate = System.DateTime.Now.Date;
            pkm.CurrentHandler = 0;
            pkm.OT_Friendship = 1;
            pkm.HT_Name = "";
            pkm.HT_Friendship = 0;
            pkm.HT_Language = 0;
            pkm.HT_Gender = 0;
            pkm.HT_Memory = 0;
            pkm.HT_Feeling = 0;
            pkm.HT_Intensity = 0;
            pkm.EVs = new int[] { 0, 0, 0, 0, 0, 0 };
            pkm.Markings = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            pkm.SetRibbon(rng.Next(53, 98), true); //ribbons 53-97 are marks

            switch (shinyOdds[shinyRng])
            {
                case 3: pkm.SetUnshiny(); break;
                case 5: CommonEdits.SetShiny(pkm, Shiny.AlwaysStar); break;
                case 6: CommonEdits.SetShiny(pkm, Shiny.AlwaysSquare); break;
            }
        }
    }
}
