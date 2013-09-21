using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FeintSDK;

namespace UnitTestsProject
{
    [TestClass]
    public class FormUnitTest
    {
        [TestMethod]
        public void FromContentTest()
        {
            Dictionary<string, string> content = new Dictionary<string, string>();
            content.Add("testCharField1", "test");
            content.Add("testCharField2", "test");
            content.Add("testIntegerField1", "1024");
            content.Add("testIntegerField2", "2");
            Assert.IsTrue(Form.FromContent<TestForm>(content).IsValid);
            content = new Dictionary<string, string>();
            content.Add("testCharField1", "test");
            content.Add("testCharField2", "test");
            content.Add("testIntegerField1", "1024");
            content.Add("testIntegerField2", "8");
            Assert.IsFalse(Form.FromContent<TestForm>(content).IsValid);
            content = new Dictionary<string, string>();
            content.Add("testCharField1", "test");
            content.Add("testCharField2", "test1234123");
            content.Add("testIntegerField1", "1024");
            content.Add("testIntegerField2", "2");
            Assert.IsFalse(Form.FromContent<TestForm>(content).IsValid);
            content = new Dictionary<string, string>();
            content.Add("testCharField1", "test");
            content.Add("testCharField2", "test");
            content.Add("testIntegerField1", "1024");
            content.Add("testIntegerField2", "2");
            content.Add("testIntegerField3", "2");
            Assert.IsTrue(Form.FromContent<TestForm>(content).IsValid);
            content = new Dictionary<string, string>();
            content.Add("testCharField1", "test");
            content.Add("testCharField2", "test");
            content.Add("testIntegerField1", "1024");
            content.Add("testIntegerField2", "2");
            content.Add("testIntegerField3", "6");
            Assert.IsFalse(Form.FromContent<TestForm>(content).IsValid);
            content = new Dictionary<string, string>();
            content.Add("testCharField1", "test");
            content.Add("testCharField2", "test");
            content.Add("testIntegerField1", "1024");
            content.Add("testIntegerField2", "2");
            content.Add("testDateTimeField", "sdasdasdadasdasd");
            Assert.IsFalse(Form.FromContent<TestForm>(content).IsValid);
            content = new Dictionary<string, string>();
            content.Add("testCharField1", "test");
            content.Add("testCharField2", "test");
            content.Add("testIntegerField1", "1024");
            content.Add("testIntegerField2", "2");
            content.Add("testDateTimeField", "12/04/2013");
            Assert.IsTrue(Form.FromContent<TestForm>(content).IsValid);
        }
    }
    public class TestForm : Form
    {
        [CharField]
        public String testCharField1 { get; set; }
        [CharField(MaxLenght=5,MinLenght=2)]
        public String testCharField2 { get; set; }
        [CharField(MaxLenght = 5, MinLenght = 2,Requierd=false)]
        public String testCharField3 { get; set; }
        [IntegerField]
        public Int32 testIntegerField1 { get; set; }
        [IntegerField(MaxValue=5,MinValue=2)]
        public Int32 testIntegerField2 { get; set; }
        [IntegerField(MaxValue = 5, MinValue = 2,Requierd=false)]
        public Int32? testIntegerField3 { get; set; }
        [DateTimeField(DataTimeFormatString="dd/MM/yyyy",Requierd=false)]
        public DateTime testDateTimeField { get; set; }

    }
}
