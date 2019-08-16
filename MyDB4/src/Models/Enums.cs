using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDB4.Models
{
    public enum StateEnum
    {
        Created  = 10,
        InQueue  = 20,
        Started  = 30,
        Paused   = 35,
        Finished = 40,
        Reviewed = 50,
        Closed   = 60,
        Archived = 70
    }

    public enum TypeEnum
    {
        NA        = 0,
        ForMe     = 101,
        ForParent = 102,
        ForFamily = 103
    }
}
