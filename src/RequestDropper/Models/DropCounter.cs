﻿using System;

namespace RequestDropper.Models
{
    public struct DropCounter
    {
        public DropCounter(int count, DateTimeOffset timeStamp)
        {
            Count = count;
            TimeStamp = timeStamp;
        }

        public int Count { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
    }
}
