﻿using System.ComponentModel;

namespace SysBot.Pokemon
{
    public class DiscordSettings
    {
        private const string Startup = nameof(Startup);
        private const string Operation = nameof(Operation);
        private const string Whitelists = nameof(Whitelists);
        public override string ToString() => "Discord Integration Settings";

        // Startup

        [Category(Startup), Description("Bot login token.")]
        public string Token { get; set; } = string.Empty;

        [Category(Startup), Description("Bot command prefix.")]
        public string CommandPrefix { get; set; } = "$";

        [Category(Startup), Description("List of modules that will not be loaded when the bot is started (comma separated).")]
        public string ModuleBlacklist { get; set; } = string.Empty;

        [Category(Startup), Description("Toggle to handle commands asynchronously or synchronously.")]
        public bool AsyncCommands { get; set; }

        [Category(Startup), Description("Custom Status for playing a game. Add {0} to add a trade number updater \"On Trade #256\".")]
        public string BotGameStatus { get; set; } = "{0} SysBot.NET: Pokémon";

        [Category(Operation), Description("Custom message the bot will reply with when a user says hello to it. Use string formatting to mention the user in the reply.")]
        public string HelloResponse { get; set; } = "Hi {0}!";

        // Whitelists

        [Category(Operation), Description("Discord Role Settings")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public DiscordRoleSettings RoleSettings { get; set; } = new DiscordRoleSettings();

        // Operation

        [Category(Operation), Description("Users with these user IDs cannot use the bot.")]
        public string UserBlacklist { get; set; } = string.Empty;

        [Category(Operation), Description("Channels with these IDs are the only channels where the bot acknowledges commands.")]
        public string ChannelWhitelist { get; set; } = string.Empty;

        [Category(Operation), Description("Comma separated Discord user IDs that will have sudo access to the Bot Hub.")]
        public string GlobalSudoList { get; set; } = string.Empty;

        [Category(Operation), Description("Disabling this will remove global sudo support.")]
        public bool AllowGlobalSudo { get; set; } = true;

        [Category(Operation), Description("Comma separated channel IDs that will echo the log bot data.")]
        public string LoggingChannels { get; set; } = string.Empty;

        [Category(Operation), Description("Pings a User for when an Alt Discord Account is Detected. Leave blank for no Alt Detection.")]
        public string PingUserOnAltDetection { get; set; } = string.Empty;

        [Category(Operation), Description("Comma separated Logger channel IDs that will log trade start messages.")]
        public string TradeStartingChannels { get; set; } = string.Empty;

        [Category(Operation), Description("Comma separated Echo channel IDs that will log special messages.")]
        public string EchoChannels { get; set; } = string.Empty;

        [Category(Operation), Description("Enabling this will allow your users to use $queueStatus, $queueClear, $convert, and $legalize (and their aliases) in Direct Messages with your bot.")]
        public bool AllowCommandsThroughDM { get; set; } = false;

        [Category(Operation), Description("Enabling this will make the bot post an Overall queue count along with Specific queue count")]
        public bool PostOverallQueueCount { get; set; } = true;

        [Category(Operation), Description("Message to send over a time interval into Whitelisted Channels. Add {0} for a line break. Leave blank to remove Timed Messages.")]
        public string TimedMessage { get; set; } = string.Empty;

        [Category(Operation), Description("Timer for Timed Messages in minutes.")]
        public int TimedMessagesTimer { get; set; } = 15;

        [Category(Operation), Description("Returns PK8s and Showdown Sets of Pokémon shown in the trade to the user.")]
        public bool ReturnPK8s { get; set; } = true;

        [Category(Operation), Description("Display which Pokémon the user requested.")]
        public bool DisplayPokeName { get; set; } = false;
    }

    public class DiscordRoleSettings
    {
        private const string Roles = nameof(Roles);
        private const string DefaultDisable = "DISABLE";
        public override string ToString() => "Discord Role Settings";

        // Role Settings

        [Category(Roles), Description("Users with this role are allowed to enter the Trade queue.")]
        public string RoleCanTrade { get; set; } = DefaultDisable;

        [Category(Roles), Description("Users with this role are allowed to enter the Seed Check queue.")]
        public string RoleCanSeedCheck { get; set; } = DefaultDisable;

        [Category(Roles), Description("Users with this role are allowed to enter the Clone queue.")]
        public string RoleCanClone { get; set; } = DefaultDisable;

        [Category(Roles), Description("Users with this role are allowed to enter the FixOT queue.")]
        public string RoleCanFixOT { get; set; } = DefaultDisable;

        [Category(Roles), Description("Users with this role are allowed to enter the PowerUp queue.")]
        public string RoleCanPowerUp { get; set; } = DefaultDisable;

        [Category(Roles), Description("Users with this role are allowed to enter the EggRoll queue.")]
        public string RoleCanEggRoll { get; set; } = DefaultDisable;

        [Category(Roles), Description("Users with this role are allowed to enter the Dump queue.")]
        public string RoleCanDump { get; set; } = DefaultDisable;

        [Category(Roles), Description("Users with this role are allowed to enter the LanTrade queue.")]
        public string RoleCanLanTrade { get; set; } = DefaultDisable;

        [Category(Roles), Description("Users with this role are allowed to enter the LanRoll queue.")]
        public string RoleCanLanRoll { get; set; } = DefaultDisable;

        [Category(Roles), Description("Users with this role are allowed to remotely control the console (if running as Remote Control Bot.")]
        public string RoleRemoteControl { get; set; } = DefaultDisable;

        [Category(Roles), Description("Users with this role are allowed to bypass command restrictions.")]
        public string RoleSudo { get; set; } = DefaultDisable;

        [Category(Roles), Description("Users with this role are allowed to join the queue with a better position.")]
        public string RoleFavored { get; set; } = DefaultDisable;
    }
}