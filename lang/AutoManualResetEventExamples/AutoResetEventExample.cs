namespace ThreadExamples
{
    public class AutoResetEventExample
    {
        private AutoResetEvent _event = new AutoResetEvent(false);

        public void DoExample()
        {
            Console.WriteLine("サンプル開始");

            Task.Run(() => ThreadProc("thread-1"));
            Task.Run(() => ThreadProc("thread-2"));
            Task.Run(() => ThreadProc("thread-3"));

            Thread.Sleep(1000);
            Console.WriteLine("Set(1回目)");
            _event.Set(); // 任意の１スレッドが待機解除

            Thread.Sleep(1000);
            Console.WriteLine("Set(2回目)");
            _event.Set(); // 任意の１スレッドが待機解除

            Thread.Sleep(1000);
            Console.WriteLine("Set(3回目)");
            _event.Set(); // 任意の１スレッドが待機解除

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
