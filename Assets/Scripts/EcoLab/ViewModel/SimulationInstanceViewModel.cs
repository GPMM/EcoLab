using System;

namespace EcoLab.ViewModel
{
    [Serializable]
    public class SimulationInstanceViewModel
    {
        #region Constructors
        public SimulationInstanceViewModel(string userId, string simulationId, int day)
        {
            UserId = userId;
            SimulationId = simulationId;
            Day = day;
        }
        #endregion Constructors

        #region Properties
        public string UserId { get; }
        public string SimulationId { get; }
        public int Day { get; }
        #endregion Properties
    }
}
