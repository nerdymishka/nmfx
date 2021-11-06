namespace Mettle
{
    public partial class AssertImpl : IAssert
    {
        public static AssertImpl Instance { get; } = new AssertImpl();
    }
}