using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Orchard.DesignerTools.Services;
using Orchard.DisplayManagement;
using Orchard.DisplayManagement.Shapes;

namespace Orchard.Tests.Modules.DesignerTools.Services
{
    public class TestObject
    {
        public int SomeInteger { get; set; }
        public bool SomeBoolean { get; set; }
        public string SomeString { get; set; }
        public TestObject ChildObject { get; set; }
    }

    public class TestIShape : IShape
    {
        public ShapeMetadata Metadata { get; set; }

        public int SomeInteger { get; set; }
        public bool SomeBoolean { get; set; }
        public string SomeString { get; set; }

        public TestIShape ChildObject { get; set; }
    }

    public class TestShape : Shape
    {
        public int SomeInteger { get; set; }
        public bool SomeBoolean { get; set; }
        public string SomeString { get; set; }

        public TestShape ChildObject { get; set; }
    }

    [TestFixture]
    public class ObjectDumperTests
    {
        [Test]
        public void DumpNull() {
            var objectDumper = new ObjectDumper(1);
            var xElement = objectDumper.Dump(null, "Model");

            var stringBuilder = new StringBuilder();
            ObjectDumper.ConvertToJSon(xElement, stringBuilder);
            var json = stringBuilder.ToString();

            Assert.AreEqual("\"name\": \"Model\", \"value\": \"null\"", json);
        }

        [Test]
        public void DumpValueTypeInteger()
        {
            var objectDumper = new ObjectDumper(1);
            var xElement = objectDumper.Dump(1337, "Model");

            var stringBuilder = new StringBuilder();
            ObjectDumper.ConvertToJSon(xElement, stringBuilder);
            var json = stringBuilder.ToString();

            Assert.AreEqual("\"name\": \"Model\", \"value\": \"1337\"", json);
        }

        [Test]
        public void DumpValueTypeBoolean()
        {
            var objectDumper = new ObjectDumper(1);
            var xElement = objectDumper.Dump(true, "Model");

            var stringBuilder = new StringBuilder();
            ObjectDumper.ConvertToJSon(xElement, stringBuilder);
            var json = stringBuilder.ToString();

            Assert.AreEqual("\"name\": \"Model\", \"value\": \"True\"", json);
        }

        [Test]
        public void DumpString()
        {
            var objectDumper = new ObjectDumper(1);
            var xElement = objectDumper.Dump("Never gonna give you up", "Model");

            var stringBuilder = new StringBuilder();
            ObjectDumper.ConvertToJSon(xElement, stringBuilder);
            var json = stringBuilder.ToString();

            Assert.AreEqual("\"name\": \"Model\", \"value\": \"&quot;Never gonna give you up&quot;\"", json);
        }

        [Test]
        public void DumpEnumerable()
        {
            var enumerable = new[] { 1, 2, 3 }.AsEnumerable();

            var objectDumper = new ObjectDumper(1);
            var xElement = objectDumper.Dump(enumerable, "Model");

            Assert.Throws(typeof(NullReferenceException), () =>
            {
                var stringBuilder = new StringBuilder();
                ObjectDumper.ConvertToJSon(xElement, stringBuilder);
                var json = stringBuilder.ToString();
            });
        }

        [Test]
        public void DumpEnumerable_DepthTwo()
        {
            var enumerable = new[] { 1, 2, 3 }.AsEnumerable();

            var objectDumper = new ObjectDumper(2);
            var xElement = objectDumper.Dump(enumerable, "Model");

            var stringBuilder = new StringBuilder();
            ObjectDumper.ConvertToJSon(xElement, stringBuilder);
            var json = stringBuilder.ToString();

            Assert.AreEqual("\"name\": \"Model\", " +
                            "\"value\": \"Int32[]\", " +
                            "\"children\": [" +
                                "{ \"name\": \"[0]\", \"value\": \"1\" }, " +
                                "{\"name\": \"[1]\", \"value\": \"2\" }, " +
                                "{\"name\": \"[2]\", \"value\": \"3\" }]", json);
        }

        [Test]
        public void DumpDictionary() {
            var dictionary = new Dictionary<string, int> { { "One", 1 }, { "Two", 2 }, {"Three", 3} };

            var objectDumper = new ObjectDumper(1);
            var xElement = objectDumper.Dump(dictionary, "Model");

            Assert.Throws(typeof(NullReferenceException), () =>
            {
                var stringBuilder = new StringBuilder();
                ObjectDumper.ConvertToJSon(xElement, stringBuilder);
                var json = stringBuilder.ToString();
            });
        }

