using EcoClean.Domain;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EcoClean
{
    public class Repository
    {
        #region Constructors
        private Repository() { }
        #endregion

        #region Methods
        public static IEnumerable<Microorganism> GetMicroorganisms()
        {
            List<Microorganism> microorganisms = new List<Microorganism>()
            {
                new Microorganism("Bacteria A", Color.blue, 0.05f),
                new Microorganism("Bacteria B", Color.red, 0.03f),
                new Microorganism("Bacteria C", Color.yellow, 0.02f)
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
                new Pollutant("Pollutant A", Color.green),
                new Pollutant("Pollutant B", Color.magenta)
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

        private static Dictionary<Tuple<string, string>, float> reactionTable = new Dictionary<Tuple<string, string>, float>()
        {
            { new Tuple<string, string>("Bacteria A", "Pollutant A"), 0.21f },
            { new Tuple<string, string>("Bacteria B", "Pollutant A"), 0.12f },
            { new Tuple<string, string>("Bacteria C", "Pollutant A"), -0.09f },
            { new Tuple<string, string>("Bacteria A", "Pollutant B"), -0.08f },
            { new Tuple<string, string>("Bacteria B", "Pollutant B"), -0.1f },
            { new Tuple<string, string>("Bacteria C", "Pollutant B"), 0.11f }
        };

        /// <summary>
        /// Returns the result of the reaction between the Microorganism and the Pollutant, by how much a Microorganism's energy is incremented or decremented
        /// </summary>
        /// <param name="microorganism">The Microorganism name</param>
        /// <param name="pollutant">The Pollutant name</param>
        /// <returns>The amount of change in the microorganism's energy per tick</returns>
        public static float GetReaction (string microorganism, string pollutant)
        {
            return reactionTable[new Tuple<string, string>(microorganism, pollutant)];
        }
        #endregion
    }
}
