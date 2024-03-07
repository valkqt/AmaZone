using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AmaZone.Models
{
    public class OrderState
    {
        public int id { get; set; }
        public string idString { get; set; }
        public string state { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public DateTime timestamp { get; set; }

        public List<SelectListItem> stateList = new List<SelectListItem>();

    }
}