namespace ImpulseReSTCore.Routing
{
    public enum RestfulAction
    {
        Show = 1,
        Create = 2,
        Update = 4,
        Destroy = 8,
        Index = 16,
        Help = 32,
        None = 16384,
    }
}