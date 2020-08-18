using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EcoClean.Domain
{
    public abstract class Element
    {
        #region Constructors
        public Element(string name, Color elementColor, ElementType elementType)
        {
            Name = name;
            ElementColor = elementColor;
            ElementType = elementType;
        }
        #endregion Constructors

        #region Properties
        public ElementType ElementType { get; }
        public string Name { get; }
        public Color ElementColor { get; }
        #endregion Properties
    }
}
