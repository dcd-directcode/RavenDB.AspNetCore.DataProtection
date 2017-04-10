using System.Collections.Generic;

namespace RavenDB.AspNetCore.DataProtection
{
    public class DataProtection
    {
        public DataProtection() {
            this.Elements = new List<string>();
        }
        
        public string Id { get; set; }
        public List<string> Elements { get; set; }
    }
}
