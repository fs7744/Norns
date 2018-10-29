namespace SyncInterceptor
{
    public class TestService : ITestService
    {
        public int Sum(int x, int y)
        {
            var result = x;
            for (int i = 0; i < x; i++)
            {
                result += y;
            }
            return result;
        }

        public void Sum()
        {
            var result = 40;
            for (int i = 0; i < 40; i++)
            {
                result += 4;
            }
        }
    }
}