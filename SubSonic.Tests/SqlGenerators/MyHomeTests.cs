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

        [Test]
        public void Table_Hints_SimpleJoin_Paged() {

            var select = Select.AllColumnsFrom<Product>()
                .WithTableHint("READPAST")
                .InnerJoin<Category>()
                .WithTableHint("READPAST")
                .InnerJoin<Category>()
                .WithTableHint("NOLOCK")
                .Where(Product.Columns.ProductName)
                .IsEqualTo("apple")
                .Paged(1, 10);
            var gen = new ANSISqlGenerator(select);
            var sql = gen.BuildSelectStatement();
            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains("[dbo].[Products] WITH (READPAST)"));
            Assert.IsTrue(sql.Contains("[dbo].[Categories] WITH (READPAST)"));
            Assert.IsTrue(sql.Contains("[dbo].[Categories] WITH (NOLOCK)"));
        }

        [Test]
        public void Table_IndexHints_SimpleJoin_Paged_NoIndexHintsOn2ndQuery() {

            var select = Select.AllColumnsFrom<Product>()
                .WithTableHint("INDEX (MyIndex)")
                .InnerJoin<Category>()
                .WithTableHint("READPAST")
                .InnerJoin<Category>()
                .WithTableHint("NOLOCK")
                .Where(Product.Columns.ProductName)
                .IsEqualTo("apple")
                .Paged(1, 10);
            var gen = new ANSISqlGenerator(select);
            var sql = gen.BuildSelectStatement();
            Console.WriteLine(sql);
            Assert.AreEqual(1, Regex.Matches(sql, Regex.Escape(@"INDEX (MyIndex)")).Count);
            Assert.IsTrue(sql.Contains("[dbo].[Products] WITH (INDEX (MyIndex))"));
            Assert.IsTrue(sql.Contains("[dbo].[Categories] WITH (READPAST)"));
            Assert.IsTrue(sql.Contains("[dbo].[Categories] WITH (NOLOCK)"));
        }

        [Test]
        public void SqlExpression_WhereConstraint() {
            var select = Select.AllColumnsFrom<Product>()
                .InnerJoin<Category>()
                .WhereSql("1 = 1");
            var gen = new ANSISqlGenerator(select);
            var sql = gen.BuildSelectStatement();
            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains("WHERE 1 = 1"));
        }

        [Test]
        public void SqlExpression_AndConstraint() {
            var select = Select.AllColumnsFrom<Product>()
                .InnerJoin<Category>()
                .WhereSql("1 = 1")
                .AndSql("2 = 2");
            var gen = new ANSISqlGenerator(select);
            var sql = gen.BuildSelectStatement();
            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains("WHERE 1 = 1"));
            Assert.IsTrue(sql.Contains("AND 2 = 2"));
        }

        [Test]
        public void SqlExpression_OrConstraint() {
            var select = Select.AllColumnsFrom<Product>()
                .InnerJoin<Category>()
                .WhereSql("1 = 1")
                .OrSql("2 = 2");
            var gen = new ANSISqlGenerator(select);
            var sql = gen.BuildSelectStatement();
            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains("WHERE 1 = 1"));
            Assert.IsTrue(sql.Contains("OR 2 = 2"));
        }

        [Test]
        public void SqlExpression_MultiConstraint() {
            var select = Select.AllColumnsFrom<Product>()
                .InnerJoin<Category>()
                .WhereSql("1 = 1")
                .AndSqlExpression("2 = 2")
                .OrSql("3 = 3")
                .CloseExpression();
            var gen = new ANSISqlGenerator(select);
            var sql = gen.BuildSelectStatement();
            Console.WriteLine(sql);
            Assert.IsTrue(sql.Contains("WHERE 1 = 1"));
            Assert.IsTrue(sql.Contains("AND (2 = 2"));
            Assert.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(sql, @"OR\ 3 = 3\s*\)"));
        }
    }
}
