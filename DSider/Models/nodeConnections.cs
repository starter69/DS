using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DSider.Models
{
    public class nodeConnections
    {
        public string node { get; set; }//it means from component with this id to current component. distinguish source node
        public string source { get; set; }//from output node
        public string destination { get; set; }//to input node
        public string nodeType { get; set; }//INPUT or OUTPUT
    }
}
