using System.Threading;
using System.Windows.Input;

namespace KeyPressStat
{
    class StatisticsEntry
    {
        private int _count;

        public int Count => _count;

        public int VKCode { get; }
        public Key Key { get; }
        public string Character { get; }

        public StatisticsEntry(int vkCode, Key key, string character)
        {
            VKCode = vkCode;
            Key = key;
            Character = character;
        }

        public int Increment()
            => Interlocked.Increment(ref _count);

        public int Decrement()
            => Interlocked.Decrement(ref _count);

        public override string ToString()
            => $"{Character}: {Count}";
    }
}
