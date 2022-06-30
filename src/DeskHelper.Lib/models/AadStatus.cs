using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DeskHelper.Lib.Models;

/// <summary>
/// Information about the Azure AD status of the computer.
/// </summary>
public class AadStatus
{
    public AadStatus()
    {
        _dsregcmdOutput = GetDsregcmdStatusOutput();

        if (_dsregcmdOutput is not null)
        {
            GetAzureAdJoinedStatus();
            GetEnterpriseJoinedStatus();
            GetDomainJoinedStatus();
            GetDeviceId();
            GetTenantId();
        }
    }

    /// <summary>
    /// Whether the computer is joined to an Azure AD tenant.
    /// </summary>
    public bool AzureAdJoined
    {
        get => _azureAdJoined;
    }

    /// <summary>
    /// Whether the computer is registered to an Azure AD tenant.
    /// </summary>
    public bool EnterpriseJoined
    {
        get => _enterpriseJoined;
    }

    /// <summary>
    /// Whether computer is joined to an AD domain.
    /// </summary>
    public bool DomainJoined
    {
        get => _domainJoined;
    }

    /// <summary>
    /// The Azure AD device ID for the computer.
    /// </summary>
    public string? DeviceId
    {
        get => _deviceId;
    }

    /// <summary>
    /// The Azure AD tenant ID for the computer.
    /// </summary>
    public string? TenantId
    {
        get => _tenantId;
    }

    private bool _azureAdJoined;
    private bool _enterpriseJoined;
    private bool _domainJoined;
    private string? _deviceId;
    private string? _tenantId;

    private string? _dsregcmdOutput;

    /// <summary>
    /// Get the output of the dsregcmd CLI tool.
    /// </summary>
    /// <returns>The output of the dsregcmd CLI tool.</returns>
    private static string? GetDsregcmdStatusOutput()
    {
        // Run the 'GetDsregcmdStatusOutputAsync()' method in a new thread and wait for it to finish.
        Task<string?> getDsregcmdStatusTask = Task.Run(async () => await GetDsregcmdStatusOutputAsync());
        getDsregcmdStatusTask.Wait();

        return getDsregcmdStatusTask.Result;
    }

    /// <summary>
    /// Get the output of the dsregcmd CLI tool.
    /// </summary>
    /// <returns>The output of the dsregcmd CLI tool.</returns>
    private static async Task<string?> GetDsregcmdStatusOutputAsync()
    {
        // Create a new 'Process' object.
        using Process process = new();

        // Set the process's 'StartInfo' properties.
        // Configured to run 'dsregcmd.exe /status', redirect the output, and hide the window.
        process.StartInfo = new()
        {
            FileName = "dsregcmd.exe",
            Arguments = "/status",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        // Start the process.
        process.Start();

        // Read the output of the process.
        string? dsregcmdStatus = await process.StandardOutput.ReadToEndAsync();

        // Wait for the process to exit.
        await process.WaitForExitAsync();

        return dsregcmdStatus;
    }

    /// <summary>
    /// Set whether the computer is joined to an Azure AD tenant.
    /// </summary>
    private void GetAzureAdJoinedStatus()
    {
        // Initialize the regex pattern to match the 'AzureAdJoined' line.
        // Then run the regex on the '_dsregcmdOutput' string.
        Regex joinedStatusRegex = new("AzureAdJoined : (?'joinStatus'YES|NO)");
        Match joinedStatusMatch = joinedStatusRegex.Match(_dsregcmdOutput!);

        if (joinedStatusMatch.Success)
        {
            // If the regex match was successful, then parse the value of the 'joinStatus' group.
            _azureAdJoined = joinedStatusMatch.Groups["joinStatus"].Value switch
            {
                "YES" => true, // If the value is 'YES', then the computer is joined to an Azure AD tenant.
                _ => false // Otherwise, the computer is not joined to an Azure AD tenant.
            };
        }
        else
        {
            // Otherwise set the '_azureAdJoined' property to 'false'.
            _azureAdJoined = false;
        }
    }

    /// <summary>
    /// Set whether the computer is registered to an Azure AD tenant.
    /// </summary>
    private void GetEnterpriseJoinedStatus()
    {
        // Initialize the regex pattern to match the 'EnterpriseJoined' line.
        // Then run the regex on the '_dsregcmdOutput' string.
        Regex joinedStatusRegex = new("EnterpriseJoined : (?'joinStatus'YES|NO)");
        Match joinedStatusMatch = joinedStatusRegex.Match(_dsregcmdOutput!);

        if (joinedStatusMatch.Success)
        {
            // If the regex match was successful, then parse the value of the 'joinStatus' group.
            _enterpriseJoined = joinedStatusMatch.Groups["joinStatus"].Value switch
            {
                "YES" => true, // If the value is 'YES', then the computer is registered to an Azure AD tenant.
                _ => false // Otherwise, the computer is not registered to an Azure AD tenant.
            };
        }
        else
        {
            // Otherwise set the '_enterpriseJoined' property to 'false'.
            _enterpriseJoined = false;
        }
    }

    /// <summary>
    /// Set whether the computer is joined to an AD domain.
    /// </summary>
    private void GetDomainJoinedStatus()
    {
        // Initialize the regex pattern to match the 'DomainJoined' line.
        // Then run the regex on the '_dsregcmdOutput' string.
        Regex joinedStatusRegex = new("DomainJoined : (?'joinStatus'YES|NO)");
        Match joinedStatusMatch = joinedStatusRegex.Match(_dsregcmdOutput!);

        if (joinedStatusMatch.Success)
        {
            // If the regex match was successful, then parse the value of the 'joinStatus' group.
            _domainJoined = joinedStatusMatch.Groups["joinStatus"].Value switch
            {
                "YES" => true, // If the value is 'YES', then the computer is joined to an AD domain.
                _ => false // Otherwise, the computer is not joined to an AD domain.
            };
        }
        else
        {
            // Otherwise set the '_domainJoined' property to 'false'.
            _domainJoined = false;
        }
    }

    /// <summary>
    /// Set the Azure AD device ID for the computer.
    /// </summary>
    private void GetDeviceId()
    {
        // Initialize the regex pattern to match the 'DeviceId' line.
        // Then run the regex on the '_dsregcmdOutput' string.
        Regex deviceIdRegex = new("DeviceId : (?'deviceId'(?:[A-Za-z0-9]+(?:-|)){5}|)");
        Match deviceIdMatch = deviceIdRegex.Match(_dsregcmdOutput!);

        if (deviceIdMatch.Success)
        {
            // If the regex match was successful, then parse the value of the 'deviceId' group.
            _deviceId = deviceIdMatch.Groups["deviceId"].Value;
        }
    }

    /// <summary>
    /// Set the Azure AD tenant ID for the computer.
    /// </summary>
    private void GetTenantId()
    {
        // Initialize the regex pattern to match the 'TenantId' line.
        // Then run the regex on the '_dsregcmdOutput' string.
        Regex tenantIdRegex = new("TenantId : (?'tenantId'(?:[A-Za-z0-9]+(?:-|)){5}|)");
        Match tenantIdMatch = tenantIdRegex.Match(_dsregcmdOutput!);

        if (tenantIdMatch.Success)
        {
            // If the regex match was successful, then parse the value of the 'tenantId' group.
            _tenantId = tenantIdMatch.Groups["tenantId"].Value;
        }
    }
}
