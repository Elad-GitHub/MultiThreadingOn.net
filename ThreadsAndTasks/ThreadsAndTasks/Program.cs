using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadsAndTasks
{
    class Program
    {
        private static event Action EventFinished = () => { };

        private static object _locker = new object();

        private static ManualResetEvent _mre = new ManualResetEvent(false);

        private static AutoResetEvent _are = new AutoResetEvent(true);

        private static Mutex _mutex = new Mutex();

        private static Semaphore _semaphore = new Semaphore(2, 2);

        static void Main(string[] args)
        {
            #region Example for asynchronous code. The main thread does not wait for the inner thread.
            //Log("Before inner thread");

            //new Thread(() =>
            //{
            //    Thread.Sleep(500);
            //    Log("Inside inner  thread");
            //}).Start();

            //Log("After inner thread");

            //Thread.Sleep(1000);

            #endregion

            #region Example of a blocking thread.
            //Log("Before blocking thread");

            //var blockingThread = new Thread(() =>
            //{
            //    Thread.Sleep(500);
            //    Log("Inside blocking thread");
            //});

            //blockingThread.Start();

            //blockingThread.Join();

            //Log("After blocking thread");

            #endregion

            #region Example of a polling thread.
            //Log("Before polling thread");

            //var pollComplete = false;

            //var pollingThread = new Thread(() =>
            //{
            //    Log("Inside polling thread");
            //    Thread.Sleep(500);

            //    pollComplete = true;
            //});

            //pollingThread.Start();

            //while (!pollComplete)
            //{
            //    Console.WriteLine("Polling...");
            //    Thread.Sleep(50);
            //}

            //Log("After polling thread");

            #endregion

            #region Example for event-based callbacks (losing the calling thread on callback and causes nesting).
            //Log("Before event thread");

            //var eventThread = new Thread(() =>
            //{
            //    Log("Inside event thread");
            //    Thread.Sleep(500);

            //    EventFinished();
            //});

            //EventFinished += () =>
            //{
            //    Log("Event thread callback on complete");
            //};

            //eventThread.Start();

            //Log("After event thread");

            ////wait for work to finish
            //Thread.Sleep(1000);

            //Console.WriteLine("-----------------------------------------");

            ////same example with a method
            //Log("Before event method thread");

            //EventThreadCallbackMethod(() => {
            //    Log("Event thread callback on complete");
            //});

            //Log("After event method thread");

            #endregion

            #region Sync VS ASync

            //var url = "https://www.google.com/";

            //Log("Before sync thread");

            //WebDownloadString(url);

            //Log("After sync thread");

            //Console.WriteLine("-----------------------------------------");

            //Log("Before async thread");

            //var downloadTask = WebDownloadStringAsync(url);

            //Log("After async thread");

            //downloadTask.Wait();

            //Console.WriteLine("-----------------------------------------");

            //var task = Task.Run(async () =>
            //{
            //    Log("Before async await thread");

            //    await WebDownloadStringAsync(url);

            //    Log("After async await thread");
            //});

            //task.Wait();

            #endregion

            #region ContinuewWith vs async await

            //Log("Before ContinueWith thread");

            //DoWorkAsync("ContinueWith").ContinueWith(t =>
            //{
            //    Log("After ContinueWith thread");
            //}).Wait();

            //Console.WriteLine("-----------------------------------------");

            //Task.Run(async () =>
            //{
            //    Log("Before async await thread");

            //    await DoWorkAsync("async await");

            //     Log("After async await thread");
            // }).Wait();

            #endregion

            #region Working with Parallel Class

            //var timer = new Stopwatch();

            //timer.Start();

            //for(var i = 0; i < 1000; i++)
            //{
            //    var s = Encrypt();
            //}

            //timer.Stop();

            //Console.WriteLine($"sequintal: {timer.Elapsed}");

            //timer.Start();

            //Parallel.For(0, 1000, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount}, 
            //    i => { var t = Encrypt();  } );

            //timer.Stop();

            //Console.WriteLine($"parallel: {timer.Elapsed}");

            #endregion

            #region Working with WhenAll 

            //Task.Run(async () =>
            //{
            //    Log("Before WhenAll thread");

            //    await WhenAllAsync();

            //    Log("After WhenAll thread");
            // }).Wait();

            #endregion

            #region Lock Mechanism and Thread synchronization

            //using Lock object
            //for (int i = 0; i < 5; i++)
            //{
            //    new Thread(LockDoWork).Start();
            //}

            //using Monitor
            //for (int i = 0; i < 5; i++)
            //{
            //    new Thread(MonitorDoWork).Start();
            //}

            //using Manual Reset Event
            //new Thread(Write).Start();

            //for (int i = 0; i < 5; i++)
            //{
            //    new Thread(Read).Start();
            //}

            //using Auto Reset Event
            //for (int i = 0; i < 5; i++)
            //{
            //    new Thread(WriteAutoReset).Start();
            //}

            //using Mutex
            //for (int i = 0; i < 5; i++)
            //{
            //    new Thread(WriteMutex).Start();
            //}

            //using Semaphore
            for (int i = 0; i < 5; i++)
            {
                new Thread(WriteSemaphore).Start();
            }

            #endregion

            Console.ReadLine();
        }

        public static void Write()
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} Writing…");
            //setting to false
            _mre.Reset();
            Thread.Sleep(5000);
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} Writing completed…");
            //setting to true
            _mre.Set();
        }

        public static void WriteAutoReset()
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} Waiting…");
            _are.WaitOne();
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} Writing…");
            Thread.Sleep(5000);
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} Writing completed…");
            _are.Set();
        }

        public static void WriteMutex()
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} Waiting…");
            _mutex.WaitOne();
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} Writing…");
            Thread.Sleep(5000);
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} Writing completed…");
            _mutex.ReleaseMutex();
        }

        public static void WriteSemaphore()
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} Waiting…");
            _semaphore.WaitOne();
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} Writing…");
            Thread.Sleep(5000);
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} Writing completed…");
            _semaphore.Release();
        }

        public static void Read()
        {
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} Waiting…");
            _mre.WaitOne();
            Thread.Sleep(2000);
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} Reading completed…");
            _mre.Set();
        }

        public static void LockDoWork()
        {
            lock (_locker)
            {
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} starting…");
                Thread.Sleep(2000);
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} completed…");
            }
        }

        public static void MonitorDoWork()
        {
            try
            {
                Monitor.Enter(_locker);
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} starting…");
                Thread.Sleep(2000);
                throw new Exception();
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} completed…");
            }
            catch (Exception e)
            {
                //log..
            }
            finally
            {
                Monitor.Exit(_locker);
            }
        }

        private static async Task WhenAllAsync()
        {
            var t1 = Task.Run(async () =>
            {
                await Task.Delay(3000);
                Console.WriteLine("task 1");
            });
            var t2 = Task.Run(() =>
            {
                Console.WriteLine("task 2");
            });
            var t3 = Task.Run(() =>
            {
                Console.WriteLine("task 3");
            });


            //waits for all the task to complete
            await Task.WhenAll(new Task[] { t1, t2, t3});
        }

        private static string Encrypt(string inputString = "string to encrypt")
        {
            var result = "";
            using (Rijndael crypt = Rijndael.Create())
            {
                crypt.GenerateKey();
                crypt.GenerateIV();

                ICryptoTransform transformer = crypt.CreateEncryptor();

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, transformer, CryptoStreamMode.Write))
                    {
                        using (var wr = new StreamWriter(cs))
                        {
                            wr.Write(inputString);
                        }

                        result = System.Text.Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }

            return result;
        }

        private static void WebDownloadString(string url)
        {
            var webClient = new WebClient();

            var result = webClient.DownloadString(new Uri(url));

            Log($"Download {url}. {result.Substring(0, 10)}");
        }

        private static async Task WebDownloadStringAsync(string url)
        {
            var webClient = new WebClient();

            var result = await webClient.DownloadStringTaskAsync(new Uri(url));

            Log($"Download {url}. {result.Substring(0, 10)}");
        }

        private static void EventThreadCallbackMethod(Action completed)
        {
            new Thread(() => {
                Log("Inside event method thread");
                Thread.Sleep(500);

                completed();
            }).Start();
        }

        private static async Task DoWorkAsync(string forWho)
        {
            Log($"Doing work for {forWho}");

            await Task.Run(async () =>
            {
                Log($"Doing Work on inner thread for {forWho}");

                await Task.Delay(500);

                Log($"Doe Work on inner thread for {forWho}");
            });

            Log($"Done work for {forWho}");
        }

        private static void Log(string message)
        {
            Console.WriteLine($"{message} [{Thread.CurrentThread.ManagedThreadId}]");
        }
    }
}
