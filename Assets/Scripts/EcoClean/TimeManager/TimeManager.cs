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

            if (Config.GRAPH_SHOW_MICROORGANISMS)
            {
                foreach (Microorganism microorganism in tick.MicroorganismAmount.Keys)
                {
                    float amount = tick.MicroorganismAmount[microorganism];

                    if (amount > 0)
                    {
                        GraphManager.Instance.RenderGraph(microorganism, amount, tick.Day, maxValueMicroorganisms);
                    }
                }
            }

            if (Config.GRAPH_SHOW_POLLUTANTS)
            {
                foreach (Pollutant pollutant in tick.PollutantAmount.Keys)
                {
                    float amount = tick.PollutantAmount[pollutant];

                    if (amount > 0)
                    {
                        GraphManager.Instance.RenderGraph(pollutant, amount, tick.Day, maxValuePollutants);
                    }
                }
            }

            if (Config.GRAPH_SHOW_CONSUMPTION)
            {
                foreach (Consumption consumption in tick.ConsumptionPerMicroorganism.Keys)
                {
                    float amount = tick.ConsumptionPerMicroorganism[consumption];
                    float maxValueConsumption = Repository.GetReaction(consumption.microorganism, consumption.pollutant) * GameManager.Instance.hexMap.CachedHexCount;

                    if (amount > 0)
                    {
                        GraphManager.Instance.RenderGraph(consumption, amount, tick.Day, maxValueConsumption);
                    }
                }
            }
        }
        #endregion Methods
    }
}