namespace IdentityService.Domain.Messages;

public class AccountDeletedMessage: IEvent {
    public int Id { get; set; }
}