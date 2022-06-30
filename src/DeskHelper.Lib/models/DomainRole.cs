namespace DeskHelper.Lib.Models;

/// <summary>
/// The domain role of the computer.
/// </summary>
public enum DomainRole : ushort
{
    StandaloneWorkstation = 0,
    MemberWorkstation = 1,
    StandaloneServer = 2,
    MemberServer = 3,
    BackupDomainController = 4,
    PrimaryDomainController = 5
}