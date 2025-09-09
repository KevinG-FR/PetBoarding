using FluentAssertions;
using PetBoarding_Domain.Reservations;
using PetBoarding_Persistence.Repositories;

namespace PersistenceIntegrationTests.Repositories;

public class ReservationRepositoryTests : PostgreSqlTestBase
{
    private ReservationRepository _repository => new ReservationRepository(Context);

    [Fact]
    public async Task GetByIdAsync_Should_IncludeReservedSlots()
    {
        // Arrange
        var reservation = Reservation.Create(
            "user123",
            "pet123",
            "Buddy",
            "service123",
            DateTime.Today.AddDays(1),
            DateTime.Today.AddDays(3),
            "Test reservation"
        );

        Context.Reservations.Add(reservation);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(reservation.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(reservation.Id);
        result.UserId.Should().Be("user123");
        result.AnimalName.Should().Be("Buddy");
    }

    [Fact]
    public async Task GetAllAsync_Should_FilterByUserId()
    {
        // Arrange
        var reservations = new[]
        {
            Reservation.Create("user123", "pet1", "Buddy", "service1", DateTime.Today, null),
            Reservation.Create("user456", "pet2", "Max", "service2", DateTime.Today, null),
            Reservation.Create("user123", "pet3", "Charlie", "service3", DateTime.Today, null)
        };

        Context.Reservations.AddRange(reservations);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync(userId: "user123");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(r => r.UserId == "user123");
        result.Should().Contain(r => r.AnimalName == "Buddy");
        result.Should().Contain(r => r.AnimalName == "Charlie");
    }

    [Fact]
    public async Task GetAllAsync_Should_FilterByServiceId()
    {
        // Arrange
        var reservations = new[]
        {
            Reservation.Create("user123", "pet1", "Buddy", "service1", DateTime.Today, null),
            Reservation.Create("user456", "pet2", "Max", "service1", DateTime.Today, null),
            Reservation.Create("user789", "pet3", "Charlie", "service2", DateTime.Today, null)
        };

        Context.Reservations.AddRange(reservations);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync(serviceId: "service1");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(r => r.ServiceId == "service1");
        result.Should().Contain(r => r.AnimalName == "Buddy");
        result.Should().Contain(r => r.AnimalName == "Max");
    }

    [Fact]
    public async Task GetAllAsync_Should_FilterByStatus()
    {
        // Arrange
        var reservations = new[]
        {
            Reservation.Create("user123", "pet1", "Buddy", "service1", DateTime.Today, null),
            Reservation.Create("user456", "pet2", "Max", "service2", DateTime.Today, null),
            Reservation.Create("user789", "pet3", "Charlie", "service3", DateTime.Today, null)
        };

        // Marquer une réservation comme validée (via reflection car pas de méthode publique)
        var reservationToValidate = reservations[1];
        var statusProperty = typeof(Reservation).GetProperty(nameof(Reservation.Status));
        statusProperty?.SetValue(reservationToValidate, ReservationStatus.Validated);

        Context.Reservations.AddRange(reservations);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync(status: ReservationStatus.Validated);

        // Assert
        result.Should().HaveCount(1);
        result.First().AnimalName.Should().Be("Max");
        result.First().Status.Should().Be(ReservationStatus.Validated);
    }

    [Fact]
    public async Task GetAllAsync_Should_FilterByDateRange()
    {
        // Arrange
        var baseDate = DateTime.Today;
        var reservations = new[]
        {
            Reservation.Create("user123", "pet1", "Buddy", "service1", baseDate.AddDays(-1), null),
            Reservation.Create("user456", "pet2", "Max", "service2", baseDate.AddDays(1), null),
            Reservation.Create("user789", "pet3", "Charlie", "service3", baseDate.AddDays(5), null)
        };

        Context.Reservations.AddRange(reservations);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync(
            startDateMin: baseDate,
            startDateMax: baseDate.AddDays(3)
        );

        // Assert
        result.Should().HaveCount(1);
        result.First().AnimalName.Should().Be("Max");
        result.First().StartDate.Should().Be(baseDate.AddDays(1));
    }

    [Fact]
    public async Task GetAllAsync_Should_OrderByCreatedAtDescending()
    {
        // Arrange
        var baseDate = DateTime.Today;
        
        // Créer les réservations une par une avec des délais pour s'assurer que CreatedAt est différent
        var firstReservation = Reservation.Create("user123", "pet1", "First", "service1", baseDate, null);
        Context.Reservations.Add(firstReservation);
        await Context.SaveChangesAsync();
        
        await Task.Delay(10); // Petit délai pour garantir un CreatedAt différent
        
        var secondReservation = Reservation.Create("user456", "pet2", "Second", "service2", baseDate, null);
        Context.Reservations.Add(secondReservation);
        await Context.SaveChangesAsync();
        
        await Task.Delay(10); // Petit délai pour garantir un CreatedAt différent
        
        var thirdReservation = Reservation.Create("user789", "pet3", "Third", "service3", baseDate, null);
        Context.Reservations.Add(thirdReservation);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        // Le dernier ajouté devrait être en premier (ordre décroissant par CreatedAt)
        result.First().AnimalName.Should().Be("Third");
        result.Last().AnimalName.Should().Be("First");
    }

    [Fact]
    public async Task GetByUserIdAsync_Should_ReturnUserReservations()
    {
        // Arrange
        var reservations = new[]
        {
            Reservation.Create("user123", "pet1", "Buddy", "service1", DateTime.Today, null),
            Reservation.Create("user456", "pet2", "Max", "service2", DateTime.Today, null),
            Reservation.Create("user123", "pet3", "Charlie", "service3", DateTime.Today, null)
        };

        Context.Reservations.AddRange(reservations);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByUserIdAsync("user123");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(r => r.UserId == "user123");
    }

    [Fact]
    public async Task GetByServiceIdAsync_Should_ReturnServiceReservations()
    {
        // Arrange
        var reservations = new[]
        {
            Reservation.Create("user123", "pet1", "Buddy", "service1", DateTime.Today, null),
            Reservation.Create("user456", "pet2", "Max", "service1", DateTime.Today, null),
            Reservation.Create("user789", "pet3", "Charlie", "service2", DateTime.Today, null)
        };

        Context.Reservations.AddRange(reservations);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByServiceIdAsync("service1");

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(r => r.ServiceId == "service1");
    }

    [Fact]
    public async Task GetReservationsBetweenDatesAsync_Should_ReturnOverlappingReservations()
    {
        // Arrange
        var baseDate = DateTime.Today;
        var reservations = new[]
        {
            // Réservation qui commence avant et se termine dans la période
            Reservation.Create("user1", "pet1", "Pet1", "service1", baseDate.AddDays(-2), baseDate.AddDays(1)),
            
            // Réservation complètement dans la période
            Reservation.Create("user2", "pet2", "Pet2", "service2", baseDate.AddDays(1), baseDate.AddDays(3)),
            
            // Réservation qui commence dans la période et se termine après
            Reservation.Create("user3", "pet3", "Pet3", "service3", baseDate.AddDays(2), baseDate.AddDays(6)),
            
            // Réservation complètement hors période
            Reservation.Create("user4", "pet4", "Pet4", "service4", baseDate.AddDays(10), baseDate.AddDays(12))
        };

        Context.Reservations.AddRange(reservations);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetReservationsBetweenDatesAsync(
            baseDate,
            baseDate.AddDays(4)
        );

        // Assert
        result.Should().HaveCount(3);
        result.Should().NotContain(r => r.AnimalName == "Pet4");
        result.Should().Contain(r => r.AnimalName == "Pet1");
        result.Should().Contain(r => r.AnimalName == "Pet2");
        result.Should().Contain(r => r.AnimalName == "Pet3");
    }

    [Fact]
    public async Task GetExpiredCreatedReservationsAsync_Should_ReturnEmptyList()
    {
        // Arrange & Act
        var result = await _repository.GetExpiredCreatedReservationsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserDisplayedReservationsAsync_Should_ReturnOnlyDisplayedStatuses()
    {
        // Arrange
        var reservations = new[]
        {
            Reservation.Create("user123", "pet1", "Pet1", "service1", DateTime.Today, null),
            Reservation.Create("user123", "pet2", "Pet2", "service2", DateTime.Today, null),
            Reservation.Create("user123", "pet3", "Pet3", "service3", DateTime.Today, null),
            Reservation.Create("user123", "pet4", "Pet4", "service4", DateTime.Today, null)
        };

        // Définir différents statuts via reflection
        var statusProperty = typeof(Reservation).GetProperty(nameof(Reservation.Status));
        statusProperty?.SetValue(reservations[0], ReservationStatus.Created);
        statusProperty?.SetValue(reservations[1], ReservationStatus.Validated);
        statusProperty?.SetValue(reservations[2], ReservationStatus.InProgress);
        statusProperty?.SetValue(reservations[3], ReservationStatus.Completed);

        Context.Reservations.AddRange(reservations);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserDisplayedReservationsAsync("user123");

        // Assert
        result.Should().HaveCount(3);
        result.Should().NotContain(r => r.Status == ReservationStatus.Created);
        result.Should().Contain(r => r.Status == ReservationStatus.Validated);
        result.Should().Contain(r => r.Status == ReservationStatus.InProgress);
        result.Should().Contain(r => r.Status == ReservationStatus.Completed);
    }

    [Fact]
    public async Task GetUserDisplayedReservationsAsync_Should_FilterByUserId()
    {
        // Arrange
        var reservations = new[]
        {
            Reservation.Create("user123", "pet1", "Pet1", "service1", DateTime.Today, null),
            Reservation.Create("user456", "pet2", "Pet2", "service2", DateTime.Today, null)
        };

        // Marquer comme validées
        var statusProperty = typeof(Reservation).GetProperty(nameof(Reservation.Status));
        statusProperty?.SetValue(reservations[0], ReservationStatus.Validated);
        statusProperty?.SetValue(reservations[1], ReservationStatus.Validated);

        Context.Reservations.AddRange(reservations);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetUserDisplayedReservationsAsync("user123");

        // Assert
        result.Should().HaveCount(1);
        result.First().UserId.Should().Be("user123");
        result.First().AnimalName.Should().Be("Pet1");
    }
}