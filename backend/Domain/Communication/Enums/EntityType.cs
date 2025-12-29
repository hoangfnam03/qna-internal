using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Communication.Enums
{
    public enum EntityType : byte
    {
        Unknown = 0,
        Post = 1,
        Comment = 2,
        Announcement = 3,
        Report = 4,
        User = 5
    }
}
