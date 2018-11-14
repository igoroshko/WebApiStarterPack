using System.Collections.Generic;
using System.Linq;
using TestDocker01.Data.Entities;

namespace TestDocker01.Data.Repositories
{
    public class AppRepository : IAppRepository
    {
        private AppDbContext _db;

        public AppRepository(AppDbContext context)
        {
            _db = context;
        }

        // public 

        public int Add(Person person)
        {
            var added = _db.People.Add(person);
            _db.SaveChanges();
            return added.Entity.Id;
        }

        public T Get<T, TKey>(TKey key)
            where T : class
        {
            return _db.Set<T>().Find(key);
        }

        public IList<string> GetNames()
        {
            var result = _db.People.Select(x => x.Name).ToList();
            return result;
        }

        public IQueryable<T> Query<T>() where T : class
        {
            return _db.Set<T>();
        }
    }
}
