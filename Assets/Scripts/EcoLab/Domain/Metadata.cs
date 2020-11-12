using EcoLab.TimeManaging.Domain;
using EcoLab.ViewModel;
using System.Collections.Generic;

namespace EcoLab.Domain
{
    public class Metadata
    {
        public Metadata(string userId, Tick tick)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                ErrorHandler.LogError("User set an empty name.");
            }

            ErrorHandler.AssertNull(tick);

            UserId = userId;
            Tick = tick;
        }

        public string UserId { get; }

        public Tick Tick { get; }

        public SimulationInstanceViewModel ToSimulationInstance()
        {
            string userId = MetadataManager.Instance.UserId;
            string simulationId = Tick.SimulationId;
            int day = Tick.Day;

            SimulationInstanceViewModel simulationInstanceViewModel = new SimulationInstanceViewModel(
                userId,
                simulationId,
                day);

            return simulationInstanceViewModel;
        }

        public List<SimulationDetailViewModel> ToSimulationDetailList(int simulationInstanceId)
        {
            List<SimulationDetailViewModel> list = new List<SimulationDetailViewModel>();

            foreach (Microorganism microorganism in Tick.MicroorganismAmount.Keys)
            {
                SimulationDetailViewModel viewModel = new SimulationDetailViewModel(
                    simulationInstanceId,
                    (int)ElementType.MICROORGANISM,
                    microorganism.name,
                    Tick.MicroorganismAmount[microorganism]);

                list.Add(viewModel);
            }

            foreach (Pollutant pollutant in Tick.PollutantAmount.Keys)
            {
                SimulationDetailViewModel viewModel = new SimulationDetailViewModel(
                    simulationInstanceId,
                    (int)ElementType.POLLUTANT,
                    pollutant.name,
                    Tick.PollutantAmount[pollutant]);

                list.Add(viewModel);
            }

            foreach (Consumption consumption in Tick.ConsumptionPerMicroorganism.Keys)
            {
                SimulationDetailViewModel viewModel = new SimulationDetailViewModel(
                    simulationInstanceId,
                    (int)ElementType.CONSUMPTION,
                    consumption.name,
                    Tick.ConsumptionPerMicroorganism[consumption]);

                list.Add(viewModel);
            }

            return list;
        }
    }
}
