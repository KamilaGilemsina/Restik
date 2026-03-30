using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restik.DB
{
    public static class CurrentUser
    {
        public static int ID { get; set; }
        public static string UserName { get; set; }
        public static string FullName { get; set; }
        public static int RoleID { get; set; }
        public static bool IsAuthenticated { get; set; } = false;

        public static void Clear()
        {
            ID = 0;
            UserName = null;
            FullName = null;
            RoleID = 0;
            IsAuthenticated = false;
        }
    }
}
