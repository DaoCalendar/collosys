namespace ColloSys.DataLayer.Components
{
    public interface IPolicyStatusComponent
    {
        bool IsActive { get; set; }
        bool IsInUse { get; set; }
    }
}