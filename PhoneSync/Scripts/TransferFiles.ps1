Param(
	[Parameter(Mandatory=$true)][string]$selectedDrive,
	[Parameter(Mandatory=$true)][string]$destination,
	[string[]]$ignoreList = @()
)

if(-not($removePrefix)){
	$removePrefix = "\$selectedDrive\Internal shared storage"
}

$ignore = {$ignoreList}.Invoke()
if($ignore.Count -lt 1){
    $ignore.Add("\DCIM\.thumbnails\")
    $ignore.Add("\Android\data\")
}

# http://blogs.technet.com/b/heyscriptingguy/archive/2013/04/26/use-powershell-to-work-with-windows-explorer.aspx
$shell = New-Object -com Shell.Application
$folder = $shell.NameSpace(0x11)

# https://msdn.microsoft.com/en-us/library/windows/desktop/bb774096(v=vs.85).aspx
# ShellSpecialFolderConstants.ssfDRIVES == 0x11

$items = $folder.Items()
$drives = $items | %{ $_.Name }
$choice = [Array]::IndexOf($drives, $selectedDrive);
if($choice -lt [int]0){
	Throw "The drive $selectedDrive was not available"
}

$android = $items.Item([int]$choice)
$root = $android.GetFolder()

# FolderItem versus FolderItems

$folders = New-Object PSCustomObject

Function Handle-File($file, [string]$path){
    if($path.IndexOf($removePrefix) -eq 0){
        $path = $path.Substring($removePrefix.Length)
    }
    $ignoreFile = $false
    foreach($i in $ignore){
        if($path.IndexOf($i) -eq 0){
            $ignoreFile = $true
            break
        }
    }
    if($ignoreFile){
        Write-Host "Ignoring $path"
        @($path,"Ignored","")
    } else {
        $floc = Join-Path $destination $path
        if(Test-Path $floc){
            @($path,"Exists",$floc)
            return
        }
        $dir = Split-Path -Path $floc -Parent
        
        $shellFolder = ""
        if(Get-Member -InputObject $folders -name $dir -MemberType Properties){
            $shellFolder = $folders | select -ExpandProperty $dir
        } else {
            New-Item -ItemType Directory -Force -Path $dir
            $shellFolder = $shell.NameSpace($dir)
            Add-Member -InputObject $folders -Name $dir -Value $shellFolder -MemberType NoteProperty
        }

        $shellFolder.CopyHere($file)

        Write-Host "File: $path, copied to $floc"
        @($path, "Transferred", "$floc")
    }
}

Function ExploreDevice($item, $parentPath) {
    $name = $item.Name
    
    if($item.Title){
        $name = $item.Title
    }

    $path = "$parentPath\$name"
    Write-Host
    Write-Host "Exploring $path"


    if($item.Count -gt 0){
        #Write-Host "$name has Count"
        for ($i = 0; $i -lt $item.Count) {
            ExploreDevice $item.items($i) $path
        }
    } else {
        if($item.Items){
            #Write-Host "$name has sub-items"
            $subItems = $item.Items()
            foreach($i in $subItems){
                if($i.IsFolder){
                    ExploreDevice $i.GetFolder() $path
                }else{
                    Handle-File $i "$path\$($i.Name)"
                }
            }
        } else {
            Handle-File $item $path
        }
    }
}

New-Item -ItemType Directory -Force -Path $destination
ExploreDevice $root ""