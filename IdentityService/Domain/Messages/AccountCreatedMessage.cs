namespace IdentityService.Domain.Messages;

public class AccountCreatedMessage: IEvent {
    public int Id { get; set; }
}