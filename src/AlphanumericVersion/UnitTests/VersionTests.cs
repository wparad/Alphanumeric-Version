using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AlphanumericVersion.UnitTests
{
    [TestFixture]
    public class VersionTests
    {
        public IEnumerable<TestCaseData> VersionTestCaseData 
        {
            get
            {
                yield return new TestCaseData("1.0", new Version { Major = "1", Minor = "0", Build = null, Revision = null });
                yield return new TestCaseData("2.0", new Version {Major = "2", Minor = "0", Build = null, Revision = null});
                yield return new TestCaseData("1.0.0", new Version {Major = "1", Minor = "0", Build = "0", Revision = null});
                yield return new TestCaseData("1.00.0000", new Version { Major = "1", Minor = "00", Build = "0000", Revision = null});
                yield return new TestCaseData("1.00.0200", new Version { Major = "1", Minor = "00", Build = "0200", Revision = null});
                yield return new TestCaseData("1.0.0.0", new Version {Major = "1", Minor = "0", Build = "0", Revision = "0"});
                yield return new TestCaseData("1.1.1.1", new Version {Major = "1", Minor = "1", Build = "1", Revision = "1"});
                yield return new TestCaseData("1.A.0", new Version {Major = "1", Minor = "A", Build = "0", Revision = null});
                yield return new TestCaseData("1.a", new Version {Major = "1", Minor = "a", Build = null, Revision = null});
                yield return new TestCaseData("1.0.a", new Version {Major = "1", Minor = "0", Build = "a", Revision = null});
                yield return new TestCaseData("15.0.a.199", new Version {Major = "15", Minor = "0", Build = "a", Revision = "199"});
            }
        }

        [Test(Description = "Validate that parsing a string works."), TestCaseSource("VersionTestCaseData")]
        public void ParseTest(string versionString, Version expectedVersion)
        {
            Version outVersion;
            Version.TryParse(versionString, out outVersion);
            Assert.AreEqual(expectedVersion.Major, outVersion.Major);
            Assert.AreEqual(expectedVersion.Minor, outVersion.Minor);
            Assert.AreEqual(expectedVersion.Build, outVersion.Build);
            Assert.AreEqual(expectedVersion.Revision, outVersion.Revision);
        }

        [Test(Description = "Validate that the constructor works."), TestCaseSource("VersionTestCaseData")]
        public void ConstructorTest(string versionString, Version expectedVersion)
        {
            var outVersion = new Version(versionString);
            Assert.AreEqual(expectedVersion.Major, outVersion.Major);
            Assert.AreEqual(expectedVersion.Minor, outVersion.Minor);
            Assert.AreEqual(expectedVersion.Build, outVersion.Build);
            Assert.AreEqual(expectedVersion.Revision, outVersion.Revision);
        }

        [Test(Description = "Validate that the operator works."), TestCaseSource("VersionTestCaseData")]
        public void OperatorTest(string versionString, Version expectedVersion)
        {
            Assert.AreEqual(expectedVersion, new Version(versionString));
        }

        public IEnumerable<TestCaseData> CompareTestCaseData 
        {
            get
            {
                yield return new TestCaseData(new Version("1.0"), new Version("1.0"), 0);
                yield return new TestCaseData(new Version("1.0"), new Version("1.1"), -1);
                yield return new TestCaseData(new Version("1.00.0000"), new Version("1.0.39"), -1);
                yield return new TestCaseData(new Version("1.00.0000"), new Version("1.01"), -1);
                yield return new TestCaseData(new Version("1.010"), new Version("1.020"), -1);
                yield return new TestCaseData(new Version("1.2"), new Version("1.1"), 1);
                yield return new TestCaseData(new Version("A.2"), new Version("1.B"), 1);
                yield return new TestCaseData(new Version("1.A"), new Version("1.a"), 0);
                yield return new TestCaseData(new Version("1.0"), new Version("1.0.0"), 0);
                yield return new TestCaseData(new Version("1.0.1"), new Version("1.1"), -1);
            }
        }

        [Test(Description = "Validate that comparison works."), TestCaseSource("CompareTestCaseData")]
        public void CompareTest(Version a, Version b, int expectedCompareResult)
        {
            Assert.AreEqual(expectedCompareResult, Version.Compare(a, b));
            if (expectedCompareResult == 0) { Assert.True(a == b, "Equals test failed"); }
            if (expectedCompareResult != 0) { Assert.True(a != b, "Not Equals test failed"); }
            if (expectedCompareResult == 1) { Assert.True(a > b, "Greater than test failed."); }
            if (expectedCompareResult != 1) { Assert.True(a <= b, "Less than or equal test failed."); }
            if (expectedCompareResult == -1) { Assert.True(a < b, "Less than test failed."); }
            if (expectedCompareResult != -1) { Assert.True(a >= b, "Greater than or equal test failed."); }
        }

        public IEnumerable<TestCaseData> ToStringTestCaseData
        {
            get
            {
                var version = new Version("1.2.3.4");
                yield return new TestCaseData("1", version, 1);
                yield return new TestCaseData("1.2", version, 2);
                yield return new TestCaseData("1.2.3", version, 3);
                yield return new TestCaseData("1.2.3.4", version, 4);
            }
        }

        [Test(Description = "Validate that ToString works."), TestCaseSource("ToStringTestCaseData")]
        public void ToStringTest(string expectedResult, Version version, int places)
        {
            Assert.AreEqual(expectedResult, version.ToString(places));
            Assert.AreEqual(version.ToString(), version.ToString(4));
        }
    }
}

