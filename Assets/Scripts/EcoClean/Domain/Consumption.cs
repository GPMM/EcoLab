using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EcoClean.Domain
{
    public class Consumption : Element
    {
        #region Constructors
        public Consumption(Microorganism microorganism, Pollutant pollutant) : base(
            microorganism.Name + "-" + pollutant.Name,
            Color.Lerp(microorganism.ElementColor, pollutant.ElementColor, 0.5f),
            ElementType.CONSUMPTION)
        {
            Microorganism = microorganism;
            Pollutant = pollutant;
        }
        #endregion Constructors

        #region Properties
        public Microorganism Microorganism { get; }
        public Pollutant Pollutant { get; }
        #endregion Properties

        #region Operators
        public static bool operator ==(Consumption a, Consumption b)
        {
            if (a is null || b is null)
            {
                return a is null && b is null;
            }

            return a.Microorganism == b.Microorganism && a.Pollutant == b.Pollutant;
        }
        public static bool operator !=(Consumption a, Consumption b)
        {
            return !(a == b);
        }
        public override bool Equals(object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Consumption m = (Consumption)obj;
                return this == m;
            }
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        #endregion Operators
    }
}
