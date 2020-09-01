using UnityEngine;

namespace EcoClean.Domain
{
    public class Consumption : Element
    {
        #region Constructors
        public Consumption(Microorganism microorganism, Pollutant pollutant) : base(
            microorganism.name + "-" + pollutant.name,
            Color.Lerp(microorganism.elementColor, pollutant.elementColor, 0.5f),
            ElementType.CONSUMPTION)
        {
            this.microorganism = microorganism;
            this.pollutant = pollutant;
        }
        #endregion Constructors

        #region Fields
        public readonly Microorganism microorganism;
        public readonly Pollutant pollutant;
        #endregion Fields

        #region Operators
        public static bool operator ==(Consumption a, Consumption b)
        {
            if (a is null || b is null)
            {
                return a is null && b is null;
            }

            return a.microorganism == b.microorganism && a.pollutant == b.pollutant;
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
            return name.GetHashCode();
        }
        public override string ToString()
        {
            return name;
        }
        #endregion Operators
    }
}
