namespace ReSTCore.Routing
{
    public enum RestfulAction
    {
        Show = 1,
        Create = 2,
        Update = 4,
        Delete = 8,
        Index = 16,
        Help = 32,
        ShowProperty = 64,
        UpdateProperty = 128,
        None = 16384,
    }
}