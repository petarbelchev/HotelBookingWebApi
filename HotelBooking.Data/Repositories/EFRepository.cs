using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Data.Repositories;

public class EFRepository<TEntity> : IRepository<TEntity>
	where TEntity : class
{
	public EFRepository(ApplicationDbContext context)
	{
		DbContext = context ?? throw new ArgumentNullException(nameof(context));
		DbSet = DbContext.Set<TEntity>();
	}

	protected DbSet<TEntity> DbSet { get; set; }

	protected ApplicationDbContext DbContext { get; set; }

	public virtual async Task AddAsync(TEntity entity) => await DbSet.AddAsync(entity).AsTask();

	public async Task AddRangeAsync(params TEntity[] entities) => await DbSet.AddRangeAsync(entities);

	public virtual IQueryable<TEntity> All() => DbSet;

	public virtual IQueryable<TEntity> AllAsNoTracking() => DbSet.AsNoTracking();

	public virtual void Delete(TEntity entity) => DbSet.Remove(entity);
	
	public async Task ExecuteSqlRawAsync(string sql, params object[] parameters)
		=> await DbContext.Database.ExecuteSqlRawAsync(sql, parameters);
	
	public async Task<TEntity?> FindAsync(int id) => await DbSet.FindAsync(id);

	public async Task<int> SaveChangesAsync() => await DbContext.SaveChangesAsync();
}
