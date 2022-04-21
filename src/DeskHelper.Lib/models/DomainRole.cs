namespace DeskHelper.Lib.Models;

public enum DomainRole : ushort
{
    StandaloneWorkstation = 0,
    MemberWorkstation = 1,
    StandaloneServer = 2,
    MemberServer = 3,
    BackupDomainController = 4,
    PrimaryDomainController = 5
}