
using AutoMapper.QueryableExtensions;
using Common.Utilities;
using Core.Utilities.Mapping;
using Dapper;
using Data.Context;
using Data.Contract.GenericContract;
using Entities.Entities.BaseEntities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Linq.Expressions;
using System.Text;

namespace Data.Repository.GenericRepository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        protected readonly AppDbContext DbContext;
        private readonly IConfiguration configuration;

        public DbSet<TEntity> Entities { get; }
        public virtual IQueryable<TEntity> TableDeleted =>
            Entities.Where(p => EF.Property<bool?>(p, "IsDeleted") == true);
        public virtual IQueryable<TEntity> TableNoTrackingDeleted =>
            Entities.Where(p => EF.Property<bool?>(p, "IsDeleted") == true).AsNoTracking();
        public virtual IQueryable<TEntity> Table =>
            Entities.Where(p => EF.Property<bool?>(p, "IsDeleted") != true);
        public virtual IQueryable<TEntity> TableNoTracking =>
            Entities.Where(p => EF.Property<bool?>(p, "IsDeleted") != true).AsNoTracking();

        public Repository(AppDbContext dbContext, IConfiguration configuration)
        {
            DbContext = dbContext;
            this.configuration = configuration;
            Entities = DbContext.Set<TEntity>(); // City => Cities
        }

        #region Async Method

        public async Task<TEntity> GetByIdQuerAsync(long Id)
        {
            var tableName = typeof(TEntity).Name;
            var connectionString = configuration.GetConnectionString("SqlServerConnection");

            var sql = $"SELECT * FROM [{tableName}] WHERE IsDeleted = 0 and Id = @Id";

            using (var connection = new SqlConnection(connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<TEntity>(sql, new { Id = Id });
            }
        }

        public async Task<IEnumerable<TEntity>> GetByRangIdQuerAsync(List<long> Ids)
        {
            var tableName = typeof(TEntity).Name;
            var connectionString = configuration.GetConnectionString("SqlServerConnection");

            var sql = $"SELECT * FROM [{tableName}] WHERE IsDeleted = 0 and Id in (";

            foreach (var id in Ids)
                sql += $"{id},";
            sql += "0)";

            using (var connection = new SqlConnection(connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<IEnumerable<TEntity>>(sql);

            }
        }

        public async Task<TEntity> GetByIdQuerDeletedItemAsync(long Id)
        {
            var tableName = typeof(TEntity).Name;
            var connectionString = configuration.GetConnectionString("SqlServerConnection");

            var sql = $"SELECT * FROM [{tableName}] WHERE IsDeleted = 1 and Id = @Id";

            using (var connection = new SqlConnection(connectionString))
            {
                return await connection.QueryFirstOrDefaultAsync<TEntity>(sql, new { Id = Id });
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetByQueryAsync(CancellationToken cancellationToken, string where = null)
        {
            var tableName = typeof(TEntity).Name;
            var sql = $"SELECT * FROM [{tableName}] WHERE IsDeleted <> 1";
            var connectionString = configuration.GetConnectionString("SqlServerConnection");

            if (!string.IsNullOrWhiteSpace(where))
                sql += " AND " + where;


            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<TEntity>(sql, cancellationToken);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetByQueryDeletedItemsAsync(CancellationToken cancellationToken, string where = null)
        {
            var tableName = typeof(TEntity).Name;
            var sql = $"SELECT * FROM [{tableName}] WHERE IsDeleted = 1";

            if (!string.IsNullOrWhiteSpace(where))
                sql += " AND " + where;

            var connectionString = configuration.GetConnectionString("SqlServerConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<TEntity>(sql, cancellationToken);
            }
        }

        public virtual async Task<TEntity> GetByIdDeletedAsync(CancellationToken cancellationToken, params object[] ids)
        {
            return await TableNoTrackingDeleted
                .FirstOrDefaultAsync(p =>
                EF.Property<long>(p, "Id") == (long)ids[0], cancellationToken);
        }

        public virtual async Task<TEntity> GetByIdAsync(CancellationToken cancellationToken, params object[] ids)
        {
            return await TableNoTracking
                .FirstOrDefaultAsync(p =>
                EF.Property<long>(p, "Id") == (long)ids[0], cancellationToken);
        }

        public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));

            entity = SetAddProperty(entity);


            await Entities.AddAsync(entity, cancellationToken).ConfigureAwait(false);
            if (saveNow)
                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));
            List<TEntity> entitiesChanged = new List<TEntity>();
            foreach (TEntity entity in entities.ToList())
                entitiesChanged.Add(SetAddProperty(entity));
            await Entities.AddRangeAsync(entitiesChanged, cancellationToken).ConfigureAwait(false);
            if (saveNow)
                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));

            var rowVersionValue = (byte[])entity.GetType().GetProperty("RowVersion").GetValue(entity);

            Entities.Attach(entity);

            DbContext.Entry(entity).Property("RowVersion").OriginalValue = rowVersionValue;

            SetUpdateProperty(entity);

            DbContext.Entry(entity).State = EntityState.Modified;

            if (saveNow)
                await DbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));

            foreach (var entity in entities)
            {
                var rowVersionValue = (byte[])entity.GetType().GetProperty("RowVersion").GetValue(entity);

                Entities.Attach(entity);

                DbContext.Entry(entity).Property("RowVersion").OriginalValue = rowVersionValue;

                SetUpdateProperty(entity);

                DbContext.Entry(entity).State = EntityState.Modified;
            }

            if (saveNow)
                await DbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));
            entity = SetDeleteProperty(entity);
            Entities.Update(entity);
            if (saveNow)
                await DbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));
            List<TEntity> entitiesChanged = new List<TEntity>();
            foreach (TEntity entity in entities.ToList())
                entitiesChanged.Add(SetAddProperty(entity));
            Entities.UpdateRange(entitiesChanged);
            if (saveNow)
                await DbContext.SaveChangesAsync(cancellationToken);
        }
        #endregion

        #region Sync Methods
        public TEntity GetByIdDeleted(params object[] ids)
        {
            return TableNoTrackingDeleted
                .FirstOrDefault(p =>
                EF.Property<long>(p, "Id") == (long)ids[0]);
        }

        public virtual TEntity GetById(params object[] ids)
        {
            return TableNoTracking
                .FirstOrDefault(p =>
                EF.Property<long>(p, "Id") == (long)ids[0]);
        }

        public virtual void Add(TEntity entity, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));
            entity = SetAddProperty(entity);
            Entities.Add(entity);
            if (saveNow)
                DbContext.SaveChanges();
        }

        public virtual void AddRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));
            List<TEntity> entitiesChanged = new List<TEntity>();
            foreach (TEntity entity in entities.ToList())
                entitiesChanged.Add(SetAddProperty(entity));
            Entities.AddRange(entitiesChanged);
            if (saveNow)
                DbContext.SaveChanges();
        }

        public virtual void Update(TEntity entity, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));
            entity = SetUpdateProperty(entity);
            Entities.Update(entity);
            DbContext.SaveChanges();
        }

        public virtual void UpdateRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));
            List<TEntity> entitiesChanged = new List<TEntity>();
            foreach (TEntity entity in entities.ToList())
                entitiesChanged.Add(SetAddProperty(entity));
            Entities.UpdateRange(entitiesChanged);
            if (saveNow)
                DbContext.SaveChanges();
        }

        public virtual void Delete(TEntity entity, bool saveNow = true)
        {
            Assert.NotNull(entity, nameof(entity));
            entity = SetDeleteProperty(entity);
            Entities.Update(entity);
            if (saveNow)
                DbContext.SaveChanges();
        }

        public virtual void DeleteRange(IEnumerable<TEntity> entities, bool saveNow = true)
        {
            Assert.NotNull(entities, nameof(entities));
            List<TEntity> entitiesChanged = new List<TEntity>();
            foreach (TEntity entity in entities.ToList())
                entitiesChanged.Add(SetAddProperty(entity));
            Entities.UpdateRange(entitiesChanged);
            if (saveNow)
                DbContext.SaveChanges();
        }
        #endregion

        #region Attach & Detach
        public virtual void Detach(TEntity entity)
        {
            Assert.NotNull(entity, nameof(entity));
            var entry = DbContext.Entry(entity);
            if (entry != null)
                entry.State = EntityState.Detached;
        }

        public virtual void Attach(TEntity entity)
        {
            Assert.NotNull(entity, nameof(entity));
            if (DbContext.Entry(entity).State == EntityState.Detached)
                Entities.Attach(entity);
        }
        #endregion

        #region Explicit Loading
        public virtual async Task LoadCollectionAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty, CancellationToken cancellationToken)
            where TProperty : class
        {
            Attach(entity);

            var collection = DbContext.Entry(entity).Collection(collectionProperty);
            if (!collection.IsLoaded)
                await collection.LoadAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual void LoadCollection<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionProperty)
            where TProperty : class
        {
            Attach(entity);
            var collection = DbContext.Entry(entity).Collection(collectionProperty);
            if (!collection.IsLoaded)
                collection.Load();
        }

        public virtual async Task LoadReferenceAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty, CancellationToken cancellationToken)
            where TProperty : class
        {
            Attach(entity);
            var reference = DbContext.Entry(entity).Reference(referenceProperty);
            if (!reference.IsLoaded)
                await reference.LoadAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual void LoadReference<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> referenceProperty)
            where TProperty : class
        {
            Attach(entity);
            var reference = DbContext.Entry(entity).Reference(referenceProperty);
            if (!reference.IsLoaded)
                reference.Load();
        }
        #endregion


        #region Set Subscriber Properties

        private TEntity SetAddProperty(TEntity entity)
        {
            var createAtProp = entity.GetType().GetProperty("CreateAt");
            if (createAtProp != null && createAtProp.CanWrite)
                createAtProp.SetValue(entity, DateTime.UtcNow);

            // Set CreateByUserId
            var createByProp = entity.GetType().GetProperty("CreateByUserId");
            if (createByProp != null && createByProp.CanWrite)
            {
                // اگر کاربر لاگین شده باشد
                long? userId = 0;// _userContextService?.UserId; // هرجایی که UserId را می‌گیری
                createByProp.SetValue(entity, userId);
            }

            return entity;
        }

        private TEntity SetUpdateProperty(TEntity entity)
        {
            var updateAt = entity.GetType().GetProperty("UpdateAt");
            if (updateAt != null && updateAt.CanWrite)
                updateAt.SetValue(entity, DateTime.UtcNow);

            var updateUserId = entity.GetType().GetProperty("UpdateByUserId");
            if (updateUserId != null && updateUserId.CanWrite)
            {
                long? userId = 0;// _userContextService?.UserId; // هرجایی که UserId را می‌گیری
                updateUserId.SetValue(entity, userId);
            }

            //var updateRowVersion = entity.GetType().GetProperty("RowVersion");
            //if (updateRowVersion != null && updateRowVersion.CanWrite)
            //{
            //    byte[] RowVersion = Encoding.UTF8.GetBytes(Guid.NewGuid().ToString());
            //    updateRowVersion.SetValue(entity, RowVersion);
            //}
            return entity;
        }

        private TEntity SetDeleteProperty(TEntity entity)
        {
            var deletedAt = entity.GetType().GetProperty("DeletedAt");
            if (deletedAt != null && deletedAt.CanWrite)
                deletedAt.SetValue(entity, DateTime.UtcNow);


            var isDeleted = entity.GetType().GetProperty("IsDeleted");
            if (isDeleted != null && isDeleted.CanWrite)
                isDeleted.SetValue(entity, true);


            var deletedByUserId = entity.GetType().GetProperty("DeletedByUserId");
            if (deletedByUserId != null && deletedByUserId.CanWrite)
            {
                long? userId = 0;// _userContextService?.UserId; // هرجایی که UserId را می‌گیری
                deletedByUserId.SetValue(entity, userId);
            }
            return entity;
        }

        #endregion


        #region GetDtos
        public async Task<IEnumerable<TDto>> GetDtos<TDto>(CancellationToken cancellationToken)
        {
            var list = await TableNoTracking.ToListAsync(cancellationToken);
            var dtoList = list.ConvertListObject<TDto, TEntity>();
            return dtoList;
        }

        public async Task<IEnumerable<TDto>> GetDtos<TDto>(
            Expression<Func<TEntity, bool>> predicate,
            CancellationToken cancellationToken)
        {
            var list = await TableNoTracking.Where(predicate)
                .ToListAsync(cancellationToken);

            var dtoList = list.ConvertListObject<TDto, TEntity>();
            return dtoList;
        }


        public async Task<TDto> GetDtoById<TDto, TKey>(TKey Id, CancellationToken cancellationToken)
        {
            var Item = await TableNoTracking.FirstOrDefaultAsync(e =>
            EF.Property<TKey>(e, "Id").Equals(Id), cancellationToken);
            var dto = Item.ConvertObject<TDto, TEntity>();
            return dto;
        }
        #endregion


        public async Task AddAsync<TDto>(TDto dto, CancellationToken cancellationToken, bool saveNow = true)
        {
            var entity = dto.ConvertObject<TEntity, TDto>();
            await AddAsync(entity, cancellationToken, saveNow);

        }

    }
}
