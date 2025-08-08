import { CommonModule } from '@angular/common';
import { Component, signal } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-contact',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatSelectModule,
    MatSnackBarModule,
    RouterModule
  ],
  templateUrl: './contact.component.html',
  styleUrl: './contact.component.scss'
})
export class ContactComponent {
  contactForm: FormGroup;
  isSubmitting = signal(false);

  // Informations de contact
  contactInfo = {
    address: {
      street: '123 Avenue des Animaux',
      city: '75016 Paris',
      country: 'France'
    },
    phone: '+33 1 23 45 67 89',
    email: 'contact@petboarding.fr',
    emergencyPhone: '+33 6 12 34 56 78',
    hours: {
      weekdays: '8h00 - 19h00',
      weekend: '9h00 - 17h00',
      holidays: 'Sur rendez-vous'
    }
  };

  // Motifs de contact
  contactReasons = [
    { value: 'reservation', label: 'Nouvelle réservation' },
    { value: 'information', label: "Demande d'information" },
    { value: 'emergency', label: 'Urgence' },
    { value: 'complaint', label: 'Réclamation' },
    { value: 'partnership', label: 'Partenariat' },
    { value: 'other', label: 'Autre' }
  ];

  // Services rapides
  quickServices = [
    {
      title: 'Réservation Express',
      description: 'Réservez directement en ligne',
      icon: 'event_available',
      action: 'reserve',
      color: 'primary'
    },
    {
      title: 'Urgence 24h/24',
      description: "Pour les situations d'urgence",
      icon: 'emergency',
      action: 'emergency',
      color: 'warn'
    },
    {
      title: 'Visite des Locaux',
      description: 'Planifiez une visite guidée',
      icon: 'tour',
      action: 'visit',
      color: 'accent'
    }
  ];

  // FAQ
  faqItems = [
    {
      question: "Quels sont vos horaires d'ouverture ?",
      answer:
        'Nous sommes ouverts du lundi au vendredi de 8h à 19h, et le week-end de 9h à 17h. Pour les urgences, un service 24h/24 est disponible.'
    },
    {
      question: 'Puis-je visiter vos installations avant de réserver ?',
      answer:
        'Bien sûr ! Nous encourageons les visites. Contactez-nous pour planifier une visite guidée de nos installations.'
    },
    {
      question: 'Que dois-je apporter pour mon animal ?',
      answer:
        'Apportez la nourriture habituelle, les médicaments si nécessaire, un objet familier (jouet, couverture) et le carnet de santé à jour.'
    },
    {
      question: "Acceptez-vous tous les types d'animaux ?",
      answer:
        "Nous accueillons principalement les chiens et chats. Pour d'autres animaux, contactez-nous pour étudier les possibilités."
    }
  ];

  constructor(
    private fb: FormBuilder,
    private snackBar: MatSnackBar,
    private router: Router
  ) {
    this.contactForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.required, Validators.pattern(/^[+]?[0-9\s\-()]+$/)]],
      reason: ['', Validators.required],
      subject: ['', [Validators.required, Validators.minLength(5)]],
      message: ['', [Validators.required, Validators.minLength(20)]]
    });
  }

  onSubmit() {
    if (this.contactForm.valid) {
      this.isSubmitting.set(true);

      // Simulation d'envoi
      setTimeout(() => {
        this.isSubmitting.set(false);
        this.snackBar.open(
          'Votre message a été envoyé avec succès ! Nous vous répondrons dans les plus brefs délais.',
          'Fermer',
          {
            duration: 5000,
            panelClass: ['success-snackbar']
          }
        );
        this.contactForm.reset();
      }, 2000);
    } else {
      this.markFormGroupTouched();
    }
  }

  onQuickAction(action: string) {
    switch (action) {
      case 'reserve':
        // Navigation vers la page prestations
        this.router.navigate(['/prestations']);
        break;
      case 'emergency':
        this.callEmergency();
        break;
      case 'visit':
        this.scheduleVisit();
        break;
    }
  }

  private callEmergency() {
    this.snackBar.open(`Appelez immédiatement le ${this.contactInfo.emergencyPhone}`, 'Appeler', {
      duration: 10000,
      panelClass: ['emergency-snackbar']
    });
  }

  scheduleVisit() {
    this.contactForm.patchValue({
      reason: 'information',
      subject: 'Demande de visite des locaux'
    });
    // Scroll vers le formulaire
    document.querySelector('#contact-form')?.scrollIntoView({
      behavior: 'smooth'
    });
  }

  private markFormGroupTouched() {
    Object.keys(this.contactForm.controls).forEach((key) => {
      const control = this.contactForm.get(key);
      control?.markAsTouched();
    });
  }

  getErrorMessage(fieldName: string): string {
    const control = this.contactForm.get(fieldName);
    if (control?.hasError('required')) {
      return 'Ce champ est obligatoire';
    }
    if (control?.hasError('email')) {
      return 'Veuillez saisir un email valide';
    }
    if (control?.hasError('minlength')) {
      const requiredLength = control.errors?.['minlength']?.requiredLength;
      return `Minimum ${requiredLength} caractères requis`;
    }
    if (control?.hasError('pattern')) {
      return 'Format invalide';
    }
    return '';
  }
}
