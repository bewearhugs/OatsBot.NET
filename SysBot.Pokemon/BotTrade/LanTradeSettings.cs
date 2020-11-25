using PKHeX.Core;
using System.ComponentModel;

namespace SysBot.Pokemon
{
    public class LanTradeSettings
    {
        private const string LanTradeConfig = nameof(LanTradeConfig);

        public override string ToString() => "Lan Trade Bot Settings";

        [Category(LanTradeConfig), Description("Only works on LAN trading routines. Helpful feature when using LanTrade that boots into LAN mode before every trade.")]
        public bool BootLanBeforeEachTrade { get; set; } = true;

        [Category(LanTradeConfig), Description("Only works on LAN trading routines. Helpful feature that allows users to request the bot to only trade them if it matches their IGN.")]
        public bool RequeueWhenSpecificIgnNotFound { get; set; } = true;
    }
}
