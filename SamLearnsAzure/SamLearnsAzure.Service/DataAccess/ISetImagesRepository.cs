using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.DataAccess
{
    public interface ISetImagesRepository
    {
        Task<SetImages> GetSetImage(IRedisService redisService, bool useCache, string setNum);

        Task<SetImages> SaveSetImage(SetImages setImage);
    }
}
