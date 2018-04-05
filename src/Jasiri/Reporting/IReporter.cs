namespace Jasiri.Reporting
{
    public interface IReporter
    {
        void Report(Span span);
    }

    public class NullReporter : IReporter
    {
        public static readonly NullReporter Instance = new NullReporter();

        private NullReporter()
        {

        }
        public void Report(Span span)
        {
            //do nothing
        }
    }
}
