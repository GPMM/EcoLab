using UnityEngine;

namespace EcoLab.Domain
{
    public class Pollutant : Element
    {
        #region Constructors
        public Pollutant(string name, Color elementColor) : base(name, elementColor, ElementType.POLLUTANT)
        {

        }
        #endregion

        #region Operators
        public static bool operator ==(Pollutant a, Pollutant b)
        {
            if (a is null || b is null)
            {
                return a is null && b is null;
            }

            return a.name == b.name;
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
                return name == r.name;
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
        #endregion
    }
}
