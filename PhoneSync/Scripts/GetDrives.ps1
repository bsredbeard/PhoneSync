# http://blogs.technet.com/b/heyscriptingguy/archive/2013/04/26/use-powershell-to-work-with-windows-explorer.aspx
$shell = New-Object -com Shell.Application
$folder = $shell.NameSpace(0x11)

# https://msdn.microsoft.com/en-us/library/windows/desktop/bb774096(v=vs.85).aspx
# ShellSpecialFolderConstants.ssfDRIVES == 0x11

$items = $folder.Items()
for ($i= 0; $i -lt $items.Count; $i++) {
    $items.Item($i).Name
}