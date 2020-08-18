using CubicHex;
using EcoClean.Domain;
using EcoClean.TimeManaging.Domain;
using System;
using System.Collections.Generic;

namespace EcoClean.TimeManaging
{
    public static class TimeManager
    {
        #region Properties
        public static int NextTick
        {
            get;
            private set;
        } = 0;
        #endregion Properties

        #region Local variables
        private static List<SimulationInstance> simulationInstances = new List<SimulationInstance>();
        private static SimulationInstance currentSimulation;
        #endregion Local variables

        #region Methods
        public static void StartNewSimulation(HexMap hexMap)
        {
            currentSimulation = new SimulationInstance(hexMap);

            GraphManager.Instance.ResetGraph();

            // Calculate day 0 data.
            CalculateNewTick(null, true);

            simulationInstances.Add(currentSimulation);
        }

        public static void CalculateNewTick(Dictionary<Tuple<Microorganism, Pollutant>, float> consumptionPerMicroorganism, bool updateGraph)
        {
            List<Hex> hexes = currentSimulation.HexMap.AllHexes;
            Tick tick = currentSimulation.GetNextTick();

            if (!(consumptionPerMicroorganism is null))
            {
                tick.ConsumptionPerMicroorganism = consumptionPerMicroorganism;
            }

            foreach (Hex hex in hexes)
            {
                PetriDishSlot petriDishSlot = (PetriDishSlot)hex;

                if (!(petriDishSlot.Microorganism is null))
                {
                    tick.MicroorganismAmount[petriDishSlot.Microorganism] += petriDishSlot.MicroorganismAmount;
                }

                if (!(petriDishSlot.Pollutant is null))
                {
                    tick.PollutantAmount[petriDishSlot.Pollutant] += petriDishSlot.PollutantAmount;
                }
            }

            if (updateGraph)
            {
                UpdateGraph(tick);
            }

            NextTick = tick.Day + 1;
        }

        private static void UpdateGraph(Tick tick)
        {
            float maxValueMicroorganisms = Config.SLOT_MAX_MICROORGANISMS * GameManager.Instance.hexMap.CachedHexCount;
            float maxValuePollutants = Config.SLOT_MAX_REAGENTS * GameManager.Instance.hexMap.CachedHexCount;

            foreach (Microorganism microorganism in tick.MicroorganismAmount.Keys)
            {
                float amount = tick.MicroorganismAmount[microorganism];

                if (amount > 0)
                {
                    GraphManager.Instance.RenderGraph(microorganism, amount, tick.Day, maxValueMicroorganisms);
                }
            }

            foreach (Pollutant pollutant in tick.PollutantAmount.Keys)
            {
                float amount = tick.PollutantAmount[pollutant];

                if (amount > 0)
                {
                    GraphManager.Instance.RenderGraph(pollutant, amount, tick.Day, maxValuePollutants);
                }
            }
        }
        #endregion Methods
    }
}