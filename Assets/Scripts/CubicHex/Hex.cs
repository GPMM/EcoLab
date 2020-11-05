using System;
using UnityEngine;

namespace CubicHex
{
    /// <summary>
    /// Following Quill18's implementation of Hex coordinates
    /// https://www.youtube.com/watch?v=j-rCuN7uMR8&list=PLbghT7MmckI7JHf0pdEQ8fbPb-LoDXEno&index=2&t=0s
    /// And Hexagonal Grids from RedBlobGames at
    /// https://www.redblobgames.com/grids/hexagons/
    /// </summary>
    public class Hex
    {
        #region Constructors
        public Hex(int q, int r)
        {
            Q = q;
            R = r;
            S = -(q + r);
        }
        #endregion

        #region Local variables
        static readonly float radius = 0.5f;
        static readonly float height = radius * 2;
        static readonly float width = Mathf.Sqrt(3) / 2 * height;
        static readonly float horizontalSpacing = width;
        static readonly float verticalSpacing = height * 0.75f;

        private readonly static Hex[] directions = new Hex[]
        {
            new Hex(1, 0),
            new Hex(1, -1),
            new Hex(0, -1),
            new Hex(-1, 0),
            new Hex(-1, 1),
            new Hex(0, 1)
        };
        #endregion

        #region Properties
        public int Q { get; }
        public int R { get; }
        public int S { get; }

        /// <summary>
        /// Converts this hex's cubic coordinates to Unity3D world space coordinates.
        /// </summary>
        /// <returns>Unity world space position of Hex</returns>
        public Vector3 WorldPosition
        {
            get
            {
                return new Vector3(
                    horizontalSpacing * (Q + (R / 2f)),
                    0,
                    verticalSpacing * R
                );
            }
        }
        #endregion
        
        #region Operators
        public static bool operator ==(Hex a, Hex b)
        {
            if (a is null || b is null)
            {
                return a is null && b is null;
            }

            return a.Q == b.Q && a.R == b.R;
        }
        public static bool operator !=(Hex a, Hex b)
        {
            return !(a == b);
        }
        public static Hex operator +(Hex a, Hex b)
        {
            return new Hex(a.Q + b.Q, a.R + b.R);
        }
        public static Hex operator -(Hex a, Hex b)
        {
            return new Hex(a.Q - b.Q, a.R - b.R);
        }
        public static Hex operator *(Hex a, int x)
        {
            return new Hex(a.Q * x, a.R * x);
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
                Hex h = (Hex)obj;
                return (Q == h.Q) && (R == h.R);
            }
        }
        public override int GetHashCode()
        {
            return Tuple.Create(Q, R).GetHashCode();
        }
        #endregion

        #region Methods
        public static int Length(Hex a)
        {
            return (Math.Abs(a.Q) + Math.Abs(a.R) + Math.Abs(a.S)) / 2;
        }
        
        public static int Distance(Hex a, Hex b)
        {
            return Length(a - b);
        }

        public static Hex Neighbour(Hex a, int direction)
        {
            direction %= directions.Length;

            return a + directions[direction];
        }

        public static Hex HexAtWorld(float x, float y)
        {

            float r = y / verticalSpacing;

            float q = (x / horizontalSpacing) - (r / 2f);

            return Round(q, r);
        }

        public static Hex Round(float q, float r)
        {
            float s = -q - r;

            int roundQ = Mathf.RoundToInt(q);
            int roundR = Mathf.RoundToInt(r);
            int roundS = Mathf.RoundToInt(s);

            float qDiff = Mathf.Abs(roundQ - q);
            float rDiff = Mathf.Abs(roundR - r);
            float sDiff = Mathf.Abs(roundS - s);

            if (qDiff > rDiff && qDiff > sDiff)
            {
                roundQ = -roundR - roundS;
            }
            else if (rDiff > sDiff)
            {
                roundR = -roundQ - roundS;
            }

            return new Hex(roundQ, roundR);
        }

        public static Hex FindHexAtMousePosition()
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (plane.Raycast(ray, out float enter))
            {
                Vector3 hitPosition = ray.GetPoint(enter);

                Hex worldToHex = HexAtWorld(hitPosition.x, hitPosition.z);

                return worldToHex;
            }

            return null;
        }
        #endregion
    }
}