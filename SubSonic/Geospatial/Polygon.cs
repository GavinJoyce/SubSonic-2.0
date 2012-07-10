using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SubSonic.Geospatial {
    public class Polygon : IGeometry, IEnumerable {
        public Polygon() {
            this.Points = new List<Point>();
        }

        public Polygon(IEnumerable<Point> points)
            : this() {
            this.Points.AddRange(points);
        }

        public List<Point> Points { get; set; }

        public override string ToString() {
            return "POLYGON ((" + String.Join(",", this.Points.Select(p => p.ToString(true)).ToArray()) + "))";
        }

        public void Add(double x, double y) {
            this.Points.Add(new Point(x, y));
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator() {
            return this.Points.GetEnumerator();
        }

        #endregion
    }
}
