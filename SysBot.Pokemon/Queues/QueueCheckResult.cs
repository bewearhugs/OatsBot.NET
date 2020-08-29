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

        public static readonly QueueCheckResult<T> None = new QueueCheckResult<T>();

        public QueueCheckResult(bool inQueue = false, TradeEntry<T>? detail = default, int position = -1, int overallPosition = -1, int queueCount = -1, int overallQueueCount = -1)
        {
            InQueue = inQueue;
            Detail = detail;
            Position = position;
            OverallPosition = overallPosition;
            QueueCount = queueCount;
            OverallQueueCount = overallQueueCount;
        }

        public string GetMessage()
        {
            if (!InQueue || Detail is null)
                return "You are not in the queue.";
            var position = $"{Position}/{QueueCount}";
            var overallPosition = $"{OverallPosition}/{OverallQueueCount}";
            var msg = $"You are in the {Detail.Type} queue! Position per Queue: [{Detail.Type}: {position}; Overall: {overallPosition}] (ID {Detail.Trade.ID})";
            var pk = Detail.Trade.TradeData;
            if (pk.Species != 0)
                msg += $", Receiving: {(Species)Detail.Trade.TradeData.Species}";
            return msg;
        }
    }
}