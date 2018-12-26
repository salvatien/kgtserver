using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogsServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DogsServer.Repositories
{
    public interface IUnitOfWork
    {
        GuideRepository GuideRepository { get; }
        DogRepository DogRepository { get; }
        ActionRepository ActionRepository { get; }
        EventRepository EventRepository { get; }
        CertificateRepository CertificateRepository { get; }
        DogCertificateRepository DogCertificateRepository { get; }
        DogTrainingCommentRepository DogTrainingCommentRepository { get; }
        DogTrainingRepository DogTrainingRepository { get;  }
        TrainingCommentRepository TrainingCommentRepository { get;  }
        TrainingRepository TrainingRepository { get; }
        DogEventRepository DogEventRepository { get; }
        DogActionRepository DogActionRepository { get; }

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
