using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SubSonic.Geospatial {
    [Serializable]
    public class Polygon : List<Point>, IGeometry {
        public Polygon() {
        }

        public Polygon(IEnumerable<Point> points) : base(points) { }

        public override string ToString() {
            return "POLYGON ((" + String.Join(",", this.Select(p => p.ToString(true)).ToArray()) + "))";
        }

        public void Add(double x, double y) {
            this.Add(new Point(x, y));
        }
    }
}
