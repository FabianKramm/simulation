using Microsoft.Xna.Framework;
using Simulation.Util.Geometry;
using System;

namespace Simulation.Game
{
    class CollisionDetection
    {
        public static bool Intersect(ref Rect r1, ref Rect r2)
        {
            return r1.Intersects(r2);
        }

        public static bool Intersect(ref Rect r1, Vector2[] poly2)
        {
            return Intersect(poly2, new Vector2[] {
                new Vector2(r1.Left, r1.Top),
                new Vector2(r1.Right, r1.Top),
                new Vector2(r1.Right, r1.Bottom),
                new Vector2(r1.Left, r1.Bottom)
            });
        }

        // Based on http://www.dyn4j.org/2010/01/sat/#sat-nointer
        public static bool Intersect(Vector2[] poly1, Vector2[] poly2)
        {
            Vector2[] axes1 = getAxes(poly1);
            Vector2[] axes2 = getAxes(poly2);

            // loop over the axes1
            for (int i = 0; i < axes1.Length; i++)
            {
                // project both shapes onto the axis
                Vector2 p1 = projectOnAxis(poly1, ref axes1[i]);
                Vector2 p2 = projectOnAxis(poly2, ref axes1[i]);

                // do the projections overlap?
                if (!projectionOverlap(ref p1, ref p2))
                {
                    // then we can guarantee that the shapes do not overlap
                    return false;
                }
            }

            // loop over the axes1
            for (int i = 0; i < axes2.Length; i++)
            {
                // project both shapes onto the axis
                Vector2 p1 = projectOnAxis(poly1, ref axes2[i]);
                Vector2 p2 = projectOnAxis(poly2, ref axes2[i]);

                // do the projections overlap?
                if (!projectionOverlap(ref p1, ref p2))
                {
                    // then we can guarantee that the shapes do not overlap
                    return false;
                }
            }

            // if we get here then we know that every axis had overlap on it
            // so we can guarantee an intersection
            return true;
        }

        // Untested
        public static bool Intersects(Vector2 a, Vector2 b, Rect r)
        {
            var minX = Math.Min(a.X, b.X);
            var maxX = Math.Max(a.X, b.X);
            var minY = Math.Min(a.Y, b.Y);
            var maxY = Math.Max(a.Y, b.Y);

            if (r.Left > maxX || r.Right < minX)
            {
                return false;
            }

            if (r.Top > maxY || r.Bottom < minY)
            {
                return false;
            }

            if (r.Left < minX && maxX < r.Right)
            {
                return true;
            }

            if (r.Top < minY && maxY < r.Bottom)
            {
                return true;
            }

            var yAtRectLeft = a.Y - (r.Left - a.X) * ((a.Y - b.Y) / (b.X - a.X));
            var yAtRectRight = a.Y - (r.Right - a.X) * ((a.Y - b.Y) / (b.X - a.X));

            if (r.Top > yAtRectLeft && r.Top > yAtRectRight)
            {
                return false;
            }

            if (r.Bottom < yAtRectLeft && r.Bottom < yAtRectRight)
            {
                return false;
            }

            return true;
        }

        private static bool projectionOverlap(ref Vector2 p1, ref Vector2 p2)
        {
            // https://stackoverflow.com/questions/13513932/algorithm-to-detect-overlapping-periods
            return p1.X <= p2.Y && p2.X <= p1.Y;
        }

        private static Vector2 projectOnAxis(Vector2[] poly, ref Vector2 axis)
        {
            float min = Vector2.Dot(axis, poly[0]);
            float max = min;

            for (int i = 1; i < poly.Length; i++)
            {
                float p = Vector2.Dot(axis, poly[i]);

                if (p < min)
                {
                    min = p;
                }
                else if (p > max)
                {
                    max = p;
                }
            }

            return new Vector2(min, max);
        }

        private static Vector2[] getAxes(Vector2[] poly)
        {
            Vector2[] axes = new Vector2[poly.Length];

            for (int i = 0; i < poly.Length; i++)
            {
                // get the current vertex
                Vector2 p1 = poly[i];

                // get the next vertex
                Vector2 p2 = poly[i + 1 == poly.Length ? 0 : i + 1];

                // subtract the two to get the edge vector
                Vector2 edge = Vector2.Subtract(p1, p2);
                
                // the perp method is just (x, y) => (-y, x) or (y, -x)
                Vector2 normal = new Vector2(-edge.Y, edge.X);

                normal.Normalize();
                
                axes[i] = normal;
            }

            return axes;
        }
    }
}
