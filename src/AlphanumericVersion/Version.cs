using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AlphanumericVersion
{
    [Serializable]
    public class Version : IComparable, IComparable<Version>, IEquatable<Version>
    {
        public string Major { get; internal set; }
        public string Minor { get; internal set; }
        public string Build { get; internal set; }
        public string Revision { get; internal set; }

        internal Version()
        {
            Major = null;
            Minor = null;
            Build = null;
            Revision = null;
        }
        public Version(string version)
        {
            var v = Parse(version);
            Major = v.Major;
            Minor = v.Minor;
            Build = v.Build;
            Revision = v.Revision;
        }

        private static readonly Regex _regex = new Regex( "^0*(?<match>[^0].*|0)$");
        public static int Compare(Version v1, Version v2)
        {
            if((object)v1 == null && (object)v2 == null) {return 0;}
            if((object)v1 == null) {return -1;}
            if((object)v2 == null) {return 1;}

            var a = v1.Major != null ? _regex.Match(v1.Major).Groups["match"].Value : "0";
            var b = v2.Major != null ? _regex.Match(v2.Major).Groups["match"].Value : "0";
            if (a != b) { return CompareValue(a, b); }

            a = v1.Minor != null ? _regex.Match(v1.Minor).Groups["match"].Value : "0";
            b = v2.Minor != null ? _regex.Match(v2.Minor).Groups["match"].Value : "0";
            if (a != b) { return CompareValue(a, b); }

            a = v1.Build != null ? _regex.Match(v1.Build).Groups["match"].Value : "0";
            b = v2.Build != null ? _regex.Match(v2.Build).Groups["match"].Value : "0";
            if (a != b) { return CompareValue(a, b); }

            a = v1.Revision != null ? _regex.Match(v1.Revision).Groups["match"].Value : "0";
            b = v2.Revision != null ? _regex.Match(v2.Revision).Groups["match"].Value : "0";
            return a != b ? CompareValue(a, b) : 0;
        }

        public static int CompareValue(string a, string b)
        {
            int aInt, bInt;

            if (a == null && b == null) { return 0; }
            if (a == null) { return -1; }
            if (b == null) { return 1; }
            
            return int.TryParse(a, out aInt) ? (int.TryParse(b, out bInt) ? aInt.CompareTo(bInt) : -1) 
                : int.TryParse(b, out bInt) ? 1 : String.Compare(a.ToLower(), b.ToLower(), StringComparison.Ordinal);
        }

        public static bool operator ==(Version v1, Version v2) { return Compare(v1, v2) == 0; }
        public static bool operator !=(Version v1, Version v2) { return Compare(v1,v2) != 0; }
        public static bool operator <=(Version v1, Version v2) { return Compare(v1,v2) != 1; }
        public static bool operator >=(Version v1, Version v2) { return Compare(v1,v2) != -1; }
        public static bool operator <(Version v1, Version v2) { return Compare(v1,v2) == -1; }
        public static bool operator >(Version v1, Version v2) { return Compare(v1,v2) == 1; }
        
        public int CompareTo(object versionObj)
        {
            var version = versionObj as Version;
            if (version == null) { throw new ArgumentException(); }
            return CompareTo(version);
        }

        public int CompareTo(Version value) { return Compare(this, value); }

        public override bool Equals(object obj) { return obj is Version && Equals((Version)obj); }
        public bool Equals(Version obj) { return Compare(this, obj) == 0; }

        public override int GetHashCode() { return ToString().GetHashCode(); }

        public override string ToString()
        {
            var str = new StringBuilder(Major);
            if (Minor != null)
            {
                str.AppendFormat(".{0}", Minor);
                if (Build != null)
                {
                    str.AppendFormat(".{0}", Build);
                    if (Revision != null) { str.AppendFormat(".{0}", Revision); }
                }
            }
            return str.ToString();
        }

        public string ToString(int fieldCount)
        {
            if (fieldCount < 1 || fieldCount > 4) { throw new FormatException(); }
            var str = new StringBuilder(Major);
            if (fieldCount > 1)
            {
                str.AppendFormat(".{0}", Minor ?? "0");
                if (fieldCount > 2)
                {
                    str.AppendFormat(".{0}", Build ?? "0");
                    if (fieldCount > 3) { str.AppendFormat(".{0}", Revision ?? "0"); }
                }
            }
            return str.ToString();
        }

        public static Version Parse(string input)
        {
            Version v;
            var success = TryParse(input, out v);
            if (!success) { throw new FormatException(); }
            return v;
        }

        public static bool TryParse(string input, out Version result)
        {
            result = new Version {Major = null, Minor = null, Build = null, Revision = null};
            if (string.IsNullOrEmpty(input)) { return false; }
            var versionInfo = input.Split('.');
            if (!versionInfo.All(v => Regex.IsMatch(v, @"^(\d+|[A-Za-z])$"))) { return false; }
            if (versionInfo.Length == 0 || versionInfo.Length == 1 ) { return false;}
            
            result.Major = versionInfo[0];
            if (versionInfo.Length > 1) { result.Minor = versionInfo[1]; }
            if (versionInfo.Length > 2) { result.Build = versionInfo[2]; }
            if (versionInfo.Length > 3) { result.Revision = versionInfo[3]; }

            return true;
        }
    }
}
