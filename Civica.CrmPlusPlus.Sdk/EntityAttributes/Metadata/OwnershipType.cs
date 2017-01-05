namespace Civica.CrmPlusPlus.Sdk.EntityAttributes.Metadata
{
    public enum OwnershipType
    {
        None = 0,
        UserOwned = 1,
        TeamOwned = 2,
        BusinessOwned = 4,
        OrganizationOwned = 8,
        BusinessParented = 16
    }
}
