namespace GmWeb.Logic.Importing.Regions.ShapefileProcessors
{
    public class ShapefileProcessorFactory
    {
        public static ShapefileProcessor Create<T>(string path) where T : ShapefileProcessor, new()
        {
            var processor = new T();
            processor.Initialize(path);
            return processor;
        }
    }
}
