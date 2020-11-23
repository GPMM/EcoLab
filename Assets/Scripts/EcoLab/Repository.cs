using EcoLab.Domain;
using EcoLab.TimeManaging;
using EcoLab.TimeManaging.Domain;
using EcoLab.ViewModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using UnityEngine;

namespace EcoLab
{
    public static class Repository
    {
        #region Static variables
        private static readonly HttpClient client = new HttpClient();
        #endregion Static variables

        #region Local variables
        // TODO: These are hard-coded for the purpose of development
        private static Microorganism PROTOBacteriaA = new Microorganism(
            "Micrococcus", new Color(219/255f, 77/255f, 77/255f, 1f), 0.03f);
        private static Microorganism PROTOBacteriaB = new Microorganism(
            "Arthrobacter", new Color(57/255f, 163/255f, 64/255f, 1f), 0.03f);
        private static Microorganism PROTOBacteriaC = new Microorganism(
            "Bacillus", new Color(222/255f, 180/255f, 67/255f, 1f), 0.03f);
        private static Microorganism PROTOBacteriaD = new Microorganism(
            "Burkholderia", new Color(106/255f, 73/255f, 214/255f, 1f), 0.03f);
        private static Microorganism PROTOBacteriaE = new Microorganism(
            "Staphylococcus", new Color(56/255f, 145/255f, 209/255f, 1f), 0.03f);

        private static Pollutant PROTOPollutantA = new Pollutant("Diesel", Color.cyan);
        private static Pollutant PROTOPollutantB = new Pollutant("Biodiesel", Color.magenta);
        #endregion Local variables

        #region Methods
        public static IEnumerable<Microorganism> GetMicroorganisms()
        {
            List<Microorganism> microorganisms = new List<Microorganism>()
            {
                PROTOBacteriaA,
                PROTOBacteriaB,
                PROTOBacteriaC,
                PROTOBacteriaD,
                PROTOBacteriaE
            };

            microorganisms.Sort((x, y) => x.name.CompareTo(y.name));

            return microorganisms;
        }

        public static Dictionary<string, Microorganism> GetMicroorganismsDictionary()
        {
            Dictionary<string, Microorganism> microorganisms = new Dictionary<string, Microorganism>();

            foreach (Microorganism microorganism in GetMicroorganisms())
            {
                microorganisms.Add(microorganism.name, microorganism);
            }
            
            return microorganisms;
        }

        public static IEnumerable<Pollutant> GetPollutants()
        {
            List<Pollutant> pollutants = new List<Pollutant>()
            {
                PROTOPollutantA,
                PROTOPollutantB
            };

            pollutants.Sort((x, y) => x.name.CompareTo(y.name));

            return pollutants;
        }

        public static Dictionary<string, Pollutant> GetPollutantsDictionary()
        {
            Dictionary<string, Pollutant> pollutants = new Dictionary<string, Pollutant>();

            foreach (Pollutant pollutant in GetPollutants())
            {
                pollutants.Add(pollutant.name, pollutant);
            }

            return pollutants;
        }

        private static Dictionary<Consumption, float> reactionTable = new Dictionary<Consumption, float>()
        {
            { new Consumption(PROTOBacteriaA, PROTOPollutantA), 0.21f },
            { new Consumption(PROTOBacteriaA, PROTOPollutantB), -0.08f },
            { new Consumption(PROTOBacteriaB, PROTOPollutantA), 0.12f },
            { new Consumption(PROTOBacteriaB, PROTOPollutantB), 0.15f },
            { new Consumption(PROTOBacteriaC, PROTOPollutantA), -0.09f },
            { new Consumption(PROTOBacteriaC, PROTOPollutantB), 0.11f },
            { new Consumption(PROTOBacteriaD, PROTOPollutantA), 0.12f },
            { new Consumption(PROTOBacteriaD, PROTOPollutantB), 0.15f },
            { new Consumption(PROTOBacteriaE, PROTOPollutantA), 0.12f },
            { new Consumption(PROTOBacteriaE, PROTOPollutantB), 0.15f }
        };

        /// <summary>
        /// Returns the result of the reaction between the Microorganism and the Pollutant, by how much a Microorganism's energy is incremented or decremented
        /// </summary>
        /// <param name="microorganism">The Microorganism name</param>
        /// <param name="pollutant">The Pollutant name</param>
        /// <returns>The amount of change in the microorganism's energy per tick</returns>
        public static float GetReaction (Microorganism microorganism, Pollutant pollutant)
        {
            return reactionTable[new Consumption(microorganism, pollutant)];
        }

        public static async void UploadSimulationInstant()
        {
            // Finding the current tick
            Tick tick = TimeManager.CurrentSimulation.ticks.Last();

            if (tick is null)
            {
                return;
            }

            // Finding the metadata to save
            string userId = "defaultUserId";

            if (!(MetadataManager.Instance is null))
            {
                userId = MetadataManager.Instance.UserId;
            }

            Metadata metadata = new Metadata(userId, tick);

            // Saving the simulation instance and preparing it to receive further data
            // regarding the elements contained within the petri dish
            SimulationInstanceViewModel simulationInstanceViewModel = metadata.ToSimulationInstance();

            int id = await SaveSimulationInstance(simulationInstanceViewModel);

            // Saving the elements data
            List<SimulationDetailViewModel> simulationDetailViewModelList = metadata.ToSimulationDetailList(id);

            SaveSimulationDetails(simulationDetailViewModelList);
        }

        private static async Task<int> SaveSimulationInstance(SimulationInstanceViewModel simulationInstanceViewModel)
        {
            string json = JsonConvert.SerializeObject(simulationInstanceViewModel, Formatting.None);
            json = HttpUtility.UrlEncode(json);

            string result = await client.GetStringAsync(Config.URL_SAVE_SIMINSTANCE + json);

            int intResult;
            int.TryParse(result, out intResult);

            return intResult;
        }

        private static async void SaveSimulationDetails(List<SimulationDetailViewModel> simulationDetailViewModelList)
        {
            foreach (SimulationDetailViewModel item in simulationDetailViewModelList)
            {
                string json = JsonConvert.SerializeObject(item, Formatting.None);
                json = HttpUtility.UrlEncode(json);

                await client.GetStringAsync(Config.URL_SAVE_SIMDETAIL + json);
            }
        }
        #endregion
    }
}
