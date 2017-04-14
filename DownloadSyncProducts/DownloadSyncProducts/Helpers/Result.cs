using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

namespace DownloadSyncProducts.Helpers
{
    public class Result
    {
        public bool Success { get; set; }
        public ExpandoObject ReturnObj { get; set; } 
        public string ErrForUser { get; set; }
        public string ErrForLog { get; set; }

        public Result() {
            Success = false;
        }
    }
}
