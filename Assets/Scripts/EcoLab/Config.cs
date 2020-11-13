namespace EcoLab
{
    public static class Config
    {
        #region Constants
        public const float MICROORGANISMS_TO_TRIGGER_DIVISION = 1.0f;
        public const float MICROORGANISM_CONSUMPTION_RANDOM_VARIATION = 0.20f;

        public const float SLOT_MAX_MICROORGANISMS = 1.0f;
        public const float SLOT_MAX_REAGENTS = 5.0f;

        public const float SECONDS_PER_TICK = 1f;

        // User controls
        public const int INSERTION_RADIUS_MICROORGANISM = 1;
        public const int INSERTION_RADIUS_POLLUTANT = 3;

        // Different game speeds
        public const int TIME_STEP_DEFAULT = 0;
        public readonly static float[] TIME_STEP_MULTIPLIERS = new float[]
        {
            1f,
            2f,
            4f,
            8f
        };

        // GRAPH
        public const bool GRAPH_SHOW_MICROORGANISMS = true;
        public const bool GRAPH_SHOW_POLLUTANTS = true;
        public const bool GRAPH_SHOW_CONSUMPTION = false;

        // UI
        public const float UI_COLOR_BLEND = 0.5f;
        public const float UI_GRAPH_NODE_DIAMETER = 5f;
        public const float UI_GRAPH_EDGE_THICKNESS = 3f;
        public const float UI_GRAPH_EDGE_ALPHA = 0.5f;
        public const float UI_GRAPH_SEPARATOR_THICKNESS = 2f;

        // Server
        public const string URL_SAVE_SIMINSTANCE = "https://albali.eic.cefet-rj.br/gpmm/ecolab/save_siminstance.php?json=";
        public const string URL_SAVE_SIMDETAIL = "https://albali.eic.cefet-rj.br/gpmm/ecolab/save_simdetail.php?json=";
        #endregion
    }
}
