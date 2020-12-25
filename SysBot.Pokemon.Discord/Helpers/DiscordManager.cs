using System;
using System.Collections.Generic;
using System.Linq;

namespace SysBot.Pokemon.Discord
{
    public class DiscordManager
    {
        public readonly PokeTradeHubConfig Config;
        public ulong Owner;

        public readonly SensitiveSet<ulong> BlacklistedUsers = new();
        public readonly SensitiveSet<ulong> WhitelistedChannels = new();

        public readonly SensitiveSet<ulong> SudoDiscord = new();
        public readonly SensitiveSet<string> SudoRoles = new();
        public readonly SensitiveSet<string> FavoredRoles = new();

        public readonly SensitiveSet<string> RolesClone = new();
        public readonly SensitiveSet<string> RolesFixOT = new();
        public readonly SensitiveSet<string> RolesPowerUp = new();
        public readonly SensitiveSet<string> RolesEggRoll = new();
        public readonly SensitiveSet<string> RolesTrade = new();
        public readonly SensitiveSet<string> RolesLanTrade = new();
        public readonly SensitiveSet<string> RolesLanRoll = new();
        public readonly SensitiveSet<string> RolesSeed = new();
        public readonly SensitiveSet<string> RolesDump = new();
        public readonly SensitiveSet<string> RolesRemoteControl = new();

        public bool CanUseSudo(ulong uid) => SudoDiscord.Contains(uid);
        public bool CanUseSudo(IEnumerable<string> roles) => roles.Any(SudoRoles.Contains);

        public bool CanUseCommandChannel(ulong channel) => WhitelistedChannels.Count == 0 || WhitelistedChannels.Contains(channel);
        public bool CanUseCommandUser(ulong uid) => !BlacklistedUsers.Contains(uid);

        public RequestSignificance GetSignificance(IEnumerable<string> roles)
        {
            var result = RequestSignificance.None;
            foreach (var r in roles)
            {
                if (SudoRoles.Contains(r))
                    return RequestSignificance.Sudo;
                if (FavoredRoles.Contains(r))
                    result = RequestSignificance.Favored;
            }
            return result;
        }

        public bool IsCommandForDMs(string msg) =>
            msg.Contains("qs") || msg.Contains("ts") || msg.Contains("queuestatus") || 
            msg.Contains("convert") || msg.Contains("showdown") ||
            msg.Contains("legalize") || msg.Contains("alm") ||
            msg.Contains("qc") || msg.Contains("tc") || msg.Contains("queueClear");

        public DiscordManager(PokeTradeHubConfig cfg)
        {
            Config = cfg;
            Read();
        }

        public bool GetHasRoleQueue(string type, IEnumerable<string> roles)
        {
            var set = GetSet(type);
            return set.Count == 0 || roles.Any(set.Contains);
        }

        private SensitiveSet<string> GetSet(string type)
        {
            return type switch
            {
                nameof(RolesClone) => RolesClone,
                nameof(RolesFixOT) => RolesFixOT,
                nameof(RolesPowerUp) => RolesPowerUp,
                nameof(RolesEggRoll) => RolesEggRoll,
                nameof(RolesTrade) => RolesTrade,
                nameof(RolesLanTrade) => RolesLanTrade,
                nameof(RolesLanRoll) => RolesLanRoll,
                nameof(RolesSeed) => RolesSeed,
                nameof(RolesDump) => RolesDump,
                nameof(RolesRemoteControl) => RolesRemoteControl,
                _ => throw new ArgumentOutOfRangeException(nameof(type)),
            };
        }

        public void Read()
        {
            var cfg = Config;
            BlacklistedUsers.Read(cfg.Discord.UserBlacklist, ulong.Parse);
            WhitelistedChannels.Read(cfg.Discord.ChannelWhitelist, ulong.Parse);

            SudoDiscord.Read(cfg.Discord.GlobalSudoList, ulong.Parse);
            SudoRoles.Read(cfg.Discord.RoleSettings.RoleSudo, z => z);
            FavoredRoles.Read(cfg.Discord.RoleSettings.RoleFavored, z => z);

            RolesClone.Read(cfg.Discord.RoleSettings.RoleCanClone, z => z);
            RolesFixOT.Read(cfg.Discord.RoleSettings.RoleCanFixOT, z => z);
            RolesPowerUp.Read(cfg.Discord.RoleSettings.RoleCanPowerUp, z => z);
            RolesEggRoll.Read(cfg.Discord.RoleSettings.RoleCanEggRoll, z => z);
            RolesTrade.Read(cfg.Discord.RoleSettings.RoleCanTrade, z => z);
            RolesLanTrade.Read(cfg.Discord.RoleSettings.RoleCanLanTrade, z => z);
            RolesLanRoll.Read(cfg.Discord.RoleSettings.RoleCanLanRoll, z => z);
            RolesSeed.Read(cfg.Discord.RoleSettings.RoleCanSeedCheck, z => z);
            RolesDump.Read(cfg.Discord.RoleSettings.RoleCanDump, z => z);
            RolesRemoteControl.Read(cfg.Discord.RoleSettings.RoleRemoteControl, z => z);
        }

        public void Write()
        {
            Config.Discord.UserBlacklist = BlacklistedUsers.Write();
            Config.Discord.ChannelWhitelist = WhitelistedChannels.Write();
            Config.Discord.RoleSettings.RoleSudo = SudoRoles.Write();
            Config.Discord.GlobalSudoList = SudoDiscord.Write();
            Config.Discord.RoleSettings.RoleFavored = FavoredRoles.Write();
            Config.Discord.RoleSettings.RoleCanClone = RolesClone.Write();
            Config.Discord.RoleSettings.RoleCanFixOT = RolesFixOT.Write();
            Config.Discord.RoleSettings.RoleCanPowerUp = RolesPowerUp.Write();
            Config.Discord.RoleSettings.RoleCanEggRoll = RolesEggRoll.Write();
            Config.Discord.RoleSettings.RoleCanTrade = RolesTrade.Write();
            Config.Discord.RoleSettings.RoleCanLanTrade = RolesLanTrade.Write();
            Config.Discord.RoleSettings.RoleCanLanRoll = RolesLanRoll.Write();
            Config.Discord.RoleSettings.RoleCanSeedCheck = RolesSeed.Write();
            Config.Discord.RoleSettings.RoleCanDump = RolesDump.Write();
        }
    }
}