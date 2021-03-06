﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Northwind;
using System.Collections;
using SubSonic.Geospatial;

namespace SubSonic.Tests.SqlGenerators {
    [TestFixture]
    public class GeometryTests {
        private static readonly Polygon TestPolygon = new Polygon {
            { -6.255040168762207, 53.347477177067546 },
            { -6.2518858909606934, 53.347387511284893 },
            { -6.2523365020751953, 53.344735880753056 },
            { -6.255147457122803, 53.34527390608084 },
            { -6.255040168762207, 53.3474771770675476 }
        };

        [Test]
        public static void CreatePolygon_Test() {
            var polygonText = "POLYGON ((-6.255040168762207 53.347477177067546,-6.2518858909606934 53.347387511284893,-6.2523365020751953 53.344735880753056,-6.2551474571228027 53.345273906080841,-6.255040168762207 53.347477177067546))";
            Console.WriteLine(TestPolygon.ToString());
            Assert.AreEqual(polygonText, TestPolygon.ToString());
        }

        [Test]
        public static void Intersects_Test() {
            var expected = @"WHERE ([dbo].[Products].[CreatedBy]).STIntersects(geography::Parse(geometry::Parse(@CreatedBy0).MakeValid().STUnion(geometry::Parse(@CreatedBy0).MakeValid().STStartPoint()).ToString()).ToString()) = 1";
            var select = Select.AllColumnsFrom<Product>()
                .Where(Product.Columns.CreatedBy).IntersectsWith(TestPolygon.ToString());
            var gen = new ANSISqlGenerator(select);
            var sql = gen.GenerateWhere().Trim();
            Console.WriteLine(sql);
            Assert.AreEqual(expected, sql);
        }

        [Test]
        public static void IntersectsWithCast_Test() {
            var expected = @"WHERE (geometry::Parse('POINT (' + CONVERT(varchar, [CreatedBy]) + ' ' + CONVERT(varchar, [CreatedOn]) + ')')).STIntersects(geometry::Parse(geometry::Parse(@Parameter00).MakeValid().STUnion(geometry::Parse(@Parameter00).MakeValid().STStartPoint()).ToString()).ToString()) = 1";
            var select = Select.AllColumnsFrom<Product>()
                .Where(Geospatial.SqlHelper.CastAsPoint(GeospatialType.Geometry, Product.Columns.CreatedBy, Product.Columns.CreatedOn)).IntersectsWith(TestPolygon.ToString(), GeospatialType.Geometry);
            var gen = new ANSISqlGenerator(select);
            var sql = gen.GenerateWhere().Trim();
            Console.WriteLine(sql);
            Assert.AreEqual(expected, sql);
        }

        [Test]
        public static void IntersectsWithCast_CountTest() {
            var expected = @"WHERE (geometry::Parse('POINT (' + CONVERT(varchar, [CreatedBy]) + ' ' + CONVERT(varchar, [CreatedOn]) + ')')).STIntersects(geometry::Parse(geometry::Parse(@Parameter00).MakeValid().STUnion(geometry::Parse(@Parameter00).MakeValid().STStartPoint()).ToString()).ToString()) = 1";
            var select = Select.AllColumnsFrom<Product>()
                .Where(Geospatial.SqlHelper.CastAsPoint(GeospatialType.Geometry, Product.Columns.CreatedBy, Product.Columns.CreatedOn)).IntersectsWith(TestPolygon.ToString(), GeospatialType.Geometry);
            var gen = new ANSISqlGenerator(select);
            var sql = gen.GetCountSelect().Trim();
            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains(expected));
        }
    }
}
