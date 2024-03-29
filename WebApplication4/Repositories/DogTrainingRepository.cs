﻿using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Dogs.Data.Models;

namespace DogsServer.Repositories
{
    public class DogTrainingRepository : Repository<DogTraining>
    {
        protected new DbSet<DogTraining> DbSet;

        public DogTrainingRepository(DbContext dataContext)
             : base(dataContext)
        {
            DbSet = dataContext.Set<DogTraining>();
        }


        public new IQueryable<DogTraining> SearchFor(Expression<Func<DogTraining, bool>> predicate)
        {
            return DbSet.Include(x => x.Comments).ThenInclude(c => c.Author)
                .Include(x => x.Dog)
                .Include(x => x.Training)
                .Where(predicate);
        }

        public new IQueryable<DogTraining> GetAll()
        {
            //return DbSet;
            return DbSet.Include(x => x.Comments).ThenInclude(c => c.Author)
                .Include(x => x.Dog)
                .Include(x => x.Training);
        }

        public DogTraining GetByIds(int dogId, int trainingId)
        {
            var dogTraining = DbSet.Where(x => x.TrainingId == trainingId && x.DogId == dogId)
                .Include(x => x.Comments).ThenInclude(c => c.Author)
                .Include(x => x.Dog)
                .Include(x=>x.Training)
                .FirstOrDefault();
            return dogTraining;
        }

        public List<DogTraining> GetAllByDogId(int dogId)
        {
            var dogTrainings = DbSet.Where(x => x.DogId == dogId)
                .Include(x => x.Comments).ThenInclude(c => c.Author)
                .Include(x => x.Dog)
                .ThenInclude(x => x.Guide)
                .Include(x => x.Training)
                .ToList();
            return dogTrainings;
        }

        public List<DogTraining> GetAllByTrainingId(int trainingId)
        {
            var dogTrainings = DbSet.Where(x => x.TrainingId == trainingId)
                .Include(x => x.Comments).ThenInclude(c => c.Author)
                .Include(x => x.Dog)
                .ThenInclude(x => x.Guide)
                .Include(x => x.Training)
                .ToList();
            return dogTrainings;
        }
    }
}
