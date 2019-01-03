using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DogsServer.Models;


namespace DogsServer.Repositories
{
    public class TrainingCommentRepository : Repository<DogsServer.Models.TrainingComment>
    {
        protected new DbSet<TrainingComment> DbSet;

        public TrainingCommentRepository(DbContext dataContext)
             : base(dataContext)
        {
            DbSet = dataContext.Set<TrainingComment>();
        }


        public new IQueryable<TrainingComment> SearchFor(Expression<Func<TrainingComment, bool>> predicate)
        {
            return DbSet.Include(x => x.Author).Include(x => x.Training).Where(predicate);
        }

        public new IQueryable<TrainingComment> GetAll()
        {
            return DbSet.Include(x => x.Author).Include(x => x.Training);
        }

        public TrainingComment GetById(int id)
        {
            var comment = DbSet.Where(x => x.TrainingCommentId == id).Include(x => x.Author).Include(x => x.Training).FirstOrDefault();
            return comment;
        }

        public IQueryable<TrainingComment> GetAllByTrainingId(int trainingId)
        {
            var comments = DbSet.Where(x => x.TrainingId == trainingId).Include(x => x.Author).Include(x => x.Training);
            return comments;
        }
    }
}