namespace EcoClean
{
    public static class Config
    {
        #region Constants
        public const float MICROORGANISMS_TO_TRIGGER_DIVISION = 1.0f;
        public const float MICROORGANISM_CONSUMPTION_RANDOM_VARIATION = 0.15f;

        public const float SLOT_MAX_MICROORGANISMS = 1.0f;
        public const float SLOT_MAX_REAGENTS = 5.0f;

        public const float SECONDS_PER_TICK = 1f;

        // Different game speeds
        public const int TIME_STEP_DEFAULT = 0;
        public readonly static float[] TIME_STEP_MULTIPLIERS = new float[]
        {
            1f,
            2f,
            4f,
            8f
        };

        // UI
        public const float UI_GRAPH_NODE_DIAMETER = 5f;
        public const float UI_GRAPH_EDGE_THICKNESS = 3f;
        public const float UI_GRAPH_EDGE_ALPHA = 0.5f;
        public const float UI_GRAPH_SEPARATOR_THICKNESS = 2f;
        #endregion
    }
}
