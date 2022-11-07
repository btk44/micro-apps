namespace IdentityService.Domain.Messages;

public class AccountUpdatedMessage: IEvent {
    public int Id { get; set; }
}