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

        public string CheckLogin(User usr)
        {
            string status = null;
            try
            {
                var UserDetails = (from u in Context.Users where u.UserName == usr.UserName select u).FirstOrDefault<User>();
                if(UserDetails!=null)
                {
                    if(UserDetails.UserPassword==usr.UserPassword)
                    {
                        if(UserDetails.RoleId==1)
                        {
                            status = "author";
                        }
                        else if(UserDetails.RoleId==2)
                        {
                            status = "reviewer";
                        }
                        else if(UserDetails.RoleId==3)
                        {
                            status = "quality anchor"; 
                        }
                    }
                    else
                    {
                        status = "wrong";
                    }
                }
                else
                {
                    status = "not exists";
                }
                
            }
            catch (Exception)
            {

                status = null;
            }
            return status;
        }

        public bool AddDocument(QPMasterPool qpObj)
        {
            bool status = false;
            try
            {
                Context.QPMasterPools.Add(qpObj);
                Context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
                
            }
            return status;
        }
    }
}
