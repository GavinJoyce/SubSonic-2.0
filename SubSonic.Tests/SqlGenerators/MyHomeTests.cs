using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Northwind;

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
    }
}
