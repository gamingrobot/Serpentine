namespace Serpentine.IISModule.Tasks
{
    internal interface IMetricTask
    {
        void BeginRequest();

        void PreHandler();

        void PostHandler();

        void EndRequest();
    }
}