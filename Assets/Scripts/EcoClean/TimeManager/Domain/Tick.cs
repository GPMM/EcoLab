using EcoClean;
using EcoClean.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EcoClean.TimeManaging.Domain
{
    public class Tick
    {
        #region Constructors
        public Tick(int day)
        {
            if (day < 0)
            {
                ErrorHandler.LogError("The integer Day cannot be lower than 0.");
            }

            InitiateCountedIndexes();

            Day = day;
        }
        #endregion Constructors

        #region Properties
        public int Day { get; }

        public Dictionary<Microorganism, float> MicroorganismAmount
        {
            get;
        } = new Dictionary<Microorganism, float>();

        public Dictionary<Pollutant, float> PollutantAmount
        {
            get;
        } = new Dictionary<Pollutant, float>();

        public Dictionary<Tuple<Microorganism, Pollutant>, float> ConsumptionPerMicroorganism
        {
            get;
            set;
        } = new Dictionary<Tuple<Microorganism, Pollutant>, float>();
        #endregion Properties

        #region Methods
        /// <summary>
        /// This method is used in the constructor to ensure no indexes need to be null-checked
        /// every single iteration of the counting method.
        /// </summary>
        /// <param name="tick"></param>
        private void InitiateCountedIndexes()
        {
            List<Microorganism> microorganisms = Repository.GetMicroorganisms().ToList();
            List<Pollutant> pollutants = Repository.GetPollutants().ToList();

            // Ensures all microorganisms are set in the dictionary
            foreach (Microorganism microorganism in microorganisms)
            {
                MicroorganismAmount.Add(microorganism, 0);
            }

            // Ensures all pollutants are set in the dictionary
            foreach (Pollutant pollutant in pollutants)
            {
                PollutantAmount.Add(pollutant, 0);
            }

            // Ensures all microorganism-pollutant relations are set in the dictionary
            // TODO: Maybe make this configurable? This is already being set by GameLogic
            //ConsumptionPerMicroorganism = GetEmptyConsumptionPerMicroorganism();
        }
        
        public static Dictionary<Tuple<Microorganism, Pollutant>, float> GetEmptyConsumptionPerMicroorganism()
        {
            List<Microorganism> microorganisms = Repository.GetMicroorganisms().ToList();
            List<Pollutant> pollutants = Repository.GetPollutants().ToList();

            Dictionary<Tuple<Microorganism, Pollutant>, float> consumptionPerMicroorganism = new Dictionary<Tuple<Microorganism, Pollutant>, float>();

            foreach (Microorganism microorganism in microorganisms)
            {
                foreach (Pollutant pollutant in pollutants)
                {
                    consumptionPerMicroorganism.Add(
                        new Tuple<Microorganism, Pollutant>(microorganism, pollutant),
                        0);
                }
            }

            return consumptionPerMicroorganism;
        }
        #endregion Methods
    }
}