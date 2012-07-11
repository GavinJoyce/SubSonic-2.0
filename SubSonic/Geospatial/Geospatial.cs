using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubSonic.Geospatial {
    public static class SqlHelper {
        public static string CastAsPoint(GeospatialType type, string xColumnName, string yColumnName) {
            return String.Format("{0}::Parse('POINT (' + CONVERT(varchar, [{1}]) + ' ' + CONVERT(varchar, [{2}]) + ')')", GetSqlType(type), xColumnName, yColumnName);
        }

        public static string GetSqlType(GeospatialType type) {
            switch (type) {
                case GeospatialType.Geography:
                    return "geography";
                case GeospatialType.Geometry:
                    return "geometry";
                default:
                    throw new ArgumentException();
            }
        }
    }
}
