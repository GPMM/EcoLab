using CubicHex;
using System;
using System.Collections.Generic;

namespace EcoClean.TimeManaging.Domain
{
    [Serializable]
    public class SimulationInstance
    {
        #region Constructors
        public SimulationInstance(HexMap hexMap)
        {
            if (hexMap is null)
            {
                string message = "HexMap fed to SimulatorInstance is null.";

                ArgumentNullException exception = new ArgumentNullException("hexMap", message);

                ErrorHandler.LogError(message, exception);
            }

            this.hexMap = hexMap;

            simulationId = Guid.NewGuid().ToString();
        }
        #endregion Constructors

        #region Local variables
        private int nextDay = 0;
        #endregion Local variables

        #region Fields
        public readonly List<Tick> ticks = new List<Tick>();
        public readonly HexMap hexMap;
        public readonly string simulationId;
        #endregion Fields

        #region Methods
        /// <summary>
        /// Generates and returns the next tick in the simulation.
        /// </summary>
        /// <returns>The newly instanced tick</returns>
        public Tick GetNextTick()
        {
            Tick tick = new Tick(nextDay++, simulationId);

            ticks.Add(tick);

            return tick;
        }
        #endregion Methods
    }
}
