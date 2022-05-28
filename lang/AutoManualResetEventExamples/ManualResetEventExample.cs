namespace ThreadExamples
{
    public class ManualResetEventExample
    {
        private ManualResetEvent _event = new ManualResetEvent(false);

        public void DoExample()
        {
            Console.WriteLine("サンプル開始");

            Task.Run(() => ThreadProc("thread-1")); // シグナル待機
            Task.Run(() => ThreadProc("thread-2")); // シグナル待機

            Thread.Sleep(1000);
            Console.WriteLine("Set");
            _event.Set(); // 以後、Reset()するまでWaitOne()は待機しない

            Thread.Sleep(1000);
            Task.Run(() => ThreadProc("thread-3")); // シグナル非待機

            Thread.Sleep(1000);
            Console.WriteLine("Reset");
            _event.Reset();
            Task.Run(() => ThreadProc("thread-4")); // シグナル待機

            Thread.Sleep(1000);
            Console.WriteLine("Set");
            _event.Set();

            Thread.Sleep(1000);
            Console.WriteLine("サンプル終了");
        }

        private void ThreadProc(string name)
        {
            Console.WriteLine($"    {name}: シグナルを待機中...");
            _event.WaitOne();
            Console.WriteLine($"    {name}: 終了");
        }

    }
}
