using System.ComponentModel.DataAnnotations;

namespace AccountService.Database;

public class BaseEntity
{
    [Key]
    public int Id { get; set; }

    public DateTime Created { get; set; }

    public int CreatedBy { get; set; }

    public DateTime Modified { get; set; }

    public int ModifiedBy { get; set; }

    public bool Active { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; }
}