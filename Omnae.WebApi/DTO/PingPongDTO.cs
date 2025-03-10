using System;

namespace Omnae.WebApi.DTO
{
    public class PingPongDTO
    {
        public bool Pong { get; set; } = true;

        public DateTimeOffset ServerTime { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset ServerTimeUTC { get; set; } = DateTimeOffset.UtcNow;

        public string Name { get; set; }
    }
}