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
            Mapper.CreateMap<QPVersion, Models.QPVersion>();
            Mapper.CreateMap<QPMasterPool, Models.QPMasterPool>();

            //Model-Entity
            Mapper.CreateMap<Models.Users, User>();
            Mapper.CreateMap<Models.QPVersion, QPVersion>();
            Mapper.CreateMap<Models.QPMasterPool,QPMasterPool>();
        }

        public Destination Translate(Source obj)
        {
            return Mapper.Map<Source, Destination>(obj);
        }
    }
}