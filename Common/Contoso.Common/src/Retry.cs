namespace Contoso.Common; using System; using System.Threading.Tasks;
public static class Retry {
  public static async Task<T> DoAsync<T>(Func<Task<T>> action,int maxAttempts=3,TimeSpan? delay=null){
    delay??=TimeSpan.FromMilliseconds(100); Exception? last=null;
    for(int i=0;i<maxAttempts;i++){ try{ return await action().ConfigureAwait(false); }
      catch(Exception ex){ last=ex; if(i<maxAttempts-1) await Task.Delay(delay.Value).ConfigureAwait(false);}}
    throw last!; } }