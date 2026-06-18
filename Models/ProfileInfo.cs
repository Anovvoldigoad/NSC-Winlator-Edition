using System.Collections.Generic;
using System;
namespace NSC.Winlator.Models
{
    public class ProfileInfo
    {
        public string Name { get; set; } = string.Empty;
        public List<string> EnabledMods { get; set; } = new List<string>();
    }
}
