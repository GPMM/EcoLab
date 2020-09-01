using CubicHex;
using EcoClean.Domain;
using EcoClean.TimeManaging.Domain;
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
        public static SimulationInstance CurrentSimulation { get; private set; }
        #endregion Properties

        #region Local variables
        private static List<SimulationInstance> simulationInstances = new List<SimulationInstance>();
        #endregion Local variables

        #region Methods
        public static void StartNewSimulation(HexMap hexMap)
        {
            CurrentSimulation = new SimulationInstance(hexMap);

            GraphManager.Instance.ResetGraph();

            // Calculate day 0 data.
            CalculateNewTick(null, true);

            simulationInstances.Add(CurrentSimulation);
        }

        public static void CalculateNewTick(Dictionary<Consumption, float> consumptionPerMicroorganism, bool updateGraph)
        {
            List<Hex> hexes = CurrentSimulation.hexMap.AllHexes;
            Tick tick = CurrentSimulation.GetNextTick();

            if (!(consumptionPerMicroorganism is null))
            {
                tick.consumptionPerMicroorganism = consumptionPerMicroorganism;
            }

            foreach (Hex hex in hexes)
            {
                PetriDishSlot petriDishSlot = (PetriDishSlot)hex;

                if (!(petriDishSlot.Microorganism is null))
                {
                    tick.microorganismAmount[petriDishSlot.Microorganism] += petriDishSlot.MicroorganismAmount;
                }

                if (!(petriDishSlot.Pollutant is null))
                {
                    tick.pollutantAmount[petriDishSlot.Pollutant] += petriDishSlot.PollutantAmount;
                }
            }

            if (updateGraph)
            {
                UpdateGraph(tick);
            }

            NextTick = tick.day + 1;
        }

        private static void UpdateGraph(Tick tick)
        {
            float maxValueMicroorganisms = Config.SLOT_MAX_MICROORGANISMS * GameManager.Instance.hexMap.CachedHexCount;
            float maxValuePollutants = Config.SLOT_MAX_REAGENTS * GameManager.Instance.hexMap.CachedHexCount;

            foreach (Microorganism microorganism in tick.microorganismAmount.Keys)
            {
                float amount = tick.microorganismAmount[microorganism];

                if (amount > 0)
                {
                    GraphManager.Instance.RenderGraph(microorganism, amount, tick.day, maxValueMicroorganisms);
                }
            }

            foreach (Pollutant pollutant in tick.pollutantAmount.Keys)
            {
                float amount = tick.pollutantAmount[pollutant];

                if (amount > 0)
                {
                    GraphManager.Instance.RenderGraph(pollutant, amount, tick.day, maxValuePollutants);
                }
            }

            foreach (Consumption consumption in tick.consumptionPerMicroorganism.Keys)
            {
                float amount = tick.consumptionPerMicroorganism[consumption];
                float maxValueConsumption = Repository.GetReaction(consumption.microorganism, consumption.pollutant) * GameManager.Instance.hexMap.CachedHexCount;

                if (amount > 0)
                {
                    GraphManager.Instance.RenderGraph(consumption, amount, tick.day, maxValueConsumption);
                }
            }
        }
        #endregion Methods
    }
}