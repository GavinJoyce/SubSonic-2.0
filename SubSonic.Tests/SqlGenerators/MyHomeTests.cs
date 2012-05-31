using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using Northwind;
using System.Text.RegularExpressions;

namespace SubSonic.Tests.SqlGenerators {
    [TestFixture]
    public class MyHomeTests {

        private static string NormaliseWhitespace(string s) {
            s = s.Replace("\t", " ").Replace("\r", " ").Replace("\n", " ");
            while (s.Contains("  ")) {
                s = s.Replace("  ", " ");
            }
            return s;
        }

        [Test]
        public void TasksIssue() {
            var select = Select.AllColumnsFrom<Product>();
            select.And(Product.Columns.CategoryID).IsEqualTo(55);
            var referencedProductIDs = new Select(OrderDetail.Columns.ProductID)
                .From<OrderDetail>()
                .Where(OrderDetail.Columns.ProductID).IsNotNull();
            select.And(Product.Columns.ProductID).NotIn(referencedProductIDs);
            select.Paged(1, 10);
            var sql = select.BuildSqlStatement();
            sql = NormaliseWhitespace(sql);
            Assert.IsTrue(!sql.Contains("FROM [dbo].[Order Details] AND [dbo].[Order Details].[ProductID] IS NOT NULL"), "BUG FOUND");
        }

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
            Assert.IsTrue(sql.Contains("[dbo].[Products] WITH (READPAST)"));
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
            Assert.IsTrue(sql.Contains("[dbo].[Products] WITH (READPAST,NOLOCK)"));
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
            Assert.IsTrue(sql.Contains("[dbo].[Products] WITH (READPAST)"));
            Assert.IsTrue(sql.Contains("[dbo].[Categories] WITH (READPAST)"));
            Assert.IsTrue(sql.Contains("[dbo].[Categories] WITH (NOLOCK)"));
        }
    }
}
