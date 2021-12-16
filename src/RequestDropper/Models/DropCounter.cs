using System;

namespace RequestDropper.Models
{
    public readonly struct DropCounter : IEquatable<DropCounter>
    {
        public DropCounter(int count, DateTimeOffset timeStamp)
        {
            Count = count;
            TimeStamp = timeStamp;
        }

        public int Count { get; }
        public DateTimeOffset TimeStamp { get; }

        #region Equality members

        public bool Equals(DropCounter other)
        {
            return this.Count == other.Count && this.TimeStamp.Equals(other.TimeStamp);
        }

        public override bool Equals(object? obj)
        {
            return obj is DropCounter other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Count, this.TimeStamp);
        }

        #endregion
    }
}
