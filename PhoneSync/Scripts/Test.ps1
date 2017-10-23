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

[Tuple]::Create("Hi", "Transferred", $selectedDrive)
[Tuple]::Create("Bye", "Exists", $destination)

#{@("hi", "Transferred", $selectedDrive)}.Invoke()
#{@("bye", "Exists", $destination)}.Invoke()
foreach($ignoreEntry in $ignore){
    #{@($ignoreEntry,"Ignored","blah")}.Invoke()
    [Tuple]::Create($ignoreEntry, "Ignored", "")
}