namespace Jasiri.Reporting
{
    public interface IReporter
    {
        void Report(IZipkinSpan span);
    }

    public class NullReporter : IReporter
    {
        public static readonly NullReporter Instance = new NullReporter();

        private NullReporter()
        {

        }
        public void Report(IZipkinSpan span)
        {
            //do nothing
        }
    }
}
