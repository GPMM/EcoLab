using EcoClean.Domain;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EcoClean
{
    public static class Repository
    {
        #region Methods
        private static Microorganism PROTOBacteriaA = new Microorganism("Bacteria A", Color.blue, 0.05f);
        private static Microorganism PROTOBacteriaB = new Microorganism("Bacteria B", Color.red, 0.03f);
        private static Microorganism PROTOBacteriaC = new Microorganism("Bacteria C", Color.yellow, 0.02f);

        private static Pollutant PROTOPollutantA = new Pollutant("Pollutant A", Color.cyan);
        private static Pollutant PROTOPollutantB = new Pollutant("Pollutant B", Color.magenta);

        public static IEnumerable<Microorganism> GetMicroorganisms()
        {
            List<Microorganism> microorganisms = new List<Microorganism>()
            {
                PROTOBacteriaA,
                PROTOBacteriaB,
                PROTOBacteriaC
            };

            microorganisms.Sort((x, y) => x.Name.CompareTo(y.Name));

            return microorganisms;
        }

        public static Dictionary<string, Microorganism> GetMicroorganismsDictionary()
        {
            Dictionary<string, Microorganism> microorganisms = new Dictionary<string, Microorganism>();

            foreach (Microorganism microorganism in GetMicroorganisms())
            {
                microorganisms.Add(microorganism.Name, microorganism);
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

            pollutants.Sort((x, y) => x.Name.CompareTo(y.Name));

            return pollutants;
        }

        public static Dictionary<string, Pollutant> GetPollutantsDictionary()
        {
            Dictionary<string, Pollutant> pollutants = new Dictionary<string, Pollutant>();

            foreach (Pollutant pollutant in GetPollutants())
            {
                pollutants.Add(pollutant.Name, pollutant);
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
            { new Consumption(PROTOBacteriaC, PROTOPollutantB), 0.11f }
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
        #endregion
    }
}
