/*
 * Copyright (c) 2016, TopCoder, Inc. All rights reserved.
 */
using System;
using System.Linq;
using System.Data.Entity;
using $safeprojectname$.Entities;
using $safeprojectname$.Exceptions;

namespace $safeprojectname$.Services.Impl
{
    /// <summary>
    /// This abstract class is a base for all <see cref="IGenericService{T,S}"/> service implementations. It
    /// provides CRUD, search and export functionality.
    /// </summary>
    /// 
    /// <typeparam name="T">The type of the managed entities.</typeparam>
    /// <typeparam name="S">The type of the entities search criteria.</typeparam>
    /// 
    /// <threadsafety>
    /// This class is mutable but effectively thread-safe.
    /// </threadsafety>
    ///
    /// <author>TCSCODER</author>
    /// <version>1.0</version>
    /// <copyright>Copyright (c) 2016, TopCoder, Inc. All rights reserved.</copyright>
    public abstract class BaseGenericService<T, S> : BasePersistenceService, IGenericService<T, S>
        where T : IdentifiableEntity
        where S : BaseSearchCriteria
    {
        /// <summary>
        /// The cached name of the entity type.
        /// </summary>
        protected readonly string _entityName = typeof(T).Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGenericService{T,S}"/> class.
        /// </summary>
        protected BaseGenericService()
        {
        }

        /// <summary>
        /// Creates given entity.
        /// </summary>
        ///
        /// <param name="entity">The entity to create.</param>
        /// <returns>The created entity.</returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="entity"/> is invalid.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        public T Create(T entity)
        {
            return ProcessWithDb(db =>
            {
                Helper.ValidateArgumentNotNull(entity, nameof(entity));

                // get existing child entities from DB, otherwise new entities will be created in database
                ResolveChildEntities(db, entity);

                entity = GetDbSet<T>(db).Add(entity);
                db.SaveChanges();

                // load entity again to return all fields populated, because child entities may contain just Ids
                return Get(entity.Id);
            },
            $"creating {_entityName} entity",
            parameters: entity);
        }

        /// <summary>
        /// Updates given entity.
        /// </summary>
        ///
        /// <param name="entity">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        ///
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="entity"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If <paramref name="entity"/> is invalid.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// If entity with the given Id doesn't exist in DB.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        public T Update(T entity)
        {
            return ProcessWithDb(db =>
            {
                Helper.ValidateArgumentNotNull(entity, nameof(entity));

                T existing = Get(db, entity.Id, "entity.Id");

                // get existing child entities from DB, otherwise new entities will be created in database
                ResolveChildEntities(db, entity);

                // delete one-to-many children, so that they will be re-created
                DeleteChildEntities(db, existing);

                // copy fields to existing entity (attach approach doesn't work for child entities)
                UpdateEntityFields(existing, entity, db);
                db.SaveChanges();

                // load entity again to return all fields populated, because child entities may contain just Ids
                return Get(entity.Id);
            },
            $"updating {_entityName} entity",
            parameters: entity);
        }

        /// <summary>
        /// Retrieves entity with the given Id.
        /// </summary>
        ///
        /// <param name="id">The id of the entity to retrieve.</param>
        /// <returns>The retrieved entity.</returns>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="id"/> is not positive.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// If entity with the given Id doesn't exist in DB.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        public T Get(long id)
        {
            return ProcessWithDb(db =>
            {
                return Get(db, id);
            },
            $"retrieving {_entityName} entity",
            parameters: id);
        }

        /// <summary>
        /// Deletes entity with the given Id.
        /// </summary>
        ///
        /// <param name="id">The id of the entity to delete.</param>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="id"/> is not positive.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// If entity with the given Id doesn't exist in DB.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        public void Delete(long id)
        {
            ProcessWithDb(db =>
            {
                Helper.ValidateArgumentPositive(id, nameof(id));

                T entity = Get(db, id, full: false);

                DeleteAssociatedEntities(db, entity);
                GetDbSet<T>(db).Remove(entity);
            },
            $"deleting {_entityName} entity",
            saveChanges: true,
            parameters: id);
        }

        /// <summary>
        /// Retrieves entities matching given search criteria.
        /// </summary>
        ///
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The matched entities.</returns>
        ///
        /// <exception cref="ArgumentNullException">If the <paramref name="criteria"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If the <paramref name="criteria"/> is incorrect,
        /// e.g. PageNumber is negative, or PageNumber is positive and PageSize is not positive.</exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        public SearchResult<T> Search(S criteria)
        {
            return ProcessWithDb(db =>
            {
                Helper.CheckSearchCriteria(criteria);
                IQueryable<T> query = GetDbSet<T>(db);

                // include navigation properties
                query = IncludeNavigationProperties(query);

                // construct query conditions
                query = ConstructQueryConditions(query, criteria);

                // set total count of filtered records
                var result = new SearchResult<T>();
                result.TotalRecords = query.Count();

                // adjust SortBy (e.g. navigation properties should be handled by child services)
                criteria.SortBy = ResolveSortBy(criteria.SortBy);

                // construct SortBy property selector expression
                query = AddSortingCondition(query, criteria);

                // select required page
                if (criteria.PageNumber > 0)
                {
                    query = query.Skip(criteria.PageSize * (criteria.PageNumber - 1)).Take(criteria.PageSize);
                }

                // execute query and set result properties
                result.Items = query.ToList();
                SetSearchResultPageInfo(result, criteria);
                return result;
            },
            $"searching {_entityName} entities",
            parameters: criteria);
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
        protected virtual void DeleteAssociatedEntities(DbContext context, T entity)
        {
            // do nothing by default
        }

        /// <summary>
        /// Deletes the child entities from the database context.
        /// </summary>
        ///
        /// <remarks>
        /// All thrown exceptions will be propagated to caller method.
        /// </remarks>
        ///
        /// <param name="context">The database context.</param>
        /// <param name="entity">The entity to delete children for.</param>
        protected virtual void DeleteChildEntities(DbContext context, T entity)
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
        protected void DeleteChildEntity<TChild>(DbContext context, TChild entity)
            where TChild : IdentifiableEntity
        {
            if (entity != null)
            {
                GetDbSet<TChild>(context).Remove(entity);
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
        protected virtual void ResolveChildEntities(DbContext context, T entity)
        {
            // do nothing by default
        }

        /// <summary>
        /// Updates the <paramref name="existing"/> entity according to <paramref name="newEntity"/> entity.
        /// </summary>
        /// <remarks>Override in child services to update navigation properties.</remarks>
        /// <param name="existing">The existing entity.</param>
        /// <param name="newEntity">The new entity.</param>
        /// <param name="context">The database context.</param>
        protected virtual void UpdateEntityFields(T existing, T newEntity, DbContext context)
        {
            context.Entry(existing).CurrentValues.SetValues(newEntity);
        }

        /// <summary>
        /// Includes the navigation properties loading for the entity.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The updated query.</returns>
        protected virtual IQueryable<T> IncludeNavigationProperties(IQueryable<T> query)
        {
            // by default do not include any child property
            return query;
        }

        /// <summary>
        /// Gets the resolved SortBy property.
        /// </summary>
        /// <param name="sortBy">The SortBy property value.</param>
        /// <returns>Resolved SortBy property.</returns>
        protected virtual string ResolveSortBy(string sortBy)
        {
            // Note: override in child classes to handle sorting by navigation properties
            if (sortBy == null)
            {
                return "Id";
            }

            return sortBy;
        }

        /// <summary>
        /// Retrieves entity with the given Id.
        /// </summary>
        ///
        /// <param name="context">The database context.</param>
        /// <param name="id">The id of the entity to retrieve.</param>
        /// <param name="idParamName">The name of the Id parameter in the method.</param>
        /// <param name="full">Determines whether navigation properties should also be loaded.</param>
        /// <returns>The retrieved entity.</returns>
        ///
        /// <exception cref="ArgumentException">
        /// If <paramref name="id"/> is not positive.
        /// </exception>
        /// <exception cref="EntityNotFoundException">
        /// If entity with the given Id doesn't exist in DB.
        /// </exception>
        /// <exception cref="PersistenceException">
        /// If a DB-based error occurs.
        /// </exception>
        /// <exception cref="ServiceException">
        /// If any other errors occur while performing this operation.
        /// </exception>
        protected T Get(DbContext context, long id, string idParamName = "id", bool full = true)
        {
            Helper.ValidateArgumentPositive(id, idParamName);

            IQueryable<T> query = GetDbSet<T>(context);
            if (full)
            {
                query = IncludeNavigationProperties(query);
            }
            T entity = query.FirstOrDefault(e => e.Id == id);
            Helper.CheckFoundEntity(entity, id);
            return entity;
        }

        /// <summary>
        /// Applies ordering to the given query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="criteria">The search criteria.</param>
        /// <returns>The updated query with applied ordering.</returns>
        protected virtual IQueryable<T> AddSortingCondition(IQueryable<T> query, S criteria)
        {
            return criteria.SortType == SortType.Ascending
                ? query.OrderBy(criteria.SortBy)
                : query.OrderByDescending(criteria.SortBy);
        }
    }
}
