# Get today's date
$today = Get-Date
$weekOfMonth = [math]::Ceiling($today.Day / 7)
$todayFormatted = $today.ToString("dd-MMM-yyyy")

# Get repo URL
$repoUrl = git remote get-url origin

# Commit message
$msg = "DCS (Week $weekOfMonth) ($todayFormatted)"

git add .
git commit -m "$msg"
git push

# Show repo URL
Write-Output "`nRepository URL: $repoUrl"

Read-Host "`nPress Enter to continue..."

