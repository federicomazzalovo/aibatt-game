using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFlyff.Session
{
    public static class ConnectedUser
    {
        public static string Username { get; set; }
        public static string WSSessionId { get; internal set; }
    }
}
