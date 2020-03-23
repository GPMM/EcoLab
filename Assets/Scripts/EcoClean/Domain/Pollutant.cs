using UnityEngine;

namespace EcoClean.Domain
{
    public class Pollutant
    {
        #region Constructors
        public Pollutant(string name, Color pollutantColor)
        {
            Name = name;
            PollutantColor = pollutantColor;
        }
        #endregion

        #region Properties
        public string Name { get; }
        public Color PollutantColor { get; }
        #endregion

        #region Operators
        public static bool operator ==(Pollutant a, Pollutant b)
        {
            if (a is null || b is null)
            {
                return a is null && b is null;
            }

            return a.Name == b.Name;
        }
        public static bool operator !=(Pollutant a, Pollutant b)
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
                Pollutant r = (Pollutant)obj;
                return Name == r.Name;
            }
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        #endregion
    }
}
