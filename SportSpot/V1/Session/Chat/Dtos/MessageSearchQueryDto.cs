﻿using System.ComponentModel;

namespace SportSpot.V1.Session.Chat.Dtos
{
    public record MessageSearchQueryDto
    {
        [DefaultValue(10)]
        public int Size { get; init; } = 10;

        [DefaultValue(0)]
        public int Page { get; init; } = 0;
        public Guid? SenderId { get; init; }
        public string? Content { get; init; }
        public DateTime? StartTime { get; init; }
        public DateTime? EndTime { get; init; }
        [DefaultValue(true)]
        public bool Ascending { get; init; } = true;
    }
}
