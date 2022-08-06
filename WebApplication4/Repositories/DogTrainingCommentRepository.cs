using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using Dogs.Data.Models;


namespace DogsServer.Repositories
{
    public class DogTrainingCommentRepository : Repository<Dogs.Data.Models.DogTrainingComment>
    {
        protected new DbSet<DogTrainingComment> DbSet;

        public DogTrainingCommentRepository(DbContext dataContext)
             : base(dataContext)
        {
            DbSet = dataContext.Set<DogTrainingComment>();
        }


        public new IQueryable<DogTrainingComment> SearchFor(Expression<Func<DogTrainingComment, bool>> predicate)
        {
            return DbSet.Include(x => x.Author).Include(x => x.DogTraining).Where(predicate);
        }

        public new IQueryable<DogTrainingComment> GetAll()
        {
            return DbSet.Include(x => x.Author).Include(x => x.DogTraining);
        }

        public DogTrainingComment GetById(int id)
        {
            //return DbSet.Find(id);
            //var dog = DbSet.Where(x => x.DogID == id).Include(x => x.Guide).FirstOrDefault();
            //return dog;
            var comment = DbSet.Where(x => x.DogTrainingCommentId == id).Include(x => x.Author).Include(x => x.DogTraining).FirstOrDefault();
            return comment;
        }

        public IQueryable<DogTrainingComment> GetAllByDogIdAndTrainingId(int dogId, int trainingId)
        {
            return DbSet.Where(x => x.DogId == dogId && x.TrainingId == trainingId).Include(x => x.Author).Include(x => x.DogTraining);
        }
    }
}