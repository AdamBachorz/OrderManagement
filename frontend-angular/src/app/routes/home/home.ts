import { HttpClient } from '@angular/common/http';
import { Component, inject } from '@angular/core';
import { MatIcon } from '@angular/material/icon';
import { debounceTime } from 'rxjs';

@Component({
  imports: [MatIcon],
  selector: 'app-home',
  styleUrl: './home.scss',
  templateUrl: './home.html',
})
export class Home {
  private http = inject(HttpClient);
  labelText = 'Default';

  constructor() {}

  onClickButton(): void {
    this.http
      .get('/api/test')
      .pipe(debounceTime(100))
      .subscribe({
        next: x => alert(x),
        error: err => console.error('Szczegóły błędu:', err),
      });
  }
}
