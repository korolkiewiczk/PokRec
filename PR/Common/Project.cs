using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Project
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public Boards Boards { get; set; }
    }
}
