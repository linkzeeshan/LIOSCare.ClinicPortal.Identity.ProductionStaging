namespace LIOSCare.ClinicPortal.Web.Exceptions;

public sealed class DoctorProfileNotFoundException : InvalidOperationException
{
    public Guid UserId { get; }
    
    public DoctorProfileNotFoundException(Guid userId) 
        : base($"Doctor profile not found for user ID: {userId}. Please ensure the user has a valid doctor profile assigned.")
    {
        UserId = userId;
    }
    
    public DoctorProfileNotFoundException(Guid userId, string message) 
        : base(message)
    {
        UserId = userId;
    }
    
    public DoctorProfileNotFoundException(Guid userId, string message, Exception innerException) 
        : base(message, innerException)
    {
        UserId = userId;
    }
}
