using TransactionService.Domain.Common.Entities;

namespace TransactionService.Domain.Entities;

public class CurrencyEntity: BaseEntity {
    public string Description { get; set; }
    public string Code { get; set; }
    public int VisualPropertiesId { get; set; }
    public VisualPropertiesEntity VisualProperties { get; set; } = new VisualPropertiesEntity();
}