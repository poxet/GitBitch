try { 
$path = $env:chocolateyPackageFolder + "\lib\ClownCrew.GitBitch.Client.exe"
start $path
Write-ChocolateySuccess 'GitBitch'
} catch {
  Write-ChocolateyFailure 'GitBitch' "$($_.Exception.Message)"
  throw 
}