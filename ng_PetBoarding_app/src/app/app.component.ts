import { HttpClient } from '@angular/common/http';
import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: 'app.component.html',
  styleUrl: './app.component.css'
})

export class AppComponent implements OnInit{  
  http = inject(HttpClient);
  title = 'ng-PetBoarding-app';
  data: any;

  ngOnInit(): void {
    this.http.get('https://jsonplaceholder.typicode.com/todos/1').subscribe({
      next: response => this.data = response,
      error: error => console.log(error),
      complete: () => console.log('Request has completed')
    })
  }
}
