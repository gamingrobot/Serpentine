#https://stackoverflow.com/a/2611435/1161491
$Source = @"
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

public static class GacUtil {

    public static void InstallAssembly(string path, bool forceRefresh) {
        IAssemblyCache iac = null;
        CreateAssemblyCache(out iac, 0);
        try {
            uint flags = forceRefresh ? 2u : 1u;
            int hr = iac.InstallAssembly(flags, path, IntPtr.Zero);
            if (hr < 0) Marshal.ThrowExceptionForHR(hr);
        }
        finally {
            Marshal.FinalReleaseComObject(iac);
        }
    }

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("e707dcde-d1cd-11d2-bab9-00c04f8eceae")]
    internal interface IAssemblyCache {
        [PreserveSig]
        int UninstallAssembly(uint flags, [MarshalAs(UnmanagedType.LPWStr)] string assemblyName, IntPtr pvReserved, out uint pulDisposition);
        [PreserveSig]
        int QueryAssemblyInfo(uint dwFlags, [MarshalAs(UnmanagedType.LPWStr)] string pszAssemblyName, IntPtr pAsmInfo);
        [PreserveSig]
        int CreateAssemblyCacheItem(/* arguments omitted */);
        [PreserveSig]
        int CreateAssemblyScavenger(out object ppAsmScavenger);
        [PreserveSig]
        int InstallAssembly(uint dwFlags, [MarshalAs(UnmanagedType.LPWStr)] string pszManifestFilePath, IntPtr pvReserved);
    }

    [DllImport("mscorwks.dll", PreserveSig = false)]  // NOTE: use "clr.dll" in .NET 4+
    internal static extern void CreateAssemblyCache(out IAssemblyCache ppAsmCache, int reserved);
} 
"@

$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if(!$currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)){
    Write-Error "Install script must be run as Administrator"
    exit 1
}

#Try and add type, it will throw if it already exists
try { Add-Type -TypeDefinition $Source -Language CSharp; } catch {}

#InstallAssembly requires full path
$dllpath = "$PSScriptRoot\Serpentine.IISModule.dll"
[GacUtil]::InstallAssembly($dllpath, $false)

Write-Host "Installed $dllpath into GAC"

$fullName = [System.Reflection.AssemblyName]::GetAssemblyName($dllpath).FullName

$appCmdPath = "C:\windows\system32\inetsrv\"
& $appCmdPath\appcmd.exe add module /name:Serpentine.IISModule /type:"Serpentine.IISModule.SerpentineModule, $fullName" /preCondition:managedHandler,runtimeVersionv4.0