        [Test]
        public void DumpDictionary_DepthTwo()
        {
            var dictionary = new Dictionary<string, int> { { "One", 1 }, { "Two", 2 }, { "Three", 3 } };

            var objectDumper = new ObjectDumper(2);
            var xElement = objectDumper.Dump(dictionary, "Model");

            var stringBuilder = new StringBuilder();
            ObjectDumper.ConvertToJSon(xElement, stringBuilder);
            var json = stringBuilder.ToString();

            Assert.AreEqual("\"name\": \"Model\", " +
                            "\"value\": \"Dictionary&lt;String, Int32&gt;\", " +
                            "\"children\": [" +
                                "{ \"name\": \"[&quot;One&quot;]\", \"value\": \"1\" }, " +
                                "{\"name\": \"[&quot;Two&quot;]\", \"value\": \"2\" }, " +
                                "{\"name\": \"[&quot;Three&quot;]\", \"value\": \"3\" }]", json);
        }

        [Test]
        public void DumpObject_DepthOne()
        {
            var objectDumper = new ObjectDumper(1);
            var xElement = objectDumper.Dump(new TestObject
            {
                SomeInteger = 1337,
                SomeBoolean = true,
                SomeString = "Never gonna give you up"
            }, "Model");

            Assert.Throws(typeof (NullReferenceException), () => {
                var stringBuilder = new StringBuilder();
                ObjectDumper.ConvertToJSon(xElement, stringBuilder);
                var json = stringBuilder.ToString();
            });
        }

        [Test]
        public void DumpObject_DepthTwo()
        {
            var objectDumper = new ObjectDumper(2);
            var xElement = objectDumper.Dump(new TestObject
            {
                SomeInteger = 1337,
                SomeBoolean = true,
                SomeString = "Never gonna give you up"
            }, "Model");

            var stringBuilder = new StringBuilder();
            ObjectDumper.ConvertToJSon(xElement, stringBuilder);
            var json = stringBuilder.ToString();

            Assert.AreEqual("\"name\": \"Model\", " +
                            "\"value\": \"TestObject\", " +
                            "\"children\": [" +
                                "{ \"name\": \"SomeInteger\", \"value\": \"1337\" }, " +
                                "{\"name\": \"SomeBoolean\", \"value\": \"True\" }, " +
                                "{\"name\": \"SomeString\", \"value\": \"&quot;Never gonna give you up&quot;\" }, " +
                                "{\"name\": \"ChildObject\", \"value\": \"null\" }]", json);
        }

        [Test]
        public void DumpObjectAndChild_DepthTwo()
        {
            var objectDumper = new ObjectDumper(2);
            var xElement = objectDumper.Dump(new TestObject
            {
                SomeInteger = 1337,
                SomeBoolean = true,
                SomeString = "Never gonna give you up",
                ChildObject = new TestObject() {
                    SomeInteger = 58008,
                    SomeBoolean = false,
                    SomeString = "Never gonna let you down",                    
                }
            }, "Model");

            var stringBuilder = new StringBuilder();
            ObjectDumper.ConvertToJSon(xElement, stringBuilder);
            var json = stringBuilder.ToString();

            Assert.AreEqual("\"name\": \"Model\", " +
                            "\"value\": \"TestObject\", " +
                            "\"children\": [" +
                                "{ \"name\": \"SomeInteger\", \"value\": \"1337\" }, " +
                                "{\"name\": \"SomeBoolean\", \"value\": \"True\" }, " +
                                "{\"name\": \"SomeString\", \"value\": \"&quot;Never gonna give you up&quot;\" }]", json);
        }

        [Test]
        public void DumpObjectAndChild_DepthThree()
        {
            var objectDumper = new ObjectDumper(3);
            var xElement = objectDumper.Dump(new TestObject
            {
                SomeInteger = 1337,
                SomeBoolean = true,
                SomeString = "Never gonna give you up",
                ChildObject = new TestObject() {
                    SomeInteger = 58008,
                    SomeBoolean = false,
                    SomeString = "Never gonna let you down",                    
                }
            }, "Model");

            var stringBuilder = new StringBuilder();
            ObjectDumper.ConvertToJSon(xElement, stringBuilder);
            var json = stringBuilder.ToString();

            Assert.AreEqual("\"name\": \"Model\", " +
                            "\"value\": \"TestObject\", " +
                            "\"children\": [" +
                            "{ \"name\": \"SomeInteger\", \"value\": \"1337\" }, " +
                            "{\"name\": \"SomeBoolean\", \"value\": \"True\" }, " +
                            "{\"name\": \"SomeString\", \"value\": \"&quot;Never gonna give you up&quot;\" }, " +
                            "{" +
                                "\"name\": \"ChildObject\", " +
                                "\"value\": \"TestObject\", " +
                                "\"children\": [" +
                                    "{ \"name\": \"SomeInteger\", \"value\": \"58008\" }, " +
                                    "{\"name\": \"SomeBoolean\", \"value\": \"False\" }, " +
                                    "{\"name\": \"SomeString\", \"value\": \"&quot;Never gonna let you down&quot;\" }, " +
                                    "{\"name\": \"ChildObject\", \"value\": \"null\" }] }]", json);
        }
    }
}
