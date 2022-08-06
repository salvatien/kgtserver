using System.Linq;
using Dogs.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace DogsServer.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _dbContext;
        #region Repositories
        public GuideRepository GuideRepository =>
           new GuideRepository(_dbContext);
        public DogRepository DogRepository =>
           new DogRepository(_dbContext);
        public EventRepository EventRepository =>
           new EventRepository(_dbContext);
        public CertificateRepository CertificateRepository =>
           new CertificateRepository(_dbContext);
        public DogTrainingCommentRepository DogTrainingCommentRepository => 
            new DogTrainingCommentRepository(_dbContext);
        public DogTrainingRepository DogTrainingRepository => 
            new DogTrainingRepository(_dbContext);
        public TrainingCommentRepository TrainingCommentRepository => 
            new TrainingCommentRepository(_dbContext);
        public TrainingRepository TrainingRepository =>
            new TrainingRepository(_dbContext);
        public DogCertificateRepository DogCertificateRepository =>
           new DogCertificateRepository(_dbContext);
        public DogEventRepository DogEventRepository =>
           new DogEventRepository(_dbContext);
        #endregion
        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Commit()
        {
            _dbContext.SaveChanges();
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
        public void RejectChanges()
        {
            foreach (var entry in _dbContext.ChangeTracker.Entries()
                  .Where(e => e.State != EntityState.Unchanged))
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Modified:
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                }
            }
        }
    }
}
