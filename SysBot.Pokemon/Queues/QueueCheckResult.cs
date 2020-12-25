using PKHeX.Core;

namespace SysBot.Pokemon
{
    public class QueueCheckResult<T> where T : PKM, new()
    {
        public readonly bool InQueue;
        public readonly TradeEntry<T>? Detail;
        public readonly int Position;
        public readonly int OverallPosition;
        public readonly int QueueCount;
        public readonly int OverallQueueCount;
        public readonly PokeTradeHub<T>? Hub;

        public static readonly QueueCheckResult<T> None = new();

        public QueueCheckResult(bool inQueue = false, TradeEntry<T>? detail = default, int position = -1, int overallPosition = -1, int queueCount = -1, int overallQueueCount = -1, PokeTradeHub<T>? hub = default)
        {
            InQueue = inQueue;
            Detail = detail;
            Position = position;
            OverallPosition = overallPosition;
            QueueCount = queueCount;
            OverallQueueCount = overallQueueCount;
            Hub = hub;
        }

        public string GetMessage()
        {
            if (!InQueue || Detail is null)
                return "You are not in the queue.";
            var position = $"{Position}/{QueueCount}";
            string overallPosition;
            if (QueueCount != OverallQueueCount)
                if (Detail.Type == PokeRoutineType.SeedCheck && Hub.Config.Queues.FlexMode == FlexYieldMode.LessCheatyFirst)
                    overallPosition = $" | __Overall: {Position}/{OverallQueueCount}__";
                else
                    overallPosition = $" | __Overall: {OverallPosition}/{OverallQueueCount}__";
            else
                overallPosition = $"";
            var msg = $"You are in the **{Detail.Type}** queue! Position: __{Detail.Type}: {position}__{overallPosition} (ID {Detail.Trade.ID})";
            var pk = Detail.Trade.TradeData;
            if (pk.Species != 0)
                if (Detail.Type == PokeRoutineType.EggRoll)
                    msg += $". Receiving: Mysterious Egg";
                else if (Detail.Type == PokeRoutineType.LanRoll)
                    msg += $". Receiving: An Illegal Egg";
                else
                    msg += $". Receiving: {(Species)Detail.Trade.TradeData.Species}";
            return msg;
        }
    }
}