using UnityEngine;

namespace EcoClean.Domain
{
    public class Microorganism : Element
    {
        #region Constructors
        public Microorganism(string name, Color elementColor, float passiveEnergyLoss) : base(name, elementColor, ElementType.MICROORGANISM)
        {
            PassiveEnergyLoss = passiveEnergyLoss;
        }
        #endregion Constructors

        #region Fields
        public float PassiveEnergyLoss { get; }
        #endregion Fields

        #region Operators
        public static bool operator ==(Microorganism a, Microorganism b)
        {
            if (a is null || b is null)
            {
                return a is null && b is null;
            }

            return a.name == b.name;
        }
        public static bool operator !=(Microorganism a, Microorganism b)
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
                Microorganism m = (Microorganism)obj;
                return name == m.name;
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
