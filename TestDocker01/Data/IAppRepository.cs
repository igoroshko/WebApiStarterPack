using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestDocker01.Data.Entities;

namespace TestDocker01.Data
{
    public interface IAppRepository
    {
        IList<string> GetNames();

        int Add(Person person);

        T Get<T, TKey>(TKey key) where T : class;

        IQueryable<T> Query<T>() where T : class;
    }
}
