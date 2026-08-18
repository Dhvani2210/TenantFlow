namespace TenantFlow.Application.Common;

public enum ErrorType
{
    None,           
    NotFound,       
    Unauthorized,  
    Conflict,       
    Validation,
    Forbidden,
    ServerError
}