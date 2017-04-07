using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using QP_Management_DataAccessLayer;

namespace QP_Management_System.Repository
{
    public class QPMapper<Source,Destination> where Source:class where Destination:class
    {
        public QPMapper()
        {
            //Entity-Model
            Mapper.CreateMap<User, Models.Users>();

            //Model-Entity
            Mapper.CreateMap<Models.Users, User>();
        }

        public Destination Translate(Source obj)
        {
            return Mapper.Map<Source, Destination>(obj);
        }
    }
}