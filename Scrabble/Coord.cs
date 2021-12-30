using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Scrabble
{
    public record Coord
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coord(int x, int y)
        {
            X = x;
            Y = y;
        }

        //https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/statements-expressions-operators/how-to-define-value-equality-for-a-type
        /*
        public override bool Equals(object obj) => this.Equals(obj as Coord);

        public bool Equals(Coord coord)
        {
            if (coord is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, coord))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (GetType() != coord.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (X == coord.X) && (Y == coord.Y);
        }

        public override int GetHashCode() => (X, Y).GetHashCode();

        public static bool operator ==(Coord lhs, Coord rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Coord lhs, Coord rhs) => !(lhs == rhs);*/

        /// <summary>
        /// Provides a string representation of the Coord object, used for debugging purposes
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return base.ToString() + ": (" + X + ", " + Y + ")";
        }

        /// <summary>
        /// Returns the 4 Coords that border this object
        /// </summary>
        /// <returns>A list of Coords</returns>
        public List<Coord> GetSurroundingCoords()
        {
            List<Coord> surroundingCoords = new();
            if (X > 0)
            {
                surroundingCoords.Add(new Coord(X - 1, Y));
            }
            if (X < 14)
            {
                surroundingCoords.Add(new Coord(X + 1, Y));
            }
            if (Y > 0)
            {
                surroundingCoords.Add(new Coord(X, Y - 1));
            }
            if (Y < 14)
            {
                surroundingCoords.Add(new Coord(X, Y + 1));
            }
            return surroundingCoords;
        }

        /// <summary>
        /// Generates a random Coord
        /// </summary>
        /// <returns>A random Coord</returns>
        public static Coord RandomCoord()
        {
            Random rnd = new();
            Coord coord = new(rnd.Next(0, 14), rnd.Next(0, 14));
            return coord;
        }

        public bool IsOutsideBounds()
        {
            return (X < 0 || X > 14 || Y < 0 || Y > 14);
        }
    }
}
