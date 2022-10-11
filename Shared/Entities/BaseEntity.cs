namespace Shared.Entities;

public class BaseEntity
{
    public int Id { get; set; }
    public DateTime Created { get; set; }
    public int CreatedBy { get; set; }
    public DateTime Modified { get; set; }
    public int ModifiedBy { get; set; }
    public bool Active { get; set; }
}