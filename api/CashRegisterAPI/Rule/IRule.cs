namespace CashRegisterAPI.Rule;

// T = return value
// V = the current working value
public interface IRule<T, V>
{
    public string Name();
    public T Apply(V value);
    public bool IsApplicable(V value);
}
