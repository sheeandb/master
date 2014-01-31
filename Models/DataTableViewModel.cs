using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MortgageCalulator.Models
{
    public class DataTableModel
    {
        public Dictionary<string, string> tableRow { get; set; }
        public List<Dictionary<string, string>> tableSet { get; set; }
    }
}