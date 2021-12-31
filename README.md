Threads:

Asynchronous is if you start something, and don’t want to wait while it’s happening.
It literally means to not occur at the same time.
This means not that our code return early, but rather it doesn’t sit there blocking the code while it waits (doesn’t block the thread).

Thread thread = new Thread();  Creates a new thread, quite expensive. 
thread.Sleep(1000);  suspends the thread for a given time (milliseconds).
thread.Join();  joins the thread to the app main thread.
thread.Start();  Starts the thread.
thread.Start(background = true);  the main thread can finish not dependent on background threads.
ThreadPool.QueueUserWorkItem(() => {});  Manages bunch of threads, once a thread job ends, recycles the thread for a new job.
Task.Run(() => {}); Creates a managed thread pool for a bunch of background threads.
async – If we define the main methos async, and add ’await’ to the Task.Run(), The main thread will run not dependent on the background threads but when it ends, it will wait till the task.Run() thread pool threads will end. 
Task.Delay(1000);  suspends the thread or bunch of threads for a given time (milliseconds).
Task.Run(() => {MyButton.Diaptcher.Invoke(() => {MyButton.Content = “Loggen In”}); });  MyButton is created on the main thread, which means, when we are on the Task context (a new thread) we don’t have access to MyButton. Every WPF control has a dispatcher. That’s why we use the dispatcher – to join to the main thread context.
Task.Result  best practice is await Task. Avoid Task.Result – only in rare occasion to avoid a hraed deadlock.
If we want the main thread to await till the Task is ended, we will add ‘await’ before the Task.Run(), 
and async to the caller function.
Avoid!!! ‘async void’  Best Practice!!! ‘async Task’ 
await Task.Run(async () = > { var html = await webclient.GetStringAsyn(“http://link.com”)}).ConfigureAwait(false); - ??? 

Tasks:

Issues with threads:

1.	Expensive to make – (working with a ThreadPool can resolve this)
2.	Not natural to be able to resume after a thread has finished to do something related to the thread that created it.

Issue 2 is still an issue for threads and is one reason why Tasks where made. In order to resume to some asynchronous operation that has occurred we could with a thread:

1.	Block your code waiting for it (no better than just doing it in the same thread)
2.	Constantly poll for completion, waiting for a bool flag to say “done” (inefficient, slow)
3.	Event-base callbacks (lose the calling thread on callback, and causes nesting)

What is a Task?
A Task encapsulates the promise of an operation completing in the future.

Task, Async and Await – resolves all threads issues.
The point is to allow easy and clean asynchronous code to be written without complex or messy code.
Async and await are always used together. A method or lambda tagged with async can then await any task.

When you await something, the thread which called the await is free to then return to what it was doing, while in parallel the task inside the await is now running on another thread. Once the task is done, it returns either to the original calling thread, or carries on, on another thread to do the work that codes after the await.

Task.Wait();  wait for a task to complete in a method that is not async. Wait is a synchronous and Await is asynchronous.
Also can use GetAwaiter(), GetResult().

Code inside a function that returns a Task runs its code on the caller’s thread up until the first line that call await. At this point in time:

1.	The current thread executing your code is released (making your code asynchronous). That means from a normal point of view, your function has returned at this point (it has returned the Task object – a promise).
2.	Once the task you have awaited completes, your method should appear to continue from where it left off, as if it never returned from the method, so resume the line below the await.

To achieve this, C# at the point of reaching the await call:

1.	Stores all local variables in the scope of the method
2.	Stores the parameters of your method
3.	Stores the ‘this’ variable to store all class-level variables
4.	Stores all contexts (Execution, Security, Call)


Async return types: You can only return void, Task or Task<T> for a method marked as async, as the method not complete when it returns, so no other result is valid.

SynchronizationContext

As you call a method that returns a ‘Task’ and uses ‘async’, inside the method all code, up until the first ‘await’ statement, is runing like normal function in the thread that called it.

Once it hits the ‘await’ the function returns the ‘Task’ to the caller and does its work that’s inside the ‘await’ call on a different thread (new thread or existing).

Once it done, and effectively “after” the await line, execution returns to a certain thread.

That thread is determined by first checking if the thread has a synchronization context and if it does it asks that what thread to return to. For UI threads this will return work to the UI thread itself.
For Normal threads that have no synchronization context, the code after the ‘await’ typically, but not always, continues on the same thread that the inner work was being done on, but has no requirement to resume on any specific thread.

Typically, if you use ‘ContinueWith’ instead of ‘await’, the code inside ‘ContinueWith’’
Runs on a different thread than the inner task was running on and using ‘await’ typically continues on the same thread.

This also means after every ‘await’ the next line is typically on a new thread if there is no synchronization context.

An exception is if you use ‘ConfigureAwait(false)’ then the SynchronizationContext is totally ignored and the resuming thread is treated as if there were no context.

Resuming on the original thread via the SynchronizationContext is an expensive thing (takes time) and so if you choose to not care about resuming on the same thread and want to save time you can use ‘ConfigureAwait(false)’ to remove that overhead. 

The Parallel Class 

Has the Parallel.For and Parallel.ForEach methods which allows you to do parallel work.
Which means it is more efficient than regular multithreading work.
There is a difference between Parallelism and Concurrency. 
Parallelism using different cores of the processor to do multiple tasks on different threads on the same time.
Concurrency do multiple tasks on a single core by switching context of threads.

WhenAll() and WaitAll()
Both wait for all tasks in array to complete before continuing to do the rest of the code after it.
WaitAll:
-	Waits for all the tasks to complete
-	Blocking operation
-	Tasks are passed as array
-	Can set time out
-	Can send cancellation token 
WhenAll:
-	Creates a task which waits for all the tasks to complete
-	Non Blocking / Blocking can be configured
-	Tasks are passed as array or IEnumerable<Task>
-	Cannot set timeout
-	Cannot send cancellation token 

