namespace PetBoarding_Api.Extensions;

/// <summary>
/// Enum pour définir les codes de statut HTTP de succès alternatifs
/// </summary>
public enum SuccessStatusCode
{
    /// <summary>
    /// 200 OK - Par défaut
    /// </summary>
    Ok = 200,
    
    /// <summary>
    /// 201 Created - Pour les créations
    /// </summary>
    Created = 201,
    
    /// <summary>
    /// 202 Accepted - Pour les opérations asynchrones
    /// </summary>
    Accepted = 202,
    
    /// <summary>
    /// 204 No Content - Pour les suppressions ou modifications sans contenu de retour
    /// </summary>
    NoContent = 204
}
