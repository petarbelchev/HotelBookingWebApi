namespace HotelBooking.Data.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
	public Task AddAsync(TEntity entity);

	public Task AddRangeAsync(params TEntity[] entities);

	public IQueryable<TEntity> All();

	public IQueryable<TEntity> AllAsNoTracking();

	public void Delete(TEntity entity);
	
	public Task<TEntity?> FindAsync(int id);

	public Task ExecuteSqlRawAsync(string sql, params object[] parameters);
	
	public Task<int> SaveChangesAsync();
}
