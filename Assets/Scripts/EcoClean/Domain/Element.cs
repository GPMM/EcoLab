using UnityEngine;

namespace EcoClean.Domain
{
    public abstract class Element
    {
        #region Constructors
        public Element(string name, Color elementColor, ElementType elementType)
        {
            this.name = name;
            this.elementColor = elementColor;
            this.elementType = elementType;
        }
        #endregion Constructors

        #region Properties
        public readonly ElementType elementType;
        public readonly string name;
        public readonly Color elementColor;
        #endregion Properties
    }
}
