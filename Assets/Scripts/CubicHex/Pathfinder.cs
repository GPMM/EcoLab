using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubicHex
{
    public class Pathfinder
    {
        #region Constructors
        private Pathfinder () { }
        #endregion

        #region Methods
        public static List<Hex> FindHexPathByBreadth(Hex origin, Func<Hex, bool> hexIsGoal)
        {
            Queue<Hex> frontier = new Queue<Hex>();
            Dictionary<Hex, Hex> visited = new Dictionary<Hex, Hex>();
            Hex goal = null;

            frontier.Enqueue(origin);
            visited.Add(origin, null);

            while (frontier.Any() && goal == null)
            {
                Hex hex = frontier.Dequeue();

                for (int i = 0; i < 6; i++)
                {
                    // TODO: This is going to make it run out of memory. Neighbour returns a virtual representation of Hex, with
                    // will never be null. As such, this will run until if finds a suitable Hex. If there is no Hex meeting the
                    // Goal condition, it will only stop when the game crashes out of memory.
                    Hex neighbour = Hex.Neighbour(hex, i);

                    if (neighbour == null)
                    {
                        continue;
                    }

                    if (hexIsGoal(neighbour))
                    {
                        visited.Add(neighbour, hex);
                        goal = neighbour;
                        break;
                    }
                    else
                    {
                        frontier.Enqueue(neighbour);
                        visited.Add(neighbour, hex);
                    }
                }
            }

            if (goal == null)
            {
                // No goal was found.

                return null;
            }
            else
            {
                // Build path from visited matrix.

                Hex iterated = goal;
                List<Hex> path = new List<Hex>();

                while (iterated != origin)
                {
                    path.Add(iterated);
                    iterated = visited[iterated];
                }

                path.Add(origin);

                return path;
            }
        }
        #endregion
    }
}
