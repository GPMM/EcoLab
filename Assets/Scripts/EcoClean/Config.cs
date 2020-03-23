using System.Collections.Generic;

namespace EcoClean
{
    public class Config
    {
        #region Constructors
        private Config() { }
        #endregion

        #region Constants
        public const float MICROORGANISMS_TO_TRIGGER_DIVISION = 1.0f;
        public const float MICROORGANISM_CONSUMPTION_RANDOM_VARIATION = 0.15f;

        public const float SLOT_MAX_MICROORGANISMS = 1.0f;
        public const float SLOT_MAX_REAGENTS = 5.0f;

        public const float SECONDS_PER_TICK = 1f;

        // Different game speeds
        public readonly static float[] TIME_STEP_MULTIPLIERS = new float[]
        {
            0.25f,
            0.5f,
            1f,
            2f,
            4f
        };
        #endregion
    }
}
