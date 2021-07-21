using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pikappDes
{

    public class NewMainMasterMasterMenuItem
    {
        public NewMainMasterMasterMenuItem()
        {
            TargetType = typeof(NewMainMasterMasterMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public string ImageSource { get; set; }
        public Type TargetType { get; set; }
    }
}