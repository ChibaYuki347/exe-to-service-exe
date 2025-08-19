using Contoso.Common; Console.WriteLine("[Order] starting");
var clock=new SystemClock(); Console.WriteLine($"[Order] now: {clock.Now:O}");
await Retry.DoAsync(async ()=>{ await Task.Delay(50); Console.WriteLine("[Order] ok"); return 0;});
