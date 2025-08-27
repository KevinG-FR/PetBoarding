namespace PetBoarding_Domain.Pets;

/// <summary>
/// Types d'animaux supportés par l'application
/// Synchronisé avec l'enum PetType côté frontend Angular
/// </summary>
public enum PetType
{
    Chien = 1,
    Chat = 2,
    Oiseau = 3,
    Lapin = 4,
    Hamster = 5,
    Autre = 99
}