using System.Diagnostics;
using System.Runtime.InteropServices;
static async Task<int> RunServiceAsync(string name){
  var baseDir=AppContext.BaseDirectory;
  var exe=Path.GetFullPath(Path.Combine(baseDir,"..","Services",name,$"{name}.exe"));
  if(!File.Exists(exe)){ Console.Error.WriteLine($"not found: {exe}"); return -1; }
  var psi=new ProcessStartInfo(exe){WorkingDirectory=Path.GetDirectoryName(exe)!,UseShellExecute=false,RedirectStandardOutput=true,RedirectStandardError=true,CreateNoWindow=true};
  using var p=Process.Start(psi)!; var t1=p.StandardOutput.ReadToEndAsync(); var t2=p.StandardError.ReadToEndAsync(); await p.WaitForExitAsync();
  Console.WriteLine($"[{name}] exit={p.ExitCode}"); Console.WriteLine((await t1).Trim()); var e=(await t2).Trim(); if(e.Length>0) Console.Error.WriteLine(e); return p.ExitCode;
}
// pause 指定（--pause または 環境変数 PAUSE_ON_EXIT=1）で終了前に待機
var explicitPause = Array.Exists(args, a => string.Equals(a, "--pause", StringComparison.OrdinalIgnoreCase))
                || string.Equals(Environment.GetEnvironmentVariable("PAUSE_ON_EXIT"), "1", StringComparison.OrdinalIgnoreCase);
var explicitNoPause = Array.Exists(args, a => string.Equals(a, "--no-pause", StringComparison.OrdinalIgnoreCase));
var autoPause = IsLaunchedFromExplorer();
var pause = explicitNoPause ? false : (explicitPause || autoPause);

Console.WriteLine("Launcher: starting");
await RunServiceAsync("Order");
await RunServiceAsync("Inventory");
Console.WriteLine("Launcher: done");

if (pause)
{
  Console.WriteLine("Press Enter to exit...");
  Console.ReadLine();
}

static bool IsLaunchedFromExplorer()
{
  if (!OperatingSystem.IsWindows()) return false;
  try
  {
    var parentPid = GetParentProcessId(Environment.ProcessId);
    if (parentPid <= 0) return false;
    using var parent = Process.GetProcessById(parentPid);
    var name = parent.ProcessName;
    return string.Equals(name, "explorer", StringComparison.OrdinalIgnoreCase);
  }
  catch { return false; }
}

static int GetParentProcessId(int pid)
{
  const uint TH32CS_SNAPPROCESS = 0x00000002;
  IntPtr snapshot = Win32.CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
  if (snapshot == IntPtr.Zero || snapshot == (IntPtr)(-1)) return -1;
  try
  {
  Win32.PROCESSENTRY32 pe32 = new Win32.PROCESSENTRY32();
  pe32.dwSize = (uint)Marshal.SizeOf(typeof(Win32.PROCESSENTRY32));
    if (!Win32.Process32First(snapshot, ref pe32)) return -1;
    do
    {
      if (pe32.th32ProcessID == (uint)pid)
        return (int)pe32.th32ParentProcessID;
    }
    while (Win32.Process32Next(snapshot, ref pe32));
    return -1;
  }
  finally { Win32.CloseHandle(snapshot); }
}

static class Win32
{
  [DllImport("kernel32.dll", SetLastError = true)]
  public static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessId);

  [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
  public static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

  [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
  public static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

  [DllImport("kernel32.dll", SetLastError = true)]
  public static extern bool CloseHandle(IntPtr hObject);

  [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
  public struct PROCESSENTRY32
  {
    public uint dwSize;
    public uint cntUsage;
    public uint th32ProcessID;
    public IntPtr th32DefaultHeapID;
    public uint th32ModuleID;
    public uint cntThreads;
    public uint th32ParentProcessID;
    public int pcPriClassBase;
    public uint dwFlags;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string szExeFile;
  }
}
