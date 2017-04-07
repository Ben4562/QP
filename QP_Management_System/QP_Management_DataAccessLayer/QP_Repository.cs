using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QP_Management_DataAccessLayer
{
    public class QP_Repository
    {
        public QP_ManagementDBContext Context { get; set; }

        public QP_Repository()
        {
            Context = new QP_ManagementDBContext();
        }

        public bool CheckLogin(User usr)
        {
            bool status=false;
            try
            {
                var UserDetails = (from u in Context.Users where u.UserName == usr.UserName select u).FirstOrDefault<User>();
                if(UserDetails!=null)
                {
                    if(UserDetails.UserPassword==usr.UserPassword)
                    {
                        status = true;
                    }
                    else
                    {
                        status = false;
                    }
                }
                else
                {
                    status = false;
                }
                
            }
            catch (Exception)
            {

                status = false;
            }
            return status;
        }
    }
}
