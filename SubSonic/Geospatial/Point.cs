using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubSonic.Geospatial {
    [Serializable]
    public struct Point : IGeometry {
        public Point(double x, double y)
            : this() {
            this.X = x;
            this.Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public override string ToString() {
            return this.ToString(false);
        }

        public string ToString(bool inner) {
            if (inner) {
                return this.X.ToString("G17") + " " + this.Y.ToString("G17");
            } else {
                return "POINT (" + this.ToString(true) + ")";
            }
        }
    }
}
