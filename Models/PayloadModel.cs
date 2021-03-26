using System;
using System.Collections.Generic;

namespace ElectronNetWithMVC.Models
{
    public class PayloadModel
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Body { get; set; }
        public List<string> Actions { get; set; }
       
    }
}
