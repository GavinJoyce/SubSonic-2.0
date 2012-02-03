using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Northwind;
using System.Text.RegularExpressions;

namespace SubSonic.Tests.SqlGenerators {
    [TestFixture]
    public class MyHomeTests {

        [Test]
        public void CanSearchFullTextIndex() {
            var select = Select.AllColumnsFrom<Product>();

            select.And(Product.Columns.ProductName).FullTextContainsString("apple");
            ANSISqlGenerator gen = new ANSISqlGenerator(select);

            string where = gen.GenerateWhere();

            Assert.AreEqual(" WHERE CONTAINS([dbo].[Products].[ProductName], @ProductName0)\r\n", where);
        }

        [Test]
        public void No_Duplicate_Where_Clause_When_Paging() {
            var select = Select.AllColumnsFrom<Product>()
                .Where(Product.Columns.ProductName)
                .IsEqualTo("apple")
                .Paged(0, 10);
            var gen = new ANSISqlGenerator(select);
            var sql = gen.BuildPagedSelectStatement();
            Assert.AreEqual(1, Regex.Matches(sql, Regex.Escape("[dbo].[Products].[ProductName] = ")).Count);
        }

        [Test]
        public void Table_Hints_Single() {
            var select = Select.AllColumnsFrom<Product>()
                .WithTableHint("READPAST")
                .Where(Product.Columns.ProductName)
                .IsEqualTo("apple");
            var gen = new ANSISqlGenerator(select);
            var sql = gen.BuildSelectStatement();
            Assert.True(sql.Contains("[dbo].[Products] WITH (READPAST)"));
        }

        [Test]
        public void Table_Hints_Multiple() {
            var select = Select.AllColumnsFrom<Product>()
                .WithTableHint("READPAST")
                .WithTableHint("NOLOCK")
                .Where(Product.Columns.ProductName)
                .IsEqualTo("apple");
            var gen = new ANSISqlGenerator(select);
            var sql = gen.BuildSelectStatement();
            Console.WriteLine(sql);
            Assert.True(sql.Contains("[dbo].[Products] WITH (READPAST,NOLOCK)"));
        }

        [Test]
        public void Table_Hints_SimpleJoin() {

            var select = Select.AllColumnsFrom<Product>()
                .WithTableHint("READPAST")
                .InnerJoin<Category>()
                .WithTableHint("READPAST")
                .InnerJoin<Category>()
                .WithTableHint("NOLOCK")
                .Where(Product.Columns.ProductName)
                .IsEqualTo("apple");
            var gen = new ANSISqlGenerator(select);
            var sql = gen.BuildSelectStatement();
            Console.WriteLine(sql);
            Assert.True(sql.Contains("[dbo].[Products] WITH (READPAST)"));
            Assert.True(sql.Contains("[dbo].[Categories] WITH (READPAST)"));
            Assert.True(sql.Contains("[dbo].[Categories] WITH (NOLOCK)"));
        }
    }
}
