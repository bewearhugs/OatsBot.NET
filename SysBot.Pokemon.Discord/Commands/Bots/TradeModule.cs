using Discord;
using Discord.Commands;
using Discord.WebSocket;
using PKHeX.Core;
using SysBot.Base;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace SysBot.Pokemon.Discord
{
    [Summary("Queues new Link Code trades")]
    public class TradeModule : ModuleBase<SocketCommandContext>
    {
        private static TradeQueueInfo<PK8> Info => SysCordInstance.Self.Hub.Queues.Info;

        public PokeBotConfig Config = new PokeBotConfig();

        [Command("tradeList")]
        [Alias("tl")]
        [Summary("Prints the users in the trade queues.")]
        [RequireSudo]
        public async Task GetTradeListAsync()
        {
            string msg = Info.GetTradeList(PokeRoutineType.LinkTrade);
            var embed = new EmbedBuilder();
            embed.AddField(x =>
            {
                x.Name = "Pending Trades";
                x.Value = msg;
                x.IsInline = false;
            });
            await ReplyAsync("These are the users who are currently waiting:", embed: embed.Build()).ConfigureAwait(false);
        }

        [Command("eggList")]
        [Alias("el")]
        [Summary("Prints the users in the EggRoll queue.")]
        [RequireSudo]
        public async Task GetEggRollListAsync()
        {
            string msg = Info.GetTradeList(PokeRoutineType.EggRoll);
            var embed = new EmbedBuilder();
            embed.AddField(x =>
            {
                x.Name = "Pending EggRoll Trades";
                x.Value = msg;
                x.IsInline = false;
            });
            await ReplyAsync("These are the users who are currently waiting:", embed: embed.Build()).ConfigureAwait(false);
        }

        [Command("trade")]
        [Alias("t")]
        [Summary("Makes the bot trade you the provided Pokémon file.")]
        [RequireQueueRole(nameof(DiscordManager.RolesTrade))]
        public async Task TradeAsyncAttach([Summary("Trade Code")] int code)
        {
            var sig = Context.User.GetFavor();
            await TradeAsyncAttach(code, sig, Context.User).ConfigureAwait(false);
        }

        [Command("trade")]
        [Alias("t")]
        [Summary("Makes the bot trade you a Pokémon converted from the provided Showdown Set.")]
        [RequireQueueRole(nameof(DiscordManager.RolesTrade))]
        public async Task TradeAsync([Summary("Trade Code")] int code, [Summary("Showdown Set")][Remainder] string content)
        {
            const int gen = 8;
            content = ReusableActions.StripCodeBlock(content);

            var set = new ShowdownSet(content);
            var template = AutoLegalityWrapper.GetTemplate(set);

            if (set.InvalidLines.Count != 0)
            {
                var msg = $"Unable to parse Showdown Set:\n{string.Join("\n", set.InvalidLines)}";
                await ReplyAsync(msg).ConfigureAwait(false);
                return;
            }

            var sav = AutoLegalityWrapper.GetTrainerInfo(gen);
            var pkm = sav.GetLegal(template, out var result);

            if (Info.Hub.Config.Trade.DittoTrade && pkm.Species == 132)
                TradeExtensions.DittoTrade(pkm);

            if (Info.Hub.Config.Trade.EggTrade && pkm.Nickname == "Egg")
                TradeExtensions.EggTrade((PK8)pkm);

            var la = new LegalityAnalysis(pkm);
            var spec = GameInfo.Strings.Species[template.Species];
            var invalid = !(pkm is PK8) || (!la.Valid && SysCordInstance.Self.Hub.Config.Legality.VerifyLegality);
            if (invalid && !Info.Hub.Config.Trade.Memes)
            {
                var reason = result == "Timeout" ? "That set took too long to generate." : "I wasn't able to create something from that.";
                var imsg = $"Oops! {reason} Here's my best attempt for that {spec}!";
                await Context.Channel.SendPKMAsync(pkm, imsg).ConfigureAwait(false);
                return;
            }
            else if (Info.Hub.Config.Trade.Memes)
            {
                if (await TrollAsync(invalid, template).ConfigureAwait(false))
                    return;
            }

            pkm.ResetPartyStats();
            var sig = Context.User.GetFavor();
            await AddTradeToQueueAsync(code, Context.User.Username, (PK8)pkm, sig, Context.User).ConfigureAwait(false);
        }

        [Command("trade")]
        [Alias("t")]
        [Summary("Makes the bot trade you a Pokémon converted from the provided Showdown Set.")]
        [RequireQueueRole(nameof(DiscordManager.RolesTrade))]
        public async Task TradeAsync([Summary("Showdown Set")][Remainder] string content)
        {
            var code = Info.GetRandomTradeCode();
            await TradeAsync(code, content).ConfigureAwait(false);
        }

        [Command("trade")]
        [Alias("t")]
        [Summary("Makes the bot trade you the attached file.")]
        [RequireQueueRole(nameof(DiscordManager.RolesTrade))]
        public async Task TradeAsyncAttach()
        {
            var code = Info.GetRandomTradeCode();
            await TradeAsyncAttach(code).ConfigureAwait(false);
        }

        [Command("eggroll")]
        [Alias("roll", "r")]
        [Summary("Makes the bot trade you a randomly generated egg.")]
        [RequireQueueRole(nameof(DiscordManager.RolesEggRoll))]
        public async Task EggRollAsync()
        {
            if (!Info.Hub.Config.Trade.EggRollChannels.Contains(Context.Channel.Id.ToString()) && !Info.Hub.Config.Trade.EggRollChannels.Equals(""))
            {
                await ReplyAsync($"You're typing the command in the wrong channel!").ConfigureAwait(false);
                return;
            }

            var code = Info.GetRandomTradeCode();

            var rng = new System.Random();
            int shinyRng = rng.Next(0, TradeExtensions.shinyOdds.Length);
            int abilityRng = rng.Next(0, TradeExtensions.abilityIndex.Length);
            var set = new ShowdownSet($"Egg({SpeciesName.GetSpeciesName((int)TradeExtensions.validEgg.GetValue(rng.Next(0, TradeExtensions.validEgg.Length)), 2)})");
            var template = AutoLegalityWrapper.GetTemplate(set);
            var sav = AutoLegalityWrapper.GetTrainerInfo(8);
            var pkm = (PK8)sav.GetLegal(template, out _);

            if (TradeExtensions.regional.ToList().Contains(pkm.Species))
            {
                int formRng = rng.Next(0, TradeExtensions.formIndex1.Length);
                int formRng2 = rng.Next(0, TradeExtensions.formIndex2.Length);

                if (pkm.Species != 52)
                    pkm.SetAltForm(TradeExtensions.formIndex1[formRng]);
                else pkm.SetAltForm(TradeExtensions.formIndex2[formRng2]);

                if (pkm.AltForm != 0)
                {
                    switch (pkm.Species)
                    {
                        case 27: pkm.RelearnMove3 = 10; break;
                        case 37: pkm.RelearnMove4 = 39; break;
                        case 52: pkm.RelearnMove2 = 252; pkm.RelearnMove3 = 45; break;
                        case 83: pkm.RelearnMove1 = 64; pkm.RelearnMove4 = 28; break;
                        case 222: pkm.RelearnMove1 = 33; break;
                        case 263: pkm.RelearnMove2 = 43; pkm.RelearnMove3 = 0; pkm.RelearnMove4 = 0; break;
                    }
                }
            }

            TradeExtensions.EggTrade(pkm);
            pkm.Nature = rng.Next(0, 24);
            pkm.StatNature = pkm.Nature;
            pkm.SetAbilityIndex(TradeExtensions.abilityIndex[abilityRng]);
            pkm.IVs = pkm.SetRandomIVs(3);
            BallApplicator.ApplyBallLegalRandom(pkm);

            switch (TradeExtensions.shinyOdds[shinyRng])
            {
                case 3: pkm.SetUnshiny(); break;
                case 5: CommonEdits.SetShiny(pkm, Shiny.AlwaysStar); break;
                case 6: CommonEdits.SetShiny(pkm, Shiny.AlwaysSquare); break;
            }

            var la = new LegalityAnalysis(pkm);
            var spec = GameInfo.Strings.Species[template.Species];
            var invalid = !(pkm is PK8) || (!la.Valid && SysCordInstance.Self.Hub.Config.Legality.VerifyLegality);
            if (invalid)
            {
                var imsg = $"Oops! I scrambled your egg! Don't tell Ramsay! Here's my best attempt for that {spec}!";
                await Context.Channel.SendPKMAsync(pkm, imsg).ConfigureAwait(false);
                return;
            }

            pkm.ResetPartyStats();
            var sig = Context.User.GetFavor();
            await Context.AddToQueueAsync(code, Context.User.Username, sig, pkm, PokeRoutineType.EggRoll, PokeTradeType.EggRoll).ConfigureAwait(false);
        }

        [Command("tradeUser")]
        [Alias("tu", "tradeOther")]
        [Summary("Makes the bot trade the mentioned user the attached file.")]
        [RequireSudo]
        public async Task TradeAsyncAttachUser([Summary("Trade Code")] int code, [Remainder] string _)
        {
            if (Context.Message.MentionedUsers.Count > 1)
            {
                await ReplyAsync("Too many mentions. Queue one user at a time.").ConfigureAwait(false);
                return;
            }

            if (Context.Message.MentionedUsers.Count == 0)
            {
                await ReplyAsync("A user must be mentioned in order to do this.").ConfigureAwait(false);
                return;
            }

            var usr = Context.Message.MentionedUsers.ElementAt(0);

            if (!IsTradeableUser(usr))
            {
                await ReplyAsync("Mentioned user cannot be traded because they are a bot account.").ConfigureAwait(false);
                return;
            }

            var sig = usr.GetFavor();
            await TradeAsyncAttach(code, sig, usr).ConfigureAwait(false);
        }

        [Command("tradeUser")]
        [Alias("tu", "tradeOther")]
        [Summary("Makes the bot trade the mentioned user the attached file.")]
        [RequireSudo]
        public async Task TradeAsyncAttachUser([Remainder] string _)
        {
            var code = Info.GetRandomTradeCode();
            await TradeAsyncAttachUser(code, _).ConfigureAwait(false);
        }

        private async Task TradeAsyncAttach(int code, RequestSignificance sig, SocketUser usr)
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

            await AddTradeToQueueAsync(code, usr.Username, pk8, sig, usr).ConfigureAwait(false);
        }

        private async Task AddTradeToQueueAsync(int code, string trainerName, PK8 pk8, RequestSignificance sig, SocketUser usr)
        {
            if (!pk8.CanBeTraded() || !new TradeExtensions(Info.Hub).IsItemMule(pk8))
            {
                var msg = "Provided Pokémon content is blocked from trading!";
                await ReplyAsync($"{(!Info.Hub.Config.Trade.ItemMuleCustomMessage.Equals(string.Empty) && !Info.Hub.Config.Trade.ItemMuleSpecies.Equals(Species.None) ? Info.Hub.Config.Trade.ItemMuleCustomMessage : msg)}").ConfigureAwait(false);
                return;
            }

            var la = new LegalityAnalysis(pk8);

            if (!la.Valid && Info.Hub.Config.Legality.VerifyLegality)
                await ReplyAsync("PK8 attachment is not legal, and cannot be traded!").ConfigureAwait(false);
            else
                await Context.AddToQueueAsync(code, trainerName, sig, pk8, PokeRoutineType.LinkTrade, PokeTradeType.Specific, usr).ConfigureAwait(false);
        }

        private bool IsTradeableUser(SocketUser user) => user.IsBot || user.IsWebhook ? false : true;

        private async Task<bool> TrollAsync(bool invalid, IBattleTemplate set)
        {
            var rng = new System.Random();
            var path = Info.Hub.Config.Trade.MemeFileNames.Split(',');
            var msg = $"Oops! I wasn't able to create that {GameInfo.Strings.Species[set.Species]}. Here's a meme instead!\n";

            if (path.Length == 0)
                path = new string[] { "https://i.imgur.com/qaCwr09.png" }; //If memes enabled but none provided, use a default one.

            if (invalid || !ItemRestrictions.IsHeldItemAllowed(set.HeldItem, 8) || (Info.Hub.Config.Trade.ItemMuleSpecies != Species.None && set.Shiny) || Info.Hub.Config.Trade.EggTrade && set.Nickname == "Egg" && set.Species >= 888
                || (Info.Hub.Config.Trade.ItemMuleSpecies != Species.None && GameInfo.Strings.Species[set.Species] != Info.Hub.Config.Trade.ItemMuleSpecies.ToString() && !(Info.Hub.Config.Trade.DittoTrade && set.Species == 132 || Info.Hub.Config.Trade.EggTrade && set.Nickname == "Egg" && set.Species < 888)))
            {
                if (Info.Hub.Config.Trade.MemeFileNames.Contains(".com") || path.Length == 0)
                    _ = invalid == true ? await Context.Channel.SendMessageAsync($"{msg}{path[rng.Next(path.Length)]}").ConfigureAwait(false) : await Context.Channel.SendMessageAsync($"{path[rng.Next(path.Length)]}").ConfigureAwait(false);
                else _ = invalid == true ? await Context.Channel.SendMessageAsync($"{msg}{path[rng.Next(path.Length)]}").ConfigureAwait(false) : await Context.Channel.SendMessageAsync($"{path[rng.Next(path.Length)]}").ConfigureAwait(false);
                return true;
            }
            return false;
        }
    }
}
