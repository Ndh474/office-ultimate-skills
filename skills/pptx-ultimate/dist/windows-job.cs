// Windows 10+ x64 owned-process helper. Stock .NET Framework only.
// No shell, breakaway flags, named jobs, or changes to host policy.
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Web.Script.Serialization;

public static class PptxWindowsJob {
    const uint KILL_ON_CLOSE=0x2000, WAIT_OBJECT_0=0, WAIT_TIMEOUT=258;
    [StructLayout(LayoutKind.Sequential)] struct SecurityAttributes { public int length; public IntPtr descriptor; [MarshalAs(UnmanagedType.Bool)] public bool inherit; }
    [StructLayout(LayoutKind.Sequential)] struct BasicLimit { public long processTime,jobTime; public uint flags; public UIntPtr minWorking,maxWorking; public uint activeLimit; public UIntPtr affinity; public uint priority,scheduling; }
    [StructLayout(LayoutKind.Sequential)] struct IoCounters { public ulong readOps,writeOps,otherOps,readBytes,writeBytes,otherBytes; }
    [StructLayout(LayoutKind.Sequential)] struct ExtendedLimit { public BasicLimit basic; public IoCounters io; public UIntPtr processMemory,jobMemory,peakProcess,peakJob; }
    [StructLayout(LayoutKind.Sequential)] struct Accounting { public long user,kernel,periodUser,periodKernel; public uint faults,total,active,terminated; }
    [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Unicode)] struct StartupInfo { public int cb; public string reserved,desktop,title; public uint x,y,xSize,ySize,xChars,yChars,fill,flags; public ushort show,reservedCount; public IntPtr reservedBytes,input,output,error; }
    [StructLayout(LayoutKind.Sequential)] struct StartupInfoEx { public StartupInfo startup; public IntPtr attributes; }
    [StructLayout(LayoutKind.Sequential)] struct ProcessInfo { public IntPtr process,thread; public uint processId,threadId; }
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode,SetLastError=true)] static extern IntPtr CreateJobObject(IntPtr attributes,string name);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool SetInformationJobObject(IntPtr job,int type,ref ExtendedLimit info,uint size);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool QueryInformationJobObject(IntPtr job,int type,out Accounting info,uint size,IntPtr returned);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool TerminateJobObject(IntPtr job,uint code);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool IsProcessInJob(IntPtr process,IntPtr job,out bool belongs);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool InitializeProcThreadAttributeList(IntPtr list,int count,int flags,ref IntPtr size);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool UpdateProcThreadAttribute(IntPtr list,uint flags,IntPtr attribute,IntPtr value,IntPtr size,IntPtr previous,IntPtr returned);
    [DllImport("kernel32.dll")] static extern void DeleteProcThreadAttributeList(IntPtr list);
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode,ExactSpelling=true,SetLastError=true)] static extern bool CreateProcessW(string app,StringBuilder command,IntPtr processSecurity,IntPtr threadSecurity,bool inherit,uint flags,IntPtr environment,string directory,ref StartupInfoEx startup,out ProcessInfo process);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool CreatePipe(out IntPtr read,out IntPtr write,ref SecurityAttributes security,uint size);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool SetHandleInformation(IntPtr handle,uint mask,uint flags);
    [DllImport("kernel32.dll",CharSet=CharSet.Unicode,ExactSpelling=true,SetLastError=true)] static extern IntPtr CreateFileW(string path,uint access,uint share,ref SecurityAttributes security,uint disposition,uint flags,IntPtr template);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool ReadFile(IntPtr file,[Out] byte[] buffer,uint length,out uint read,IntPtr overlapped);
    [DllImport("kernel32.dll",SetLastError=true)] static extern uint ResumeThread(IntPtr thread);
    [DllImport("kernel32.dll",SetLastError=true)] static extern uint WaitForSingleObject(IntPtr handle,uint milliseconds);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool TerminateProcess(IntPtr process,uint code);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool GetExitCodeProcess(IntPtr process,out uint code);
    [DllImport("kernel32.dll",SetLastError=true)] static extern IntPtr OpenProcess(uint access,bool inherit,int processId);
    [DllImport("kernel32.dll",SetLastError=true)] static extern bool CloseHandle(IntPtr handle);
    static volatile bool cancelled,overflow,readFailed;
    static long totalOutput;
    static int limit;
    sealed class NativeFailure:Exception { public string code; public int win32; public NativeFailure(string code,bool native) { this.code=code;win32=native?Marshal.GetLastWin32Error():0; } }
    static void Require(bool condition,string code) { if(!condition)throw new NativeFailure(code,true); }
    static void Close(ref IntPtr handle) { if(handle!=IntPtr.Zero&&handle!=new IntPtr(-1)){CloseHandle(handle);handle=IntPtr.Zero;} }
    static string Quote(string value) {
        StringBuilder result=new StringBuilder("\"");int slashes=0;
        foreach(char c in value){if(c=='\\'){slashes++;continue;}if(c=='"'){result.Append('\\',slashes*2+1);result.Append('"');}else{result.Append('\\',slashes);result.Append(c);}slashes=0;}
        result.Append('\\',slashes*2);return result.Append('"').ToString();
    }
    static Thread Pump(IntPtr handle,MemoryStream sink) {
        Thread thread=new Thread(delegate(){byte[] buffer=new byte[8192];uint count;
            while(true){if(!ReadFile(handle,buffer,(uint)buffer.Length,out count,IntPtr.Zero)){if(Marshal.GetLastWin32Error()!=109)readFailed=true;break;}if(count==0)break;
                if(Interlocked.Add(ref totalOutput,count)>limit){overflow=true;continue;}sink.Write(buffer,0,(int)count);
            }
        });thread.IsBackground=true;thread.Start();return thread;
    }
    static uint Active(IntPtr job) {Accounting accounting;Require(QueryInformationJobObject(job,1,out accounting,(uint)Marshal.SizeOf(typeof(Accounting)),IntPtr.Zero),"NATIVE_JOB_QUERY_FAILED");return accounting.active;}
    static bool EmptyJob(IntPtr job) {
        if(Active(job)>0)Require(TerminateJobObject(job,0xEC),"NATIVE_CLEANUP_FAILED");
        Stopwatch clock=Stopwatch.StartNew();while(clock.ElapsedMilliseconds<3000){if(Active(job)==0)return true;Thread.Sleep(10);}return false;
    }
    public static int Main(string[] arguments) {
        IntPtr job=IntPtr.Zero,parent=IntPtr.Zero,outRead=IntPtr.Zero,outWrite=IntPtr.Zero,errRead=IntPtr.Zero,errWrite=IntPtr.Zero,input=IntPtr.Zero,attributes=IntPtr.Zero,handles=IntPtr.Zero,jobs=IntPtr.Zero;
        bool attributesInitialized=false,jobEmpty=true,pipesComplete=true,membershipVerified=false,exactProcessStopped=true;ProcessInfo process=new ProcessInfo();Thread outThread=null,errThread=null;
        MemoryStream output=new MemoryStream(),error=new MemoryStream();string code=null,cleanupCode=null;int nativeError=0;uint exitCode=0;Stopwatch clock=Stopwatch.StartNew();
        JavaScriptSerializer json=new JavaScriptSerializer();json.MaxJsonLength=2*1024*1024;json.RecursionLimit=16;
        try {
            if(arguments.Length!=1||new FileInfo(arguments[0]).Length>1024*1024)throw new NativeFailure("NATIVE_HELPER_REQUEST_INVALID",false);
            Dictionary<string,object> request=json.Deserialize<Dictionary<string,object>>(File.ReadAllText(arguments[0],Encoding.UTF8));
            string executable=(string)request["executable"],cwd=(string)request["cwd"];
            IList args=request["args"] as IList;if(args==null)throw new NativeFailure("NATIVE_HELPER_REQUEST_INVALID",false);int timeout=Convert.ToInt32(request["timeoutMs"]);limit=Convert.ToInt32(request["maxOutputBytes"]);int parentId=Convert.ToInt32(request["parentPid"]);
            if(!Path.IsPathRooted(executable)||!Path.IsPathRooted(cwd)||timeout<1||timeout>600000||limit<1||limit>1048576||args.Count>1024)throw new NativeFailure("NATIVE_HELPER_REQUEST_INVALID",false);
            StringBuilder command=new StringBuilder(Quote(executable));foreach(object arg in args){string value=arg as string;if(value==null||value.IndexOf('\0')>=0)throw new NativeFailure("NATIVE_HELPER_REQUEST_INVALID",false);command.Append(' ').Append(Quote(value));}
            if(command.Length>=32767)throw new NativeFailure("NATIVE_HELPER_REQUEST_INVALID",false);
            parent=OpenProcess(0x100000,false,parentId);Require(parent!=IntPtr.Zero,"NATIVE_PARENT_UNAVAILABLE");
            Thread cancelThread=new Thread(delegate(){try{Console.OpenStandardInput().ReadByte();}catch{}cancelled=true;});cancelThread.IsBackground=true;cancelThread.Start();
            job=CreateJobObject(IntPtr.Zero,null);Require(job!=IntPtr.Zero,"NATIVE_JOB_UNAVAILABLE");
            ExtendedLimit limits=new ExtendedLimit();limits.basic.flags=KILL_ON_CLOSE;Require(SetInformationJobObject(job,9,ref limits,(uint)Marshal.SizeOf(typeof(ExtendedLimit))),"NATIVE_JOB_UNAVAILABLE");
            SecurityAttributes security=new SecurityAttributes();security.length=Marshal.SizeOf(typeof(SecurityAttributes));security.inherit=true;
            Require(CreatePipe(out outRead,out outWrite,ref security,0),"NATIVE_PIPE_FAILED");Require(SetHandleInformation(outRead,1,0),"NATIVE_PIPE_FAILED");
            Require(CreatePipe(out errRead,out errWrite,ref security,0),"NATIVE_PIPE_FAILED");Require(SetHandleInformation(errRead,1,0),"NATIVE_PIPE_FAILED");
            input=CreateFileW("NUL",0x80000000,3,ref security,3,0x80,IntPtr.Zero);Require(input!=new IntPtr(-1),"NATIVE_PIPE_FAILED");
            IntPtr size=IntPtr.Zero;InitializeProcThreadAttributeList(IntPtr.Zero,2,0,ref size);attributes=Marshal.AllocHGlobal(size);Require(InitializeProcThreadAttributeList(attributes,2,0,ref size),"NATIVE_JOB_UNAVAILABLE");attributesInitialized=true;
            handles=Marshal.AllocHGlobal(IntPtr.Size*3);Marshal.WriteIntPtr(handles,0,input);Marshal.WriteIntPtr(handles,IntPtr.Size,outWrite);Marshal.WriteIntPtr(handles,IntPtr.Size*2,errWrite);
            Require(UpdateProcThreadAttribute(attributes,0,new IntPtr(0x20002),handles,new IntPtr(IntPtr.Size*3),IntPtr.Zero,IntPtr.Zero),"NATIVE_PIPE_FAILED");
            jobs=Marshal.AllocHGlobal(IntPtr.Size);Marshal.WriteIntPtr(jobs,job);
            // Atomic assignment at creation (Windows10+), plus suspended start,
            // avoids both child-start and helper-death-before-assignment races.
            Require(UpdateProcThreadAttribute(attributes,0,new IntPtr(0x2000D),jobs,new IntPtr(IntPtr.Size),IntPtr.Zero,IntPtr.Zero),"NATIVE_JOB_UNAVAILABLE");
            StartupInfoEx startup=new StartupInfoEx();startup.startup.cb=Marshal.SizeOf(typeof(StartupInfoEx));startup.startup.flags=0x100;startup.startup.input=input;startup.startup.output=outWrite;startup.startup.error=errWrite;startup.attributes=attributes;
            if(cancelled||WaitForSingleObject(parent,0)==WAIT_OBJECT_0)throw new NativeFailure("NATIVE_CANCELLED",false);
            Require(CreateProcessW(executable,command,IntPtr.Zero,IntPtr.Zero,true,0x08080004,IntPtr.Zero,cwd,ref startup,out process),"NATIVE_PROCESS_CREATE_FAILED");
            exactProcessStopped=false;bool belongs;Require(IsProcessInJob(process.process,job,out belongs)&&belongs,"NATIVE_JOB_ASSIGNMENT_FAILED");membershipVerified=true;
            Close(ref outWrite);Close(ref errWrite);Close(ref input);
            outThread=Pump(outRead,output);errThread=Pump(errRead,error);
            if(cancelled||WaitForSingleObject(parent,0)==WAIT_OBJECT_0)throw new NativeFailure("NATIVE_CANCELLED",false);
            Require(ResumeThread(process.thread)!=0xFFFFFFFF,"NATIVE_PROCESS_RESUME_FAILED");
            Stopwatch execution=Stopwatch.StartNew();
            while(true){
                uint waited=WaitForSingleObject(process.process,20);if(waited!=WAIT_OBJECT_0&&waited!=WAIT_TIMEOUT)throw new NativeFailure("NATIVE_WAIT_FAILED",true);
                if(cancelled||WaitForSingleObject(parent,0)==WAIT_OBJECT_0){code="NATIVE_CANCELLED";break;}
                if(overflow){code="NATIVE_OUTPUT_LIMIT";break;}
                if(readFailed){code="NATIVE_PIPE_FAILED";break;}
                if(waited==WAIT_OBJECT_0){Require(GetExitCodeProcess(process.process,out exitCode),"NATIVE_WAIT_FAILED");if(exitCode!=0)code="NATIVE_PROCESS_FAILED";break;}
                if(execution.ElapsedMilliseconds>=timeout){code="NATIVE_TIMEOUT";break;}
            }
        }catch(NativeFailure failure){code=failure.code;nativeError=failure.win32;}
        catch{code="NATIVE_HELPER_FAILED";}
        finally{
            Close(ref outWrite);Close(ref errWrite);Close(ref input);
            if(process.process!=IntPtr.Zero&&!membershipVerified){try{TerminateProcess(process.process,0xEC);exactProcessStopped=WaitForSingleObject(process.process,1000)==WAIT_OBJECT_0;}catch{exactProcessStopped=false;}}
            if(job!=IntPtr.Zero){try{jobEmpty=EmptyJob(job);if(!jobEmpty)cleanupCode="NATIVE_CLEANUP_FAILED";}catch{jobEmpty=false;cleanupCode="NATIVE_CLEANUP_FAILED";}Close(ref job);}
            if(!membershipVerified&&!exactProcessStopped){jobEmpty=false;cleanupCode="NATIVE_CLEANUP_FAILED";}
            if(outThread!=null&&!outThread.Join(1000)){cleanupCode="NATIVE_CLEANUP_FAILED";pipesComplete=false;}if(errThread!=null&&!errThread.Join(1000)){cleanupCode="NATIVE_CLEANUP_FAILED";pipesComplete=false;}
            Close(ref outRead);Close(ref errRead);Close(ref process.thread);Close(ref process.process);Close(ref parent);
            if(attributesInitialized)DeleteProcThreadAttributeList(attributes);if(attributes!=IntPtr.Zero)Marshal.FreeHGlobal(attributes);if(handles!=IntPtr.Zero)Marshal.FreeHGlobal(handles);if(jobs!=IntPtr.Zero)Marshal.FreeHGlobal(jobs);
        }
        if(overflow&&code==null)code="NATIVE_OUTPUT_LIMIT";
        if(readFailed&&code==null)code="NATIVE_PIPE_FAILED";
        Dictionary<string,object> report=new Dictionary<string,object>{{"protocol",2},{"ok",code==null&&cleanupCode==null&&jobEmpty&&pipesComplete},{"code",code},{"cleanup_code",cleanupCode},{"pipes_complete",pipesComplete},{"exit_code",unchecked((int)exitCode)},{"win32_error",nativeError},{"job_empty",jobEmpty},{"stdout_base64",pipesComplete?Convert.ToBase64String(output.ToArray()):""},{"stderr_base64",pipesComplete?Convert.ToBase64String(error.ToArray()):""},{"duration_ms",clock.ElapsedMilliseconds}};
        try{Console.Out.WriteLine(json.Serialize(report));}catch{return 2;}return code==null&&cleanupCode==null&&jobEmpty&&pipesComplete?0:1;
    }
}
