using EcoClean;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CubicHex
{
    public class HexMap : MonoBehaviour
    {
        #region Local variables
        private static Hex[,] hexes;
        #endregion

        #region Serialized variables
        /// <summary>
        /// Prefab for the hexagon object to be tiled and made into a map
        /// </summary>
        public GameObject HexPrefab;

        /// <summary>
        /// Prefab for the background object
        /// </summary>
        public GameObject PetriDish;

        /// <summary>
        /// Total map radius
        /// </summary>
        public int Radius = 10;
        #endregion

        #region Properties
        /// <summary>
        /// Spatially-based storage of all hexes.
        /// </summary>
        public Hex[,] Hexes
        {
            get
            {
                if (hexes == null)
                {
                    ErrorHandler.LogError("The property Hexes is null! You must the method Generate() before accessing it.");
                }

                return hexes;
            }
        }

        /// <summary>
        /// Easily iteratable list of all hexes.
        /// </summary>
        public List<Hex> AllHexes
        {
            get;
        } = new List<Hex>();
        #endregion

        #region Methods
        private void Start()
        {
            Generate();
        }

        /// <summary>
        /// Creates all Hexes in the map and initiates the Hexes property.
        /// 
        /// TODO: Make this method abstract and allow other classes that implement HexMap to change this behavior.
        /// </summary>
        private void Generate()
        {
            int diameter = Radius * 2 + 1;

            // Instancing the hexes array in its proper size
            hexes = new Hex[diameter, diameter];

            for (int row = 0; row < diameter; row++)
            {
                // Petri dish declaration
                GameObject pd = null;

                // Making the map hexagon-shaped
                int minColumn = Mathf.Clamp(Radius - row, 0, Radius);
                int maxColumn = Mathf.Clamp(diameter, 0, diameter + Radius - row);

                for (int column = minColumn; column < maxColumn; column++)
                {
                    // Instantiate the hexagon object
                    GameObject go = Instantiate(
                        HexPrefab,
                        Vector3.zero,
                        Quaternion.identity,
                        transform);

                    // Creating the abstract representation of a hexagon in cubic coordinates and storing it in the Hex arrays
                    PetriDishSlot hex = new PetriDishSlot(column, row, go);
                    hexes[column, row] = hex;
                    AllHexes.Add(hex);

                    // Makes this object pretty in Unity's inspector
                    go.name = "(" + hex.Q + ", " + hex.R + ", " + hex.S + ")";
                    go.transform.position = hex.WorldPosition;

                    // Instantiate the petri dish model
                    if (row == column && row == Radius)
                    {
                        pd = Instantiate(
                           PetriDish,
                           hex.WorldPosition,
                           Quaternion.identity);

                        pd.transform.localScale = new Vector3(
                            diameter,
                            diameter,
                            diameter);
                    }
                }

                if (!(pd is null))
                {
                    Camera main = Camera.main;

                    main.transform.position = new Vector3(
                        pd.transform.position.x,
                        Radius * 1.3f,
                        pd.transform.position.z + Radius * 1.3f);

                    main.transform.LookAt(pd.transform);
                }
            }
        }

        /// <summary>
        /// Finds a Hex object in this Map according to the specified coordinates
        /// </summary>
        /// <param name="hex">The Hex object for specifiying coordinates</param>
        /// <returns>The found Hex, or null if coordinates don't contain a Hex</returns>
        public Hex GetHexAt(Hex hex)
        {
            return GetHexAt(hex.Q, hex.R);
        }

        /// <summary>
        /// Finds a Hex object in this Map according to the specified coordinates
        /// </summary>
        /// <param name="q">Q coordinate of the target Hex</param>
        /// <param name="r">R coordinate of the target Hex</param>
        /// <returns>The found Hex, or null if coordinates don't contain a Hex</returns>
        public Hex GetHexAt(int q, int r)
        {
            try
            {
                // Will either return a Hex set in the map, or null if the coordinates don't contain a Hex.
                // It will throw an IndexOutOfRangeException if the specified coordinates are outside of the array's range.
                return Hexes[q, r];
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        /// <summary>
        /// Finds all hexes within a certain range of a center Hex coordinage
        /// </summary>
        /// <param name="origin">Center Hex</param>
        /// <param name="range">Maximum range to find hexes</param>
        /// <returns>List of Hexes found</returns>
        public IEnumerable<Hex> GetHexesWithin(Hex origin, int range)
        {
            List<Hex> results = new List<Hex>();

            for (int r = -range; r <= range; r++)
            {
                int a = Mathf.Max(-range, -range - r);
                int b = Mathf.Min( range,  range - r);

                for (int q = a; q <= b; q++)
                {
                    Hex hex = GetHexAt(origin.Q + q, origin.R + r);

                    if (hex != null)
                    {
                        results.Add(hex);
                    }
                }
            }

            return results;
        }
        #endregion
    }
}