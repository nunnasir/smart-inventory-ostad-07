namespace SmartInventory.DAL.Core;

public interface IUnitOfWork : IDisposable
{
    bool SaveChanges();
    void RollBack();
    Task<bool> SaveChangesAsync();
}
