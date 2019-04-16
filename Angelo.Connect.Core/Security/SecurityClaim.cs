using Angelo.Identity.Models;
using Angelo;
using System.Security.Claims;
using System;

namespace Angelo.Connect.Security
{
    public class SecurityClaim: Claim, IEquatable<string>
    {
        private string _claimType;
        private string _claimValue;
        private string _claimTitle;

        public SecurityClaim(string type, string value) : this(type, value, type)
        {

        }

        public SecurityClaim(string type, string value, string title): base(type, value)
        {
            _claimType = type;
            _claimValue = value;
            _claimTitle = title;
        }
        //public PoolType PoolType { get; set; }
        public new string Type { get { return _claimType; } }
        public new string Value { get { return _claimValue; } }
        public string Title { get { return _claimTitle; } }

        public bool Equals(string other)
        {
            return _claimType == other;
        }

        public override string ToString()
        {
            return _claimType;
        }
    }
}