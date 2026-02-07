using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.CornerSpeed
{
    public class CornerSpeedsStorage : Dictionary<string, ObservableCollection<Corner>>
    {
        public CornerSpeedsStorage() : base() {

        }
    };
}
