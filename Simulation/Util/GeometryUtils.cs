using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Util
{
    public class GeometryUtils
    {
        public static Vector2 Rotate(float angle, ref Vector2 pivot, ref Vector2 point)
        {
            Vector2 rotatedPoint = point;

            rotatedPoint.X -= pivot.X;
            rotatedPoint.Y -= pivot.Y;

            // Rotate about the origin.
            Matrix mat = Matrix.CreateRotationZ(angle);
            rotatedPoint = Vector2.Transform(rotatedPoint, mat);

            rotatedPoint.X += pivot.X;
            rotatedPoint.Y += pivot.Y;

            return rotatedPoint;
        }
    }
}
