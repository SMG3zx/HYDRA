# Load configuration from HYDRA.config
try {
    $config = Get-Content -Raw -Path "HYDRA.config" | ConvertFrom-Json
}
catch {
    Write-Host "Error loading configuration: $_"
    exit
}

# Display the main menu
function Show-MainMenu {
    Write-Host "HYDRA Version : $($config.Version)"
    Write-Host "Author : SMG3zx"
    Write-Host "Last Updated : $($config.LastModifiedDate)"
    Write-Host "============================================="
    Write-Host "HYDRA Main Menu"
    Write-Host "1. Clean Profiles"
    Write-Host "2. Settings"
    Write-Host "3. Exit"
    $choice = Read-Host "Please select an option"
    return $choice
}

# Display the settings menu
function Show-SettingsMenu {
    Write-Host "HYDRA Settings"
    Write-Host "1. Change Client List Path ($($config.ClientsPath))"
    Write-Host "2. Change Log Location ($($config.LogPath))"
    Write-Host "3. Change Skip List ($($config.SkipProfiles -join ', '))"
    Write-Host "4. Change Age Setting ($($config.AgeSetting) days)"
    Write-Host "5. Change Throttle Limit ($($config.ThrottleLimit))"
    $settingsChoice = Read-Host "Please select an option"

    switch ($settingsChoice) {
        1 {
            $newClientListPath = Read-Host "Enter new Client List Path"
            if (Test-Path $newClientListPath) {
                $config.ClientsPath = $newClientListPath
                Write-Host "Client List Path updated to $($config.ClientsPath)"
            }
            else {
                Write-Host "Invalid path. No changes made."
            }
        }
        2 {
            $newLogLocation = Read-Host "Enter new Log Location"
            if (Test-Path (Split-Path $newLogLocation -Parent)) {
                $config.LogPath = $newLogLocation
                Write-Host "Log Location updated to $($config.LogPath)"
            }
            else {
                Write-Host "Invalid path. No changes made."
            }
        }
        3 {
            $newSkipList = Read-Host "Enter new Skip List (separate items with spaces)"
            $config.SkipProfiles = $newSkipList -split ' '
            Write-Host "Skip List updated to $($config.SkipProfiles -join ', ')"
        }
        4 {
            $newAgeSetting = Read-Host "Enter new Age Setting (in days)"
            $config.AgeSetting = [int]$newAgeSetting
            Write-Host "Age Setting updated to $($config.AgeSetting) days"
        }
        5 {
            $newThrottleLimit = Read-Host "Enter new Throttle Limit"
            $config.ThrottleLimit = [int]$newThrottleLimit
            Write-Host "Throttle Limit updated to $($config.ThrottleLimit)"
        }
        default {
            Write-Host "Invalid option. No changes made."
        }
    }
    return $settingsChoice
}

# Main logic
do {
    $userChoice = Show-MainMenu

    switch ($userChoice) {
        1 {
            try {
                [string[]]$clients = Get-Content $config.ClientsPath
                [string]$logLocation = $config.LogPath
                [string]$skipListPattern = ($config.SkipProfiles -join '|')
                [int]$ageSettingDays = $config.AgeSetting
            }
            catch {
                Write-Host "Error loading client list or configuration: $_"
                exit
            }

            # Process each client in parallel
            $clients | ForEach-Object -Parallel {
                $computer = $_
                try {
                    Add-Content -Path $using:logLocation -Value "Started Processing $computer"
                    
                    # Retrieve user profiles
                    $profiles = Get-ChildItem -Path "\\$computer\C$\Users" -Directory
                    $userAges = @()

                    # Calculate age of each profile
                    foreach ($profilePath in $profiles) {
                        try {
                            $profileFiles = Get-ChildItem -Path $profilePath -File -Recurse -ErrorAction Stop
                            $newestFile = $profileFiles | Sort-Object LastWriteTime -Descending | Select-Object -First 1
                            $ageDays = (New-TimeSpan -Start $newestFile.LastWriteTime -End (Get-Date)).Days
                            # Store profile information as a PSCustomObject so
                            # that it exposes properties we can access later
                            $userAges += [PSCustomObject]@{ Profile = $profilePath; Age = $ageDays }
                        }
                        catch {
                            continue
                        }
                    }

                    # Calculate initial total size of user profiles
                    try {
                        $initialTotalSizeGb = (Get-ChildItem -Path "\\$computer\C$\Users" -Recurse -File | Measure-Object -Property Length -Sum).Sum / 1GB
                        Add-Content -Path $using:logLocation -Value "Initial total size: $($initialTotalSizeGb) GB on computer: $computer"
                    }
                    catch {
                        Write-Host "Error calculating initial total size: $_"
                        continue
                    }
                            
                    # Process each profile
                    foreach ($profile in $profiles) {
                        if ($profile.Name -match $using:skipListPattern) {
                            Add-Content -Path $using:logLocation -Value "Skipping $profile on computer: $computer"
                        }
                        else {
                            $ageInfo = $userAges | Where-Object { $_.Profile -eq $profile } 

                            if ($ageInfo.Age -ge $ageSettingDays) {
                                Add-Content -Path $using:logLocation -Value "Deleting $($ageInfo.Profile) for being $($ageInfo.Age) days without modification on computer: $computer"
                                try {
                                    if (Test-Path $profile) {
                                        Remove-Item -Recurse $profile -ErrorAction SilentlyContinue
                                    }
                                }
                                catch {
                                    continue
                                }
                            }
                            else {
                                Add-Content -Path $using:logLocation -Value "Passing on $($ageInfo.Profile.Name) for being $($ageInfo.Age) days without modification on computer: $computer"
                            }
                        }
                    }

                    # Calculate space freed after processing
                    try {
                        $newTotalSizeGb = (Get-ChildItem -Path "\\$computer\C$\Users" -Recurse -File | Measure-Object -Property Length -Sum).Sum / 1GB
                        $spaceFreedGb = $initialTotalSizeGb - $newTotalSizeGb
                        Add-Content -Path $using:logLocation -Value "Space freed: $($spaceFreedGb) GB on computer: $computer"
                        Add-Content -Path $using:logLocation -Value "Finished with $computer : $(Get-Date)"
                    }
                    catch {
                        Write-Host "Error calculating space freed: $_"
                    }
                }
                catch {
                    Write-Host "Error processing computer ${computer} : $_"
                }
            } -ThrottleLimit $config.ThrottleLimit
        }
        2 {
            Show-SettingsMenu
        }
        3 {
            Write-Host "Exiting..."
        }
        default {
            Write-Host "Invalid option, please try again."
        }
    }
} while ($userChoice -ne 3)
