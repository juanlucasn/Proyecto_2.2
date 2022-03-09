using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Proyecto_2.Modelos
{
    public class Conversion
    {
        public string currency_base { get; set; }
        public string currency_quote { get; set; }
        public string ratio { get; set; }
        public string rate { get; set; }
        public string inv_rate { get; set; }
        public string creation_date { get; set; }
        public string valid_until { get; set; }
    }
}
