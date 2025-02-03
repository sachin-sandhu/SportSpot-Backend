using System.ComponentModel;

namespace SportSpot.V1.Session.Dtos
{
    public record SessionUserSearchQueryDto
    {
        [DefaultValue(10)]
        public int Size { get; init; } = 10;

        [DefaultValue(0)]
        public int Page { get; init; } = 0;
        [DefaultValue(false)]
        public bool WithExpired { get; init; } = false;
        [DefaultValue(true)]
        public bool IsMember { get; init; } = true;
        [DefaultValue(true)]
        public bool Ascending { get; init; } = true;
    }
}
