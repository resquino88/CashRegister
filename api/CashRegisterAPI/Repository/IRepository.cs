namespace CashRegisterAPI.Repository;

public interface IRepository<T>
{
    public Task<T> GetByName(string name);
    public Task<T> GetById(int id);
    public Task<List<T>> GetAll();
}
