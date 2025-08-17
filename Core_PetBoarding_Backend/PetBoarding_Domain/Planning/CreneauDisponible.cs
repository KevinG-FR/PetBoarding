namespace PetBoarding_Domain.Planning;

/// <summary>
/// Value object représentant un créneau disponible pour une date donnée
/// </summary>
public sealed class CreneauDisponible : IEquatable<CreneauDisponible>
{
    public CreneauDisponible(DateTime date, int capaciteMax, int capaciteReservee = 0)
    {
        if (capaciteMax <= 0)
        {
            throw new ArgumentException("La capacité maximale doit être supérieure à 0", nameof(capaciteMax));
        }

        if (capaciteReservee < 0)
        {
            throw new ArgumentException("La capacité réservée ne peut pas être négative", nameof(capaciteReservee));
        }

        if (capaciteReservee > capaciteMax)
        {
            throw new ArgumentException("La capacité réservée ne peut pas dépasser la capacité maximale", nameof(capaciteReservee));
        }

        Date = date.Date; // On ne garde que la partie date
        CapaciteMax = capaciteMax;
        CapaciteReservee = capaciteReservee;
    }

    public DateTime Date { get; }
    public int CapaciteMax { get; }
    public int CapaciteReservee { get; private set; }

    public int CapaciteDisponible => CapaciteMax - CapaciteReservee;

    public bool EstDisponible(int quantiteDemandee = 1)
    {
        return CapaciteDisponible >= quantiteDemandee && Date.Date >= DateTime.Today;
    }

    public void Reserver(int quantite)
    {
        if (quantite <= 0)
        {
            throw new ArgumentException("La quantité à réserver doit être supérieure à 0", nameof(quantite));
        }

        if (!EstDisponible(quantite))
        {
            throw new InvalidOperationException($"Capacité insuffisante. Disponible: {CapaciteDisponible}, Demandée: {quantite}");
        }

        CapaciteReservee += quantite;
    }

    public void AnnulerReservation(int quantite)
    {
        if (quantite <= 0)
        {
            throw new ArgumentException("La quantité à annuler doit être supérieure à 0", nameof(quantite));
        }

        if (quantite > CapaciteReservee)
        {
            throw new InvalidOperationException($"Impossible d'annuler {quantite} réservations. Seulement {CapaciteReservee} réservations actives.");
        }

        CapaciteReservee -= quantite;
    }

    public CreneauDisponible AvecNouvelleCapacite(int nouvelleCapaciteMax)
    {
        return new CreneauDisponible(Date, nouvelleCapaciteMax, Math.Min(CapaciteReservee, nouvelleCapaciteMax));
    }

    // Implémentation de IEquatable
    public bool Equals(CreneauDisponible? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Date.Date == other.Date.Date;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as CreneauDisponible);
    }

    public override int GetHashCode()
    {
        return Date.Date.GetHashCode();
    }

    public static bool operator ==(CreneauDisponible? left, CreneauDisponible? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(CreneauDisponible? left, CreneauDisponible? right)
    {
        return !Equals(left, right);
    }

    public override string ToString()
    {
        return $"Créneau {Date:yyyy-MM-dd} - {CapaciteDisponible}/{CapaciteMax} disponible";
    }
}
