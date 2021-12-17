using System;

namespace RequestDropper.Models
{
    public struct DropCounter : IEquatable<DropCounter>
    {
        public DropCounter(int count, DateTimeOffset timeStamp)
        {
            Count = count;
            TimeStamp = timeStamp;
        }

        public int Count { get; set; }
        public DateTimeOffset TimeStamp { get; set; }

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
