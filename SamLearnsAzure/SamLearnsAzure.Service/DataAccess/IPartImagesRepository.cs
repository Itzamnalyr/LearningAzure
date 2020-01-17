using SamLearnsAzure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamLearnsAzure.Service.DataAccess
{
    public interface IPartImagesRepository
    {
        Task<List<PartImages>> GetPartImages(IRedisService redisService, bool useCache);
        Task<PartImages> GetPartImage(IRedisService redisService, bool useCache, string partNum);
        Task<PartImages> SavePartImage(IRedisService redisService, PartImages partImage);
    }
}
