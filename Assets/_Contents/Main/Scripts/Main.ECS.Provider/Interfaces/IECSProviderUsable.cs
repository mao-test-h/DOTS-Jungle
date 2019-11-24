namespace Main.ECS.Provider
{
    public interface IECSProviderUsable
    {
        ECSProvider Provider { get; }
        void SetProvider(in ECSProvider provider);
    }
}
