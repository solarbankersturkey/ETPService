using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IotService.Model
{
    public class CumulativeRepository : BaseRepository<Cumulative>, ICumulativeRepository
    {
        public CumulativeRepository(IMongoCumulativeDBContext context) : base(context)
        {

        }
    }
}
