using System;
//using System.Data.Entity;
//using System.Linq;
//using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using DogsServer.Models;
using System.Linq;
using System.Linq.Expressions;

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
            return DbSet.Include(x => x.Comments).Include(x => x.Dog).Include(x => x.Training).Where(predicate);
        }

        public new IQueryable<DogTraining> GetAll()
        {
            //return DbSet;
            return DbSet.Include(x => x.Comments).Include(x => x.Dog).Include(x => x.Training);
        }

        public DogTraining GetByIds(int dogId, int trainingId)
        {
            var dogTraining = DbSet.Where(x => x.TrainingId == trainingId && x.DogId == dogId).Include(x => x.Comments).Include(x => x.Dog).Include(x=>x.Training).FirstOrDefault();
            return dogTraining;
        }
    }
}
