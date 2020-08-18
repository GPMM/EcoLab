using CubicHex;
using System;
using System.Collections.Generic;

namespace EcoClean.TimeManaging.Domain
{
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

            HexMap = hexMap;
        }
        #endregion Constructors

        #region Local variables
        private int nextDay = 0;
        #endregion Local variables

        #region Properties
        public List<Tick> Ticks { get; } = new List<Tick>();
        public HexMap HexMap { get; }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Generates and returns the next tick in the simulation.
        /// </summary>
        /// <returns>The newly instanced tick</returns>
        public Tick GetNextTick()
        {
            Tick tick = new Tick(nextDay++);

            Ticks.Add(tick);

            return tick;
        }
        #endregion Methods
    }
}
