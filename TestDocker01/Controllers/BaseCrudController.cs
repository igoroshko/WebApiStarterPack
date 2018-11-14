using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using TestDocker01.Common;
using TestDocker01.Data;
using TestDocker01.Data.Entities;
using TestDocker01.Models;

namespace TestDocker01.Controllers
{
    /// <summary>
    /// The base controller.
    /// </summary>
    public abstract class BaseCrudController<T, S> : BaseController
        where T : IdentifiableEntity
        where S : BaseSearchCriteria
    {
        protected readonly AppDbContext _db;

        public BaseCrudController(AppDbContext db)
        {
            _db = db;
        }

        public BaseCrudController(AppDbContext db, IMapper mapper)
            : base(mapper)
        {
            _db = db;
        }

        public TEntity Single<TEntity>(int id)
            where TEntity : IdentifiableEntity
        {
            return _db.Set<TEntity>().FirstOrDefault(x => x.Id == id);
        }

        public virtual T CreateEntity(T entity)
        {
            ValidateEntityOnCreate(entity);
            if (HasErrors())
            {
                return null;
            }

            ResolveChildEntities(entity);
            var result = _db.Set<T>().Add(entity);
            SaveChanges();
            return result.Entity;
        }

        /// <summary>
        /// Updates given entity.
        /// </summary>
        ///
        /// <param name="entity">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        public virtual void UpdateEntity(int id, T entity)
        {
            ValidateEntityOnUpdate(id, entity);
            if (HasErrors())
            {
                return;
            }

            T existing = _db.Set<T>().Find(id);
            if (existing == null)
            {
                AddNotFoundError();
                return;
            }

            // get existing child entities from DB, otherwise new entities will be created in database
            ResolveChildEntities(entity);

            if (HasErrors())
            {
                return;
            }

            // copy fields to existing entity (attach approach doesn't work for child entities)
            UpdateEntityFields(existing, entity);

            SaveChanges();
        }

        /// <summary>
        /// Retrieves entity with the given Id.
        /// </summary>
        ///
        /// <param name="id">The id of the entity to retrieve.</param>
        /// <returns>The retrieved entity.</returns>
        public virtual T GetEntity(int id)
        {
            var result = Get(id, full: true);
            return result;
        }

        /// <summary>
        /// Retrieves list of all entities.
        /// </summary>
        ///
        /// <returns>The list of all entities.</returns>
        public virtual IList<T> GetAllEntities()
        {
            return _db.Set<T>().ToList();
        }

        /// <summary>
        /// Deletes entity with the given Id.
        /// </summary>
        ///
        /// <param name="id">The id of the entity to delete.</param>
        public virtual void DeleteEntity(int id)
        {
            T entity = Get(id, full: true);
            if (HasErrors())
            {
                return;
            }

            if (entity == null)
            {
                AddNotFoundError();
                return;
            }

            DeleteAssociatedEntities(entity);
            _db.Set<T>().Remove(entity);
            SaveChanges();
        }

        /// <summary>
        /// Retrieves entities matching given search criteria.
        /// </summary>
        ///
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The matched entities.</returns>
        public virtual SearchResult<T> SearchEntities(S criteria)
        {
            IQueryable<T> query = IncludeSearchItemNavigationProperties(_db.Set<T>());

            // construct query conditions
            query = ConstructQueryConditions(query, criteria);

            // set total count of filtered records
            var metadata = new Metadata
            {
                PageIndex = criteria.PageIndex,
                PageSize = criteria.PageSize,
                TotalCount = query.Count()
            };
            var result = new SearchResult<T> { Metadata = metadata };

            // adjust SortBy (e.g. navigation properties should be handled by child services)
            var sortBy = ResolveSortBy(criteria.SortBy);

            if (sortBy.Contains(","))
            {
                var fields = sortBy.Split(",");
                foreach (string softByField in fields)
                {
                    criteria.SortBy = softByField;

                    // construct SortBy property selector expression
                    query = AddSortingCondition(query, criteria);
                }
            }
            else
            {
                criteria.SortBy = sortBy;

                // construct SortBy property selector expression
                query = AddSortingCondition(query, criteria);
            }

            // select required page
            if (criteria.PageIndex > 0)
            {
                query = query.Skip(criteria.PageSize * (criteria.PageIndex - 1)).Take(criteria.PageSize);
            }

            // execute query and set result properties
            result.Items = query.ToList();
            return result;
        }

        /// <summary>
        /// Gets the Queryable for entities.
        /// </summary>
        /// <returns>
        /// The Queryable for entities.
        /// </returns>
        protected IQueryable<T> Query()
        {
            return _db.Set<T>();
        }

        protected T Get(int id, bool full = true)
        {
            if (id <= 0)
            {
                AddIdNotPositiveError();
                return null;
            }

            IQueryable<T> query = _db.Set<T>();
            if (full)
            {
                query = IncludeNavigationProperties(query);
            }

            T entity = query.FirstOrDefault(e => e.Id == id);
            return entity;
        }

        protected virtual void ValidateEntityCommon(T entity)
        {
            if (entity == null)
            {
                // add 400 error
                AddValidationError();
            }
        }

        protected virtual void ValidateEntityOnCreate(T entity)
        {
            ValidateEntityCommon(entity);
        }

        protected virtual void ValidateEntityOnUpdate(int id, T entity)
        {
            ValidateEntityCommon(entity);

            if (id <= 0)
            {
                AddIdNotPositiveError();
            }
        }

        /// <summary>
        /// Applies filters to the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated query with applied filters.</returns>
        protected virtual IQueryable<T> ConstructQueryConditions(IQueryable<T> query, S criteria)
        {
            return query;
        }

        /// <summary>
        /// Deletes the associated entities from the database context.
        /// </summary>
        ///
        /// <remarks>
        /// All thrown exceptions will be propagated to caller method.
        /// </remarks>
        ///
        /// <param name="context">The database context.</param>
        /// <param name="entity">The entity to delete associated entities for.</param>
        protected virtual void DeleteAssociatedEntities(T entity)
        {
            // do nothing by default
        }

        /// <summary>
        /// Deletes the child entity from database, if it is not null.
        /// </summary>
        /// <remarks>All exceptions will be propagated to the caller.</remarks>
        /// <typeparam name="TChild">The type of the child.</typeparam>
        /// <param name="context">The database context.</param>
        /// <param name="entity">The entity to delete.</param>
        protected void DeleteChildEntity<TChild>(TChild entity)
            where TChild : IdentifiableEntity
        {
            if (entity != null)
            {
                _db.Set<TChild>().Remove(entity);
            }
        }

        /// <summary>
        /// Updates the child entities by loading them from the database context.
        /// </summary>
        ///
        /// <remarks>
        /// All thrown exceptions will be propagated to caller method.
        /// </remarks>
        ///
        /// <param name="context">The database context.</param>
        /// <param name="entity">The entity to resolve.</param>
        protected virtual void ResolveChildEntities(T entity)
        {
            // do nothing by default
        }

        /// <summary>
        /// Resolves given identifiable entities.
        /// </summary>
        /// <typeparam name="T">The actual type of entities in the collection.</typeparam>
        /// <param name="context">The DB context.</param>
        /// <param name="items">The entities to resolve.</param>
        /// <remarks>All exceptions will be propagated.</remarks>
        protected void ResolveEntities<TEntity>(IList<TEntity> items)
            where TEntity : IdentifiableEntity
        {
            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    items[i] = ResolveChildEntity(items[i]);
                }
            }
        }

        /// <summary>
        /// Check that entity is not <c>null</c> and tries to retrieve its updated value from the database context.
        /// </summary>
        ///
        /// <remarks>
        /// All thrown exceptions will be propagated to caller method.
        /// </remarks>
        ///
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="context">The database context.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="id">The Id of the entity.</param>
        /// <param name="isRequired">Specifies whether NotFound exception should be thrown if not found.</param>
        /// <returns>The updated entity from the database context.</returns>
        protected TEntity ResolveChildEntity<TEntity>(TEntity entity, int? id = null, bool isRequired = false)
            where TEntity : IdentifiableEntity
        {
            int? entityId = id > 0 ? id : entity?.Id;
            if (entityId == null)
            {
                if (isRequired)
                {
                    // add 400-404?
                }

                return null;
            }

            TEntity child = _db.Set<TEntity>().Find(entityId.Value);
            if (child == null)
            {
                //throw new EntityNotFoundException(
                //    $"Child entity {typeof(TEntity).Name} with Id={entityId} was not found.");
            }

            return child;
        }

        protected virtual void UpdateEntityFields(T existing, T newEntity)
        {
            newEntity.Id = existing.Id;
            _db.Entry(existing).CurrentValues.SetValues(newEntity);
        }

        protected virtual IQueryable<T> IncludeNavigationProperties(IQueryable<T> query)
        {
            // by default do not include any child property
            return query;
        }

        protected virtual IQueryable<T> IncludeSearchItemNavigationProperties(IQueryable<T> query)
        {
            // by default do not include any child property
            return query;
        }

        protected virtual string ResolveSortBy(string sortBy)
        {
            // Note: override in child classes to handle sorting by navigation properties
            if (sortBy == null)
            {
                return "Id";
            }

            return sortBy;
        }

        protected virtual IQueryable<T> AddSortingCondition(IQueryable<T> query, S criteria)
        {
            return criteria.SortType == SortType.ASC
                ? query.OrderBy(criteria.SortBy)
                : query.OrderByDescending(criteria.SortBy);
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
}
