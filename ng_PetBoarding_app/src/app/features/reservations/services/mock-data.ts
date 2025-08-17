// Mock data avec le nouveau système de planning
export const MOCK_RESERVATIONS_AVEC_PLANNING = [
  {
    id: '1',
    utilisateurId: 'user-1',
    animalId: 'animal-1',
    animalNom: 'Rex',
    animalType: 'CHIEN' as const,
    prestationId: '1',
    prestationLibelle: 'Pension complète',
    dateDebut: new Date('2025-08-20'),
    dateFin: new Date('2025-08-25'),
    prixTotal: 175, // 5 jours × 35€
    statut: 'CONFIRMEE' as const,
    commentaires: 'Rex adore jouer avec les autres chiens',
    dateCreation: new Date('2025-07-20'),
    dateModification: undefined,
    datesReservees: [
      new Date('2025-08-20'),
      new Date('2025-08-21'),
      new Date('2025-08-22'),
      new Date('2025-08-23'),
      new Date('2025-08-24'),
      new Date('2025-08-25')
    ],
    nombreJours: 6
  },
  {
    id: '2',
    utilisateurId: 'user-2',
    animalId: 'animal-2',
    animalNom: 'Minou',
    animalType: 'CHAT' as const,
    prestationId: '6',
    prestationLibelle: 'Pension chat',
    dateDebut: new Date('2025-08-18'),
    dateFin: new Date('2025-08-20'),
    prixTotal: 75, // 3 jours × 25€
    statut: 'EN_COURS' as const,
    commentaires: 'Chat très timide, préfère la tranquillité',
    dateCreation: new Date('2025-07-25'),
    dateModification: undefined,
    datesReservees: [new Date('2025-08-18'), new Date('2025-08-19'), new Date('2025-08-20')],
    nombreJours: 3
  },
  {
    id: '3',
    utilisateurId: 'user-1',
    animalId: 'animal-3',
    animalNom: 'Buddy',
    animalType: 'CHIEN' as const,
    prestationId: '4',
    prestationLibelle: 'Promenade',
    dateDebut: new Date('2025-08-22'),
    dateFin: undefined,
    prixTotal: 15,
    statut: 'CONFIRMEE' as const,
    commentaires: 'Promenade matinale',
    dateCreation: new Date('2025-08-15'),
    dateModification: undefined,
    datesReservees: [new Date('2025-08-22')],
    nombreJours: 1
  },
  {
    id: '4',
    utilisateurId: 'user-3',
    animalId: 'animal-4',
    animalNom: 'Luna',
    animalType: 'CHAT' as const,
    prestationId: '5',
    prestationLibelle: 'Garde à domicile',
    dateDebut: new Date('2025-08-25'),
    dateFin: new Date('2025-08-27'),
    prixTotal: 60, // 3 jours × 20€
    statut: 'EN_ATTENTE' as const,
    commentaires: 'Garde pendant le weekend',
    dateCreation: new Date('2025-08-17'),
    dateModification: undefined,
    datesReservees: [new Date('2025-08-25'), new Date('2025-08-26'), new Date('2025-08-27')],
    nombreJours: 3
  },
  {
    id: '5',
    utilisateurId: 'user-2',
    animalId: 'animal-5',
    animalNom: 'Charlie',
    animalType: 'CHIEN' as const,
    prestationId: '2',
    prestationLibelle: 'Garderie journée',
    dateDebut: new Date('2025-08-19'),
    dateFin: undefined,
    prixTotal: 25,
    statut: 'TERMINEE' as const,
    commentaires: 'Charlie a passé une excellente journée',
    dateCreation: new Date('2025-08-18'),
    dateModification: new Date('2025-08-19'),
    datesReservees: [new Date('2025-08-19')],
    nombreJours: 1
  }
];

export const MOCK_PLANNINGS_EXEMPLE = [
  {
    id: 'planning-toilettage-exemple',
    prestationId: '3',
    nom: 'Planning Toilettage Complet - Exemple',
    description: 'Exemple de planning avec créneaux variés pour le toilettage',
    estActif: true,
    dateCreation: new Date('2025-01-01'),
    creneaux: [
      // Semaine avec créneaux complets et partiels
      { date: new Date('2025-08-25'), capaciteMax: 4, capaciteReservee: 4, capaciteDisponible: 0 }, // Complet
      { date: new Date('2025-08-26'), capaciteMax: 4, capaciteReservee: 2, capaciteDisponible: 2 }, // Partiel
      { date: new Date('2025-08-27'), capaciteMax: 4, capaciteReservee: 1, capaciteDisponible: 3 }, // Disponible
      { date: new Date('2025-08-28'), capaciteMax: 4, capaciteReservee: 0, capaciteDisponible: 4 }, // Libre
      { date: new Date('2025-08-29'), capaciteMax: 4, capaciteReservee: 3, capaciteDisponible: 1 }, // Presque complet
      { date: new Date('2025-08-30'), capaciteMax: 6, capaciteReservee: 2, capaciteDisponible: 4 }, // Weekend
      { date: new Date('2025-08-31'), capaciteMax: 6, capaciteReservee: 1, capaciteDisponible: 5 } // Weekend
    ]
  }
];
