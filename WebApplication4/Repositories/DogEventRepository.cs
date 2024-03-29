﻿using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Dogs.Data.Models;

namespace DogsServer.Repositories
{
    public class DogEventRepository : Repository<DogEvent>
    {
        protected new DbSet<DogEvent> DbSet;

        public DogEventRepository(DbContext dataContext)
             : base(dataContext)
        {
            DbSet = dataContext.Set<DogEvent>();
        }


        public new IQueryable<DogEvent> SearchFor(Expression<Func<DogEvent, bool>> predicate)
        {
            return DbSet.Include(x => x.Dog).ThenInclude(x=>x.Guide).Include(x => x.Event).Where(predicate);
        }

        public new IQueryable<DogEvent> GetAll()
        {
            //return DbSet;
            return DbSet.Include(x => x.Dog).ThenInclude(x=> x.Guide).Include(x => x.Event);
        }

        public DogEvent GetByIds(int dogId, int eventId)
        {
            var DogEvent = DbSet.Where(x => x.EventId == eventId && x.DogId == dogId).Include(x => x.Dog).ThenInclude(x => x.Guide)
                .Include(x => x.Event).FirstOrDefault();
            return DogEvent;
        }

        public List<DogEvent> GetAllByDogId(int dogId)
        {
            var dogEvent = DbSet.Where(x => x.DogId == dogId)
                                      .Include(x => x.Dog).ThenInclude(x => x.Guide)
                                      .Include(x => x.Event)
                                      .ToList();
            return dogEvent;
        }

        public List<DogEvent> GetAllByEventId(int eventId)
        {
            var dogEvent = DbSet.Where(x => x.EventId == eventId)
                                      .Include(x => x.Dog).ThenInclude(x => x.Guide)
                                      .Include(x => x.Event)
                                      .ToList();
            return dogEvent;
        }
    }
}
