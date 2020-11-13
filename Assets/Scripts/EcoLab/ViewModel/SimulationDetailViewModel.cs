using System;

namespace EcoLab.ViewModel
{
    [Serializable]
    public class SimulationDetailViewModel
    {
        #region Constructors
        public SimulationDetailViewModel(int simulationDetailId, int type, string name, float value)
        {
            SimulationInstanceId = simulationDetailId;
            Type = type;
            Name = name;
            Value = value;
        }
        #endregion Constructors

        #region Properties
        public int SimulationInstanceId { get; }

        /// <summary>
        /// 0: NULL
        /// 1: Microorganism
        /// 2: Pollutant
        /// 3: Consumption (amount of pollutant consumed by a microorganism)
        /// </summary>
        public int Type { get; }
        public string Name { get; }
        public float Value { get; }
        #endregion Properties
    }
}
