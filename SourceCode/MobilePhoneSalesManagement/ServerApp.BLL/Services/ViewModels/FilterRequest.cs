using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.BLL.Services.ViewModels
{
    public class FilterRequest
    {
        public string Sort { get; set; } // "banchay", "giathap", "giacao"
        public List<string> Brands { get; set; }
        public List<string> Prices { get; set; }
        public List<string> ScreenSizes { get; set; }
        public List<string> InternalMemory { get; set; }
    }
}
