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
        using Process process = new();

        process.StartInfo = new()
        {
            FileName = "dsregcmd.exe",
            Arguments = "/status",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true
        };

        process.Start();

        string? dsregcmdStatus = await process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync();

        return dsregcmdStatus;
    }

    /// <summary>
    /// Set whether the computer is joined to an Azure AD tenant.
    /// </summary>
    private void GetAzureAdJoinedStatus()
    {
        Regex joinedStatusRegex = new("AzureAdJoined : (?'joinStatus'YES|NO)");

        Match joinedStatusMatch = joinedStatusRegex.Match(_dsregcmdOutput!);

        if (joinedStatusMatch.Success)
        {
            _azureAdJoined = joinedStatusMatch.Groups["joinStatus"].Value switch
            {
                "YES" => true,
                _ => false
            };
        }
        else
        {
            _azureAdJoined = false;
        }
    }

    /// <summary>
    /// Set whether the computer is registered to an Azure AD tenant.
    /// </summary>
    private void GetEnterpriseJoinedStatus()
    {
        Regex joinedStatusRegex = new("EnterpriseJoined : (?'joinStatus'YES|NO)");

        Match joinedStatusMatch = joinedStatusRegex.Match(_dsregcmdOutput!);

        if (joinedStatusMatch.Success)
        {
            _enterpriseJoined = joinedStatusMatch.Groups["joinStatus"].Value switch
            {
                "YES" => true,
                _ => false
            };
        }
        else
        {
            _enterpriseJoined = false;
        }
    }

    /// <summary>
    /// Set whether the computer is joined to an AD domain.
    /// </summary>
    private void GetDomainJoinedStatus()
    {
        Regex joinedStatusRegex = new("DomainJoined : (?'joinStatus'YES|NO)");

        Match joinedStatusMatch = joinedStatusRegex.Match(_dsregcmdOutput!);

        if (joinedStatusMatch.Success)
        {
            _domainJoined = joinedStatusMatch.Groups["joinStatus"].Value switch
            {
                "YES" => true,
                _ => false
            };
        }
        else
        {
            _domainJoined = false;
        }
    }

    /// <summary>
    /// Set the Azure AD device ID for the computer.
    /// </summary>
    private void GetDeviceId()
    {
        Regex deviceIdRegex = new("DeviceId : (?'deviceId'(?:[A-Za-z0-9]+(?:-|)){5}|)");

        Match deviceIdMatch = deviceIdRegex.Match(_dsregcmdOutput!);

        if (deviceIdMatch.Success)
        {
            _deviceId = deviceIdMatch.Groups["deviceId"].Value;
        }
    }

    /// <summary>
    /// Set the Azure AD tenant ID for the computer.
    /// </summary>
    private void GetTenantId()
    {
        Regex tenantIdRegex = new("TenantId : (?'tenantId'(?:[A-Za-z0-9]+(?:-|)){5}|)");

        Match tenantIdMatch = tenantIdRegex.Match(_dsregcmdOutput!);

        if (tenantIdMatch.Success)
        {
            _tenantId = tenantIdMatch.Groups["tenantId"].Value;
        }
    }
}
