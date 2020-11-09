using EcoLab.TimeManaging.Domain;

namespace EcoLab.Domain
{
    public class Metadata
    {
        public Metadata(string userId, Tick tick)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                ErrorHandler.LogError("User set an empty name.");
            }

            ErrorHandler.AssertNull(tick);

            UserId = userId;
            Tick = tick;
        }

        public string UserId { get; }

        public Tick Tick { get; }
    }
}
