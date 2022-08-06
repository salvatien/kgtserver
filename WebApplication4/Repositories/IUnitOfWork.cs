namespace DogsServer.Repositories
{
    public interface IUnitOfWork
    {
        GuideRepository GuideRepository { get; }
        DogRepository DogRepository { get; }
        EventRepository EventRepository { get; }
        CertificateRepository CertificateRepository { get; }
        DogCertificateRepository DogCertificateRepository { get; }
        DogTrainingCommentRepository DogTrainingCommentRepository { get; }
        DogTrainingRepository DogTrainingRepository { get;  }
        TrainingCommentRepository TrainingCommentRepository { get;  }
        TrainingRepository TrainingRepository { get; }
        DogEventRepository DogEventRepository { get; }

        /// <summary>
        /// Commits all changes
        /// </summary>
        void Commit();
        /// <summary>
        /// Discards all changes that has not been commited
        /// </summary>
        void RejectChanges();
        void Dispose();
    }
    
}
