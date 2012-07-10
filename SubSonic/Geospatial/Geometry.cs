using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubSonic.Geospatial {
    public static class Geometry {
        public static string CastAsPoint(string xColumnName, string yColumnName) {
            return String.Format("geometry::STGeomFromText('POINT (' + CONVERT(varchar, [{0}]) + ' ' + CONVERT(varchar, [{1}]) + ')', 0)", xColumnName, yColumnName);
        }
    }
}
