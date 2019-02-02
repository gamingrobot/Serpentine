namespace Serpentine.IISModule.Tasks
{
    internal interface IMetricsTask
    {
        void BeginRequest();

        void PreHandler();

        void PostHandler();

        void EndRequest();
    }
}