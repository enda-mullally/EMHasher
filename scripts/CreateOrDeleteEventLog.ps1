$create = $true

if ($create -eq $true)
{
    # Define your custom log name and source
    $logName = "Enda Mullally"
    $source = "EM Hasher"

    # Check if the log exists, create if needed
    if (-not [System.Diagnostics.EventLog]::Exists($logName)) {
        New-EventLog -LogName $logName -Source $source
        Write-Host "Created log '$logName' with source '$source'."
    }
    elseif (-not [System.Diagnostics.EventLog]::SourceExists($source)) {
        [System.Diagnostics.EventLog]::CreateEventSource($source, $logName)
        Write-Host "Created source '$source' under existing log '$logName'."
    }
    else {
        Write-Host "Source '$source' already exists under log '$logName'."
    }

    Write-EventLog -LogName "Enda Mullally" -Source "EM Hasher" -EntryType Information -EventId 1001 -Message "Test message in custom log"
} else
{
    # CLEANUP #

    # Check if the log exists
    if ([System.Diagnostics.EventLog]::Exists($logName)) {
         Delete the custom log and all associated sources
        [System.Diagnostics.EventLog]::Delete($logName)
        Write-Host "Deleted event log '$logName' and all associated sources."
    } else {
        Write-Host "Event log '$logName' does not exist."
    }

    Stop-Service eventlog
    Start-Service eventlog
}
