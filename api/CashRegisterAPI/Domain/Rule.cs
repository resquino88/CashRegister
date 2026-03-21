namespace CashRegisterAPI.Domain;

public class Rule
{
    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Priority { get; private set; }
    public bool IsActive { get; private set; }

    public Rule() { }

    public Rule(int id, string name, int priority, bool isActive)
    {
        Id = id;
        Name = name;
        Priority = priority;
        IsActive = isActive;
    }
}
