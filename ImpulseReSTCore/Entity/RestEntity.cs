namespace ImpulseReSTCore.Entity
{
    public interface IRestEntity<T>
    {
        T Id { get; }

        string Uri { get; }
    }
}